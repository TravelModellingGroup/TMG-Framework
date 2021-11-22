﻿/*
    Copyright 2018-2021 University of Toronto

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
using System.Text;

namespace TMG.Test.Data
{
    [TestClass]
    public class CategoryMapTest
    {
        [TestMethod]
        public void Aggregate()
        {
            string error = null;
            Categories a = Categories.CreateCategories(new List<int> { 1, 3, 5, 7 }, ref error);
            Assert.IsNotNull(a, error);
            Categories b = Categories.CreateCategories(new List<int> { 2, 4 }, ref error);
            Assert.IsNotNull(b, error);
            Assert.IsTrue(CategoryMap.CreateCategoryMap(a, b,
                new List<(int originFlatIndex, int destinationFlatIndex)>()
                {
                    (0, 0),
                    (1, 0),
                    (2, 1),
                    (3, 1)
                }, out var map, ref error), error);
            var va = new Vector(a);
            va.Data[0] = 3;
            va.Data[1] = 7;
            va.Data[2] = 2;
            va.Data[3] = 4;
            Assert.IsTrue(map.AggregateToDestination(va, out var result, ref error), error);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreSame(b, result.Categories);
            Assert.AreEqual(10, result.Data[0]);
            Assert.AreEqual(6, result.Data[1]);
        }

        [TestMethod]
        public void CreateIndex()
        {
            string error = null;
            Categories a = Categories.CreateCategories(new List<int> { 1, 3, 5, 7 }, ref error);
            Assert.IsNotNull(a, error);
            Categories b = Categories.CreateCategories(new List<int> { 2, 4 }, ref error);
            Assert.IsNotNull(b, error);
            Assert.IsTrue(CategoryMap.CreateCategoryMap(a, b,
                new List<(int originFlatIndex, int destinationFlatIndex)>()
                {
                    (0, 0),
                    (1, 0),
                    (2, 1),
                    (3, 1)
                }, out var map, ref error), error);
            var index = map.CreateIndex();
            Assert.AreEqual(0, (int)index[0]);
            Assert.AreEqual(0, (int)index[1]);
            Assert.AreEqual(1, (int)index[2]);
            Assert.AreEqual(1, (int)index[3]);
        }

        [TestMethod]
        public void CreateReverseIndex()
        {
            string error = null;
            Categories a = Categories.CreateCategories(new List<int> { 1, 3, 5, 7 }, ref error);
            Assert.IsNotNull(a, error);
            Categories b = Categories.CreateCategories(new List<int> { 2, 4 }, ref error);
            Assert.IsNotNull(b, error);
            Assert.IsTrue(CategoryMap.CreateCategoryMap(a, b,
                new List<(int originFlatIndex, int destinationFlatIndex)>()
                {
                    (0, 0),
                    (1, 0),
                    (2, 1),
                    (3, 1)
                }, out var map, ref error), error);
            var index = map.CreateReverseIndex();
            var list = index[0];
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(0, (int)list[0]);
            Assert.AreEqual(1, (int)list[1]);

            list = index[1];
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, (int)list[0]);
            Assert.AreEqual(3, (int)list[1]);
        }
    }
}
