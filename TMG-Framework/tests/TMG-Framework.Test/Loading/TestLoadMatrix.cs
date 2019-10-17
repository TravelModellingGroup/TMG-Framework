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
    public class TestLoadMatrix
    {
        [TestMethod]
        public void LoadMatrixFromCSVMatrix()
        {
            var map = MapHelper.LoadMap(MapHelper.WriteCSV(64));
            float[][] data = new float[64][];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new float[64];
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = 2.0f + i * j;
                }
            }
            var matrixFileName = MatrixHelper.WriteMatrixToCSVMatrix(map, data);
            var matrixLoader = new LoadMatrixFromCSVMatrix()
            {
                ColumnCategories = Helper.CreateParameter(map),
                RowCategories = Helper.CreateParameter(map)
            };
            using (var stream = (new OpenReadStreamFromFile()
            {
                FilePath = Helper.CreateParameter(matrixFileName)
            }).Invoke())
            {
                var vector = matrixLoader.Invoke(stream);
                Assert.AreSame(map, vector.RowCategories);
                var vData = vector.Data;
                for (int i = 0; i < map.Count; i++)
                {
                    for (int j = 0; j < map.Count; j++)
                    {
                        if(Math.Abs((2.0f + i * j) - vData[i * map.Count + j]) < 0.00001f)
                        {
                            Assert.AreEqual(2.0f + i * j, vData[i * map.Count + j], 0.00001f);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void LoadMatrixFromCSVThirdNormalized()
        {
            var map = MapHelper.LoadMap(MapHelper.WriteCSV(64));
            float[][] data = new float[64][];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new float[64];
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = 2.0f + i * j;
                }
            }
            var matrixFileName = MatrixHelper.WriteMatrixToCSVThirdNormalized(map, data);
            var matrixLoader = new LoadMatrixFromCSVThirdNormalized()
            {
                RowCategories = Helper.CreateParameter(map),
                ColumnCategories = Helper.CreateParameter(map),
                OriginColumn = Helper.CreateParameter(0),
                DestinationColumn = Helper.CreateParameter(0),
                DataColumn = Helper.CreateParameter(0)
            };
            using (var stream = (new OpenReadStreamFromFile()
            {
                FilePath = Helper.CreateParameter(matrixFileName)
            }).Invoke())
            {
                var vector = matrixLoader.Invoke(stream);
                Assert.AreSame(map, vector.RowCategories);
                var vData = vector.Data;
                for (int i = 0; i < map.Count; i++)
                {
                    for (int j = 0; j < map.Count; j++)
                    {
                        if (Math.Abs((2.0f + i * j) - vData[i * map.Count + j]) < 0.00001f)
                        {
                            Assert.AreEqual(2.0f + i * j, vData[i * map.Count + j], 0.00001f);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestLoadMatrixFromMTX()
        {
            var map = MapHelper.LoadMap(MapHelper.WriteCSV(64));
            float[][] data = new float[64][];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new float[64];
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = 2.0f + i * j;
                }
            }
            var matrixFileName = MatrixHelper.WriteMatrixToMTX(map, data);
            var matrixLoader = new LoadMatrixFromMTX()
            {
                Categories = Helper.CreateParameter(map),
            };
            using (var stream = (new OpenReadStreamFromFile()
            {
                FilePath = Helper.CreateParameter(matrixFileName)
            }).Invoke())
            {
                var vector = matrixLoader.Invoke(stream);
                Assert.AreSame(map, vector.RowCategories);
                var vData = vector.Data;
                for (int i = 0; i < map.Count; i++)
                {
                    for (int j = 0; j < map.Count; j++)
                    {
                        if (Math.Abs((2.0f + i * j) - vData[i * map.Count + j]) < 0.00001f)
                        {
                            Assert.AreEqual(2.0f + i * j, vData[i * map.Count + j], 0.00001f);
                        }
                    }
                }
            }
        }
    }
}
