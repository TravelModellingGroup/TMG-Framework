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
using System.Text;
using TMG.Saving;

namespace TMG.Test.Utilities
{
    class MatrixHelper
    {
        internal static string WriteMatrixToCSVMatrix(Map map, float[][] data)
        {
            Assert.AreEqual(map.Count, data.Length);
            var fileName = Path.GetTempFileName();
            try
            {
                var matrix = new Matrix(map);
                for (int i = 0; i < data.Length; i++)
                {
                    Array.Copy(data[i], 0, matrix.Data, i * data.Length, data[i].Length);
                }
                var save = new SaveMatrixAsCSV()
                {
                    ThirdNormalized = Helper.CreateParameter(false),
                    FirstIndexHeader = Helper.CreateParameter("Origin"),
                    SecondIndexHeader = Helper.CreateParameter("Destination")
                };
                using (var writeStream = (new XTMF2.RuntimeModules.OpenWriteStreamFromFile())
                    .Invoke(fileName))
                {
                    save.Invoke((matrix, writeStream));
                }
                return fileName;
            }
            catch
            {
                File.Delete(fileName);
                Assert.Fail("Unable to write matrix CSV file");
                return null;
            }
        }

        internal static string WriteMatrixToCSVThirdNormalized(Map map, float[][] data)
        {
            Assert.AreEqual(map.Count, data.Length);
            var fileName = Path.GetTempFileName();
            try
            {
                var matrix = new Matrix(map);
                for (int i = 0; i < data.Length; i++)
                {
                    Array.Copy(data[i], 0, matrix.Data, i * data.Length, data[i].Length);
                }
                var save = new SaveMatrixAsCSV()
                {
                    ThirdNormalized = Helper.CreateParameter(true),
                    FirstIndexHeader = Helper.CreateParameter("Origin"),
                    SecondIndexHeader = Helper.CreateParameter("Destination"),
                    DataIndexHeader = Helper.CreateParameter("Data")
                };
                using (var writeStream = (new XTMF2.RuntimeModules.OpenWriteStreamFromFile())
                    .Invoke(fileName))
                {
                    save.Invoke((matrix, writeStream));
                }
                return fileName;
            }
            catch
            {
                File.Delete(fileName);
                Assert.Fail("Unable to write matrix CSV file");
                return null;
            }
        }

        internal static string WriteMatrixToMTX(Map map, float[][] data)
        {
            Assert.AreEqual(map.Count, data.Length);
            var fileName = Path.GetTempFileName();
            try
            {
                var matrix = new Matrix(map);
                for (int i = 0; i < data.Length; i++)
                {
                    Array.Copy(data[i], 0, matrix.Data, i * data.Length, data[i].Length);
                }
                var save = new SaveMatrixAsMTX();
                using (var writeStream = (new XTMF2.RuntimeModules.OpenWriteStreamFromFile())
                    .Invoke(fileName))
                {
                    save.Invoke((matrix, writeStream));
                }
                return fileName;
            }
            catch
            {
                File.Delete(fileName);
                Assert.Fail("Unable to write matrix CSV file");
                return null;
            }
        }
    }
}
