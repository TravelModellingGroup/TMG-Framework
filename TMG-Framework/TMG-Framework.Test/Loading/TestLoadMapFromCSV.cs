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

namespace TMG.Test.Loading
{
    [TestClass]
    public class TestLoadMapFromCSV
    {
        private static string WriteCSV()
        {
            var tempName = Path.GetTempFileName();
            try
            {
                using (StreamWriter writer = File.CreateText(tempName))
                {
                    writer.WriteLine("Zone");
                    for (int i = 0; i < 64; i++)
                    {
                        writer.WriteLine(i + 1);
                    }
                }
            }
            catch
            {
                File.Delete(tempName);
                Assert.Fail("Unable to create map file!");
            }
            return tempName;
        }

        [TestMethod]
        public void TestLoadingMapFromCSV()
        {
            var mapFilePath = WriteCSV();
            try
            {
                LoadMapFromCSV mapLoader = new LoadMapFromCSV();
                OpenReadStreamFromFile streamLoader = new OpenReadStreamFromFile();
                using (var reader = streamLoader.Invoke(mapFilePath))
                {
                    var map = mapLoader.Invoke(reader);
                    Assert.AreEqual(64, map.Count);
                    for (int i = 0; i < map.Count; i++)
                    {
                        Assert.AreEqual(i + 1, map.GetSparseIndex(i));
                    }
                }
            }
            finally
            {
                File.Delete(mapFilePath);
            }
        }
    }
}
