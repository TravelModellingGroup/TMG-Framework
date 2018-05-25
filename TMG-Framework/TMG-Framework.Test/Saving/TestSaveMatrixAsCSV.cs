using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMG.Saving;
using TMG.Test.Utilities;
using XTMF2.RuntimeModules;

namespace TMG.Test.Saving
{
    [TestClass]
    public class TestSaveMatrixAsCSV
    {
        [TestMethod]
        public void SaveMatrixAsCSVAsMatrix()
        {
            SaveMatrixAsCSV module = new SaveMatrixAsCSV()
            {
                FirstIndexHeader = Helper.CreateParameter("First"),
                SecondIndexHeader = Helper.CreateParameter("Second"),
                DataIndexHeader = Helper.CreateParameter("Data"),
                ThirdNormalized = Helper.CreateParameter(false)
            };
            var categories = MapHelper.LoadMap(MapHelper.WriteCSV(2000));
            var a = new Matrix(categories, categories);
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writeStream = new OpenWriteStreamFromFile().Invoke(fileName))
                {
                    module.Invoke((a, writeStream));
                }
            }
            finally
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }

        [TestMethod]
        public void SaveMatrixAsCSVAsMatrixAndLoad()
        {
            SaveMatrixAsCSV module = new SaveMatrixAsCSV()
            {
                FirstIndexHeader = Helper.CreateParameter("First"),
                SecondIndexHeader = Helper.CreateParameter("Second"),
                DataIndexHeader = Helper.CreateParameter("Data"),
                ThirdNormalized = Helper.CreateParameter(false)
            };
            var categories = MapHelper.LoadMap(MapHelper.WriteCSV(2000));
            var a = new Matrix(categories, categories);
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writeStream = new OpenWriteStreamFromFile().Invoke(fileName))
                {
                    module.Invoke((a, writeStream));
                }
                // Now that the matrix has been saved attempt to load it back in.
                using (var readStream = new OpenReadStreamFromFile().Invoke(fileName))
                {
                    var readMatrix = new TMG.Loading.LoadMatrixFromCSVMatrix()
                    {
                        RowCategories = Helper.CreateParameter(categories),
                        ColumnCategories = Helper.CreateParameter(categories)
                    }.Invoke(readStream);
                    string error = null;
                    Assert.IsTrue(MatrixHelper.Compare(a, readMatrix, ref error), error);
                }
            }
            finally
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }

        [TestMethod]
        public void SaveMatrixAsCSVAsThirdNormalized()
        {
            SaveMatrixAsCSV module = new SaveMatrixAsCSV()
            {
                FirstIndexHeader = Helper.CreateParameter("First"),
                SecondIndexHeader = Helper.CreateParameter("Second"),
                DataIndexHeader = Helper.CreateParameter("Data"),
                ThirdNormalized = Helper.CreateParameter(true)
            };
            var categories = MapHelper.LoadMap(MapHelper.WriteCSV(2000));
            var a = new Matrix(categories, categories);
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writeStream = new OpenWriteStreamFromFile().Invoke(fileName))
                {
                    module.Invoke((a, writeStream));
                }
            }
            finally
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }

        [TestMethod]
        public void SaveMatrixAsCSVAsThirdNormalizedAndLoad()
        {
            SaveMatrixAsCSV module = new SaveMatrixAsCSV()
            {
                FirstIndexHeader = Helper.CreateParameter("First"),
                SecondIndexHeader = Helper.CreateParameter("Second"),
                DataIndexHeader = Helper.CreateParameter("Data"),
                ThirdNormalized = Helper.CreateParameter(true)
            };
            var categories = MapHelper.LoadMap(MapHelper.WriteCSV(2000));
            var a = new Matrix(categories, categories);
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writeStream = new OpenWriteStreamFromFile().Invoke(fileName))
                {
                    module.Invoke((a, writeStream));
                }
                // Now that the matrix has been saved attempt to load it back in.
                using (var readStream = new OpenReadStreamFromFile().Invoke(fileName))
                {
                    var readMatrix = new TMG.Loading.LoadMatrixFromCSVThirdNormalized()
                    {
                        RowCategories = Helper.CreateParameter(categories),
                        ColumnCategories = Helper.CreateParameter(categories),
                        OriginColumn = Helper.CreateParameter(0),
                        DestinationColumn = Helper.CreateParameter(1),
                        DataColumn = Helper.CreateParameter(2),
                    }.Invoke(readStream);
                    string error = null;
                    Assert.IsTrue(MatrixHelper.Compare(a, readMatrix, ref error), error);
                }
            }
            finally
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }
    }
}
