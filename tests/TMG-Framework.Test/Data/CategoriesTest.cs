/*
    Copyright 2017-2018 University of Toronto

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
    public class CategoriesTest
    {
        [TestMethod]
        public void CreateMap()
        {
            string error = null;
            var set = new List<int>() { 2, 6, 4, 8, 10 };
            var categories = Categories.CreateCategories(set, ref error);
            Assert.AreEqual(set.Count, categories.Count);
            Assert.AreEqual((CategoryIndex)4, categories.GetSparseIndex(1));
            Assert.AreEqual((CategoryIndex)6, categories.GetSparseIndex(2));

            Assert.AreEqual(0, categories.GetFlatIndex(2));
            Assert.AreEqual(1, categories.GetFlatIndex(4));
            Assert.AreEqual(2, categories.GetFlatIndex(6));
            Assert.AreEqual(3, categories.GetFlatIndex(8));
        }
    }
}
