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
    public class SparseMapTest
    {
        [TestMethod]
        public void CreateMap()
        {
            var set = new List<int>() { 2, 6, 4, 8, 10 };
            var map = new Categories(set);
            Assert.AreEqual(set.Count, map.Count);
            Assert.AreEqual(4, map.GetSparseIndex(1));
            Assert.AreEqual(6, map.GetSparseIndex(2));

            Assert.AreEqual(0, map.GetFlatIndex(2));
            Assert.AreEqual(1, map.GetFlatIndex(4));
            Assert.AreEqual(2, map.GetFlatIndex(6));
            Assert.AreEqual(3, map.GetFlatIndex(8));
        }
    }
}
