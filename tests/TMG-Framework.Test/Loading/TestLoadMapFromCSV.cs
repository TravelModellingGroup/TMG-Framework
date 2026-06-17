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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TMG;
using TMG.Frameworks.Data.Processing.AST;
using XTMF2;
using XTMF2.RuntimeModules;
using TMG.Loading;
using TMG.Test.Utilities;

namespace TMG.Test.Loading
{
    [TestClass]
    public class TestLoadMapFromCSV
    {

        [TestMethod]
        public void TestLoadingMapFromCSV()
        {
            var mapFilePath = MapHelper.WriteCSV(64);
            var result = MapHelper.LoadMap(mapFilePath);
            Assert.AreEqual(64, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual((CategoryIndex)(i + 1), result.GetSparseIndex(i));
            }
        }

        [TestMethod]
        public void TestLoadingMapFromCSVDefinedBackwards()
        {
            var mapFilePath = MapHelper.WriteBackwardsCSV(64);
            var result = MapHelper.LoadMap(mapFilePath);
            Assert.AreEqual(64, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual((CategoryIndex)(i + 1), result.GetSparseIndex(i));
            }
        }
    }
}
