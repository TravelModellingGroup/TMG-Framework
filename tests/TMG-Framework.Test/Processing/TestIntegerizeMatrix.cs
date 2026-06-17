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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TMG;
using TMG.Frameworks.Data.Processing.AST;
using TMG.Processing;
using TMG.Test.Utilities;

namespace TMG.Test.Processing
{
    [TestClass]
    public class TestIntegerizeMatrix
    {
        /// <summary>
        /// This test just wants to make sure that the most basic case passes.
        /// </summary>
        [TestMethod]
        public void IntegerizeZeroMatrix()
        {
            string error = null;
            var zones = Categories.CreateCategories(new List<int>() { 1, 2, 3, 4 }, ref error);
            var pds = Categories.CreateCategories(new List<int>() { 1, 2, }, ref error);
            Assert.IsTrue(CategoryMap.CreateCategoryMap(zones, pds, new List<(int originFlatIndex, int destinationFlatIndex)>()
            {
                new (0, 0),
                new (1, 0),
                new (2, 1),
                new (3, 1)
            }, out var map, ref error));
            var inputMatrix = new Matrix(zones, zones);
            var module = new IntegerizeMatrix()
            {
                RandomSeed = Helper.CreateParameter(12345),
                InputMatrix = Helper.CreateParameter(inputMatrix),
                ZoneToPDMap = Helper.CreateParameter(map)
            };
            var integerMatrix = module.Invoke();
            Assert.IsNotNull(integerMatrix);
        }

        /// <summary>
        /// This test just wants to make sure that a simple already integer matrix doesn't change.
        /// </summary>
        [TestMethod]
        public void IntegerizeIdentityMatrix()
        {
            string error = null;
            var zones = Categories.CreateCategories(new List<int>() { 1, 2, 3, 4 }, ref error);
            var pds = Categories.CreateCategories(new List<int>() { 1, 2, }, ref error);
            Assert.IsTrue(CategoryMap.CreateCategoryMap(zones, pds, new List<(int originFlatIndex, int destinationFlatIndex)>()
            {
                new (0, 0),
                new (1, 0),
                new (2, 1),
                new (3, 1)
            }, out var map, ref error));
            var inputMatrix = new Matrix(zones, zones);
            inputMatrix.Data[0] = 1.0f;
            inputMatrix.Data[3] = 1.0f;
            var module = new IntegerizeMatrix()
            {
                RandomSeed = Helper.CreateParameter(12345),
                InputMatrix = Helper.CreateParameter(inputMatrix),
                ZoneToPDMap = Helper.CreateParameter(map)
            };
            var integerMatrix = module.Invoke();
            Assert.IsNotNull(integerMatrix);
            Assert.AreEqual(1.0f, integerMatrix.Data[0], 0.00000001f);
            Assert.AreEqual(0.0f, integerMatrix.Data[1], 0.00000001f);
            Assert.AreEqual(0.0f, integerMatrix.Data[2], 0.00000001f);
            Assert.AreEqual(1.0f, integerMatrix.Data[3], 0.00000001f);
        }

        /// <summary>
        /// Test that we can actually integerize a real matrix.
        /// </summary>
        [TestMethod]
        public void IntegerizeRealMatrix()
        {
            string error = null;
            var zones = Categories.CreateCategories(new List<int>() { 1, 2, 3, 4 }, ref error);
            var pds = Categories.CreateCategories(new List<int>() { 1, 2, }, ref error);
            Assert.IsTrue(CategoryMap.CreateCategoryMap(zones, pds, new List<(int originFlatIndex, int destinationFlatIndex)>()
            {
                new (0, 0),
                new (1, 0),
                new (2, 1),
                new (3, 1)
            }, out var map, ref error));
            var random = new Random(12345);
            var inputMatrix = new Matrix(zones, zones);
            for (int i = 0; i < inputMatrix.Data.Length; i++)
            {
                inputMatrix.Data[i] = (float)random.NextDouble();
            }
            var module = new IntegerizeMatrix()
            {
                RandomSeed = Helper.CreateParameter(12345),
                InputMatrix = Helper.CreateParameter(inputMatrix.Clone()),
                ZoneToPDMap = Helper.CreateParameter(map)
            };
            var integerMatrix = module.Invoke();
            Assert.IsNotNull(integerMatrix);

            // Make sure they are close to integers and only +- 1 from the original values
            for (int i = 0; i < integerMatrix.Data.Length; i++)
            {
                Assert.AreEqual(inputMatrix.Data[i], integerMatrix.Data[i], 1.000001f);
                Assert.AreEqual(Math.Truncate(integerMatrix.Data[i]), (double)integerMatrix.Data[0], 1.000001f);
            }
        }
    }
}
