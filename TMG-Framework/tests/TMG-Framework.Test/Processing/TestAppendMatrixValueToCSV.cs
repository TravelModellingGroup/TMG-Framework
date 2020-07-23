/*
    Copyright 2020 University of Toronto

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
using TMG.Processing;
using TMG.Test.Utilities;
namespace TMG.Test.Processing
{
    [TestClass]
    public class TestAppendMatrixValueToCSV
    {
        [TestMethod]
        public void AppendMatrixValueToCSV()
        {
            string csvFileName = Path.GetTempFileName();
            string processedCsvFileName = Path.GetTempFileName();
            try
            {
                var zoneSystem = MapHelper.LoadMap(64);
                var matrix = new Matrix(zoneSystem, zoneSystem);
                // Setup matrix
                matrix.GetFromSparseIndexes(5, 2) = 10;
                matrix.GetFromSparseIndexes(2, 5) = 11;

                matrix.GetFromSparseIndexes(10, 4) = 12;
                matrix.GetFromSparseIndexes(4, 10) = 13;
                // Setup CSV

                using (var writer = new StreamWriter(csvFileName))
                {
                    writer.WriteLine("O,D,OtherData");
                    writer.WriteLine("5,2,ABC");
                    writer.WriteLine("2,5,DEF");
                    writer.WriteLine("10,4,GHI");
                    writer.WriteLine("4,10,GHI");
                }

                using (var readStream = Helper.CreateReadStreamFromFile(csvFileName))
                using (var writeStream = Helper.CreateWriteStreamFromFile(processedCsvFileName))
                {
                    // Run append
                    AppendMatrixValueToCSV appendModule = new AppendMatrixValueToCSV()
                    {
                        Name = "Append",
                        Matrix = Helper.CreateParameter(matrix, "Matrix"),
                        RowIndex = Helper.CreateParameter(0),
                        ColumnIndex = Helper.CreateParameter(1),
                        ColumnName = Helper.CreateParameter("MatrixValue"),
                        InputStream = Helper.CreateParameter(readStream, "Reader"),
                        OutputStream = Helper.CreateParameter(writeStream, "Writer")
                    };
                    appendModule.Invoke();
                }

                // Test Results
                using (var reader = new StreamReader(processedCsvFileName))
                {
                    Assert.AreEqual("O,D,OtherData,MatrixValue", reader.ReadLine());
                    Assert.AreEqual("5,2,ABC,10", reader.ReadLine());
                    Assert.AreEqual("2,5,DEF,11", reader.ReadLine());
                    Assert.AreEqual("10,4,GHI,12", reader.ReadLine());
                    Assert.AreEqual("4,10,GHI,13", reader.ReadLine());
                    Assert.AreEqual(null, reader.ReadLine());
                }
            }
            finally
            {
                if (File.Exists(csvFileName))
                {
                    File.Delete(csvFileName);
                }
                if (File.Exists(processedCsvFileName))
                {
                    File.Delete(processedCsvFileName);
                }
            }
        }
    }
}
