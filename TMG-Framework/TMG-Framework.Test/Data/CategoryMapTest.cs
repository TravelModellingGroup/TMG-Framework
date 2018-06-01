/*
    Copyright 2018 University of Toronto

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
            Categories a = new Categories(new List<int> { 1, 3, 5, 7 });
            Categories b = new Categories(new List<int> { 2, 4 });
            CategoryMap map = new CategoryMap(a, b,
                new List<(int originFlatIndex, int destinationFlatIndex)>()
                {
                    (0, 0),
                    (1, 0),
                    (2, 1),
                    (3, 1)
                });
            var va = new Vector(a);
            va.Data[0] = 3;
            va.Data[1] = 7;
            va.Data[2] = 2;
            va.Data[3] = 4;
            var result = map.AggregateToDestination(va);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreSame(b, result.Categories);
            Assert.AreEqual(10, result.Data[0]);
            Assert.AreEqual(6, result.Data[1]);
        }
    }
}
