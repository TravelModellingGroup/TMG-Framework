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
using System.Linq;
using System.Text;
using TMG.Loading;
using TMG.Test.Utilities;
using XTMF2.RuntimeModules;

namespace TMG.Test.Loading
{
    [TestClass]
    public class TestLoadVector
    {
        [TestMethod]
        public void TestLoadVectorFromCSV()
        {
            var map = MapHelper.LoadMap(MapHelper.WriteCSV(64));
            float[] data = new float[64];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 2.0f + i;
            }
            var vectorFileName = VectorHelper.WriteVectorCSV(map, data);
            LoadVectorFromCSV vecLoader = new LoadVectorFromCSV()
            {
                Categories = Helper.CreateParameter(map),
                MapColumn = Helper.CreateParameter(0),
                DataColumn = Helper.CreateParameter(1)
            };
            using (var stream = (new OpenReadStreamFromFile()).Invoke(vectorFileName))
            {
                var vector = vecLoader.Invoke(stream);
                Assert.AreSame(map, vector.Categories);
                var vData = vector.Data;
                for (int i = 0; i < vData.Length; i++)
                {
                    Assert.AreEqual((float)(i + 2), vData[i], 0.00001f);
                }
            }
        }
    }
}
