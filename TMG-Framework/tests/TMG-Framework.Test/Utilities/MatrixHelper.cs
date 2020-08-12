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
    static class MatrixHelper
    {
        internal static string WriteMatrixToCSVMatrix(Categories categories, float[][] data)
        {
            Assert.AreEqual(categories.Count, data.Length);
            var fileName = Path.GetTempFileName();
            try
            {
                var matrix = new Matrix(categories, categories);
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
                using (var writeStream = Helper.CreateWriteStreamFromFile(fileName))
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

        internal static string WriteMatrixToCSVThirdNormalized(Categories categories, float[][] data)
        {
            Assert.AreEqual(categories.Count, data.Length);
            var fileName = Path.GetTempFileName();
            try
            {
                var matrix = new Matrix(categories, categories);
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
                using (var writeStream = Helper.CreateWriteStreamFromFile(fileName))
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

        internal static bool Compare(Matrix expected, Matrix testCase, ref string error)
        {
            if(expected.ColumnCategories != testCase.ColumnCategories)
            {
                error = "The column categories are not the same!";
                return false;
            }
            if(expected.RowCategories != testCase.RowCategories)
            {
                error = "The row categories are not the same!";
                return false;
            }
            // compare the data
            var expectedData = expected.Data;
            var testData = testCase.Data;
            for (int i = 0; i < testData.Length; i++)
            {
                if(expectedData[i] != testData[i])
                {
                    error = $"Found different elements at position {i}: {expectedData[i]} != {testData[i]}!";
                    return false;
                }
            }
            return true;
        }

        internal static string WriteMatrixToMTX(Categories categories, float[][] data)
        {
            Assert.AreEqual(categories.Count, data.Length);
            var fileName = Path.GetTempFileName();
            try
            {
                var matrix = new Matrix(categories, categories);
                for (int i = 0; i < data.Length; i++)
                {
                    Array.Copy(data[i], 0, matrix.Data, i * data.Length, data[i].Length);
                }
                var save = new SaveMatrixAsMTX();
                using (var writeStream = Helper.CreateWriteStreamFromFile(fileName))
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
