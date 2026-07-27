/*
    Copyright 2017 University of Toronto

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
using System.Threading.Tasks;
using TMG.Utilities;
using XTMF2;

namespace TMG.Processing
{
    [Module(Name = "Evaluate 2D Gravity Model", Description = "Evaluate the result of the 2D Gravity model.",
    DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class Evaluate2DGravityModel : BaseFunction<Matrix>
    {
        [SubModule(Name = "Production", Required = true, Index = 0, Description = "")]
        public IFunction<Vector> Production = null!;

        [SubModule(Name = "Attraction", Required = true, Index = 1, Description = "")]
        public IFunction<Vector> Attraction = null!;

        [SubModule(Name = "Friction", Required = true, Index = 2, Description = "")]
        public IFunction<Matrix> Friction = null!;

        [Parameter(Name = "Max Iterations", Index = 3, DefaultValue = "100", Description = "The maximum number of iterations before terminating.")]
        public IFunction<int> MaxIterations = null!;

        [Parameter(Name = "Max Error", Index = 4, DefaultValue = "0.05", Description = "The maximum amount of error before terminating.")]
        public IFunction<float> MaxError = null!;

        private static void Apply(Matrix ret, Matrix friction, Vector production,
            Vector attraction, Vector attractionStar, float[] columnTotals)
        {
            Parallel.For(0, production.Count, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                () => new float[columnTotals.Length],
                (flatOrigin, state, localTotals) =>
                {
                    var sProduction = production.Data;
                    var length = sProduction.Length;
                    var rowIndex = length * flatOrigin;
                    var sAttraction = attraction.Data;
                    var sFriction = friction.Data.Slice(rowIndex, length);
                    var sRet = ret.Data.Slice(rowIndex, length);
                    var sAttractionStar = attractionStar.Data;
                    
                    // check to see if there is no production, if not skip this
                    if (sProduction[flatOrigin] > 0)
                    {
                        var sumAf = VectorHelper.Multiply3AndSum(sFriction, sAttraction, sAttractionStar);
                        sumAf = (sProduction[flatOrigin] / sumAf);
                        if (float.IsInfinity(sumAf) | float.IsNaN(sumAf))
                        {
                            // this needs to be 0f, otherwise we will be making the attractions have to be balanced higher
                            sumAf = 0f;
                        }
                        VectorHelper.Multiply3Scalar1AndColumnSum(sRet, sFriction, sAttraction, sAttractionStar, sumAf, localTotals.AsSpan());
                    }
                    return localTotals;
                },
            localTotals =>
            {
                lock (columnTotals)
                {
                    VectorHelper.Add(columnTotals, 0, columnTotals, 0, localTotals, 0, columnTotals.Length);
                }
            });
        }

        private bool Balance(Vector flatAttractions, Vector flatAttractionStar, float epsilon, float[] columnTotals)
        {
            VectorHelper.Divide(columnTotals.AsSpan(), flatAttractions.Data, columnTotals.AsSpan());
            VectorHelper.Multiply(flatAttractionStar.Data, flatAttractionStar.Data, columnTotals);
            VectorHelper.ReplaceIfNotFinite(flatAttractionStar.Data, 1.0f);
            return VectorHelper.AreBoundedBy(columnTotals, 0, 1.0f, epsilon, columnTotals.Length);
        }

        public override Matrix Invoke()
        {
            var production = Production.Invoke();
            var attraction = Attraction.Invoke();
            var friction = Friction.Invoke();
            var attractionStar = new Vector(attraction);
            SetToOne(attractionStar);
            var ret = new Matrix(friction);
            var maxIterations = MaxIterations.Invoke();
            var maxError = MaxError.Invoke();
            if (production.Categories != attraction.Categories)
            {
                throw new XTMFRuntimeException(this, "The production and attraction are not of the same type.");
            }
            if (production.Categories != friction.RowCategories)
            {
                throw new XTMFRuntimeException(this, "The production and friction are not of the same type.");
            }
            float[] columnTotals = new float[attraction.Data.Length];
            int iteration = 0;
            do
            {
                Array.Clear(columnTotals, 0, columnTotals.Length);
                Apply(ret, friction, production, attraction, attractionStar, columnTotals);
            } while (!Balance(attraction, attractionStar, maxError, columnTotals) || (++iteration < maxIterations));
            return ret;
        }

        private static void SetToOne(Vector attractionStar)
        {
            var aStarData = attractionStar.Data;
            for (int i = 0; i < aStarData.Length; i++)
            {
                aStarData[i] = 1.0f;
            }
        }
    }
}
