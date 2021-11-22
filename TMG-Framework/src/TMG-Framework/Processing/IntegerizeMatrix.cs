/*
    Copyright 2021 University of Toronto

    This file is part of TMG-Framework for XTMF2.

    TMG-Framework for XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TMG-Framework for XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TMG-Framework for XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XTMF2;
using TMG;
using System.Threading.Tasks;

namespace TMG.Processing
{
    [Module(Name = "Integerize Matrix", Description = "Takes in a matrix and then integerizes it trying to keep the total by planning district.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class IntegerizeMatrix : BaseFunction<Matrix>
    {
        [SubModule(Required = true, Index = 0, Name = "Input Matrix", Description = "The matrix that will be integerized.")]
        public IFunction<Matrix> InputMatrix;

        [Parameter(Index = 1, Name = "Random Seed", Description = "The number used to initialize the random number generator.")]
        public IFunction<int> RandomSeed;

        [SubModule(Required = true, Index = 2, Name = "Zone To PD Map", Description = "A mapping between zone numbers and")]
        public IFunction<CategoryMap> ZoneToPDMap;

        /// <summary>
        /// Computes an integer matrix given the input matrix, randomly assigning the remainders within
        /// the planning district for the zones.
        /// </summary>
        /// <returns>The integerized matrix.</returns>
        public override Matrix Invoke()
        {
            var baseMatrix = InputMatrix!.Invoke();
            var pdMap = ZoneToPDMap!.Invoke();
            var zoneToPD = pdMap.CreateIndex();
            var remainders = SplitIntegerAndRemainderMatrix(baseMatrix, pdMap, zoneToPD, out var pdRemainders);
            AssignIntegerRemainders(baseMatrix, remainders, pdRemainders, zoneToPD);
            return baseMatrix;
        }

        /// <summary>
        /// Splits the integer portion of the matrix from the remainders.
        /// The rawMatrix will be integerized.
        /// </summary>
        /// <param name="rawMatrix">The matrix containing both the integer and remainder data</param>
        /// <param name="pdMap">The mapping between zone indexes and pd indexes</param>
        /// <param name="pdRemainderTotals">The </param>
        /// <returns>A new matrix containing the remainders</returns>
        private static Matrix SplitIntegerAndRemainderMatrix(Matrix rawMatrix, CategoryMap pdMap, Dictionary<CategoryIndex, CategoryIndex> zoneToPd, out Matrix pdRemainderTotals)
        {
            var remainders = new Matrix(rawMatrix.RowCategories, rawMatrix.ColumnCategories);
            var pdMatrix = new Matrix(pdMap.Destination, pdMap.Destination);

            // Split the integer and remainders while also accumulating the remainders into a PDxPD matrix
            Parallel.For(0, rawMatrix.RowCategories.Count,
                () =>
                {
                    return new Matrix(pdMap.Destination, pdMap.Destination);
                }
                , (int i, ParallelLoopState _, Matrix pdRemainders) =>
                {
                    int pdI = zoneToPd[i];
                    var iData = rawMatrix.GetRow(i);
                    var rData = remainders.GetRow(i);
                    var pdRow = pdRemainders.GetRow(pdI);
                    for (int j = 0; j < rawMatrix.ColumnCategories.Count; j++)
                    {
                        int pdJ = zoneToPd[j];
                        var original = iData[j];
                        iData[j] = (float)Math.Truncate(original);
                        rData[j] = original - iData[j];
                        pdRow[pdJ] += rData[j];
                    }
                    return pdRemainders;
                }, (pdRemainders) =>
                {
                    lock (pdMatrix)
                    {
                        Utilities.VectorHelper.Add(pdMatrix.Data, pdMatrix.Data, pdRemainders.Data);
                    }
                });
            pdRemainderTotals = pdMatrix;
            return remainders;
        }

        private void AssignIntegerRemainders(Matrix integers, Matrix remainders, Matrix pdRemainders, Dictionary<CategoryIndex, CategoryIndex> zoneToPD)
        {
            var flatZones = integers.RowCategories;
            var pdIndexes = flatZones.Select((z, i) => zoneToPD[i]).ToArray();
            var numberOfPDs = pdRemainders.RowCategories.Count;
            // Create indexes to look for each PDxPD
            var pairs = new List<ODPair>[numberOfPDs * numberOfPDs];
            for (int i = 0; i < flatZones.Count; i++)
            {
                var row = new Span<List<ODPair>>(pairs, pdIndexes[i] * numberOfPDs, numberOfPDs);
                for (int j = 0; j < flatZones.Count; j++)
                {
                    var list = row[pdIndexes[j]];
                    if (list == null)
                    {
                        list = row[pdIndexes[j]] = new List<ODPair>(100);
                    }
                    list.Add(new ODPair() { Origin = i, Destination = j });
                }
            }

            var random = new Random(RandomSeed!.Invoke());

            // Method to assign an additional trip to the integer matrix based on the remainders for the given pd of origin and destination
            void Assign(List<ODPair> zoneList, double pop, ref float pdTotal)
            {
                for (int z = 0; z < zoneList.Count; z++)
                {
                    int i = zoneList[z].Origin;
                    int j = zoneList[z].Destination;
                    var index = i * flatZones.Count + j;
                    pop -= remainders.Data[index];
                    if (pop <= 0)
                    {
                        integers.Data[index] += 1.0f;
                        pdTotal -= remainders.Data[index];
                        remainders.Data[index] = 0.0f;
                        return;
                    }
                }
            }

            // Create the random seeds to work with for each PDxPD            
            var seeds = new int[pairs.Length];
            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = random.Next(int.MinValue, int.MaxValue);
            }

            // Solve for each PDxPD
            Parallel.For(0, pairs.Length, (int index) =>
            {
                var r = new Random(seeds[index]);
                int toAssign = (int)Math.Round(pdRemainders.Data[index]);
                var zoneList = pairs[index];
                for (int k = 0; k < toAssign; k++)
                {
                    double pop = r.NextDouble() * pdRemainders.Data[index];
                    Assign(zoneList, pop, ref pdRemainders.Data[index]);
                }
            });
        }

        struct ODPair
        {
            internal int Origin;
            internal int Destination;
        }
    }
}
