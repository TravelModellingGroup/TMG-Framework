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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XTMF2;
using System.Reflection;
using System.Collections.Generic;

namespace TMG.Test.Data
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void CreateMatrix()
        {
            var categories = CreateMap();
            var matrix = new Matrix(categories, categories);
        }

        [TestMethod]
        public void GetSparseRowIndex()
        {
            var categories = CreateMap();
            var matrix = new Matrix(categories, categories);
            var size = matrix.RowCategories.Count;
            Assert.AreEqual(0, matrix.GetSparseRowIndex(2));
            Assert.AreEqual(size * (3 - 1), matrix.GetSparseRowIndex(6));
            Assert.AreEqual(size * (2 - 1), matrix.GetSparseRowIndex(4));
            Assert.AreEqual(size * (size - 1), matrix.GetSparseRowIndex(10));
            Assert.AreEqual(-1, matrix.GetSparseRowIndex(11));
        }

        [TestMethod]
        public void GetFlatRowIndex()
        {
            var categories = CreateMap();
            var matrix = new Matrix(categories, categories);
            var size = matrix.RowCategories.Count;
            Assert.AreEqual(0, matrix.GetFlatRowIndex(0));
            Assert.AreEqual(size * 1, matrix.GetFlatRowIndex(1));
            Assert.AreEqual(size * 2, matrix.GetFlatRowIndex(2));
            Assert.AreEqual(size * (size - 1), matrix.GetFlatRowIndex(size - 1));
            Assert.AreEqual(-1, matrix.GetSparseRowIndex(7));
        }

        private static Categories CreateMap()
        {
            string error = null;
            return Categories.CreateCategories(new List<int>() { 2, 6, 4, 8, 10 }, ref error);
        }
    }
}
