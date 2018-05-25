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
    public class TestSaveMatrixAsMTX
    {
        [TestMethod]
        public void SaveMatrixAsMTXAsMatrix()
        {
            var categories = MapHelper.LoadMap(MapHelper.WriteCSV(2000));
            var a = new Matrix(categories, categories);
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writeStream = new OpenWriteStreamFromFile().Invoke(fileName))
                {
                    new SaveMatrixAsMTX().Invoke((a, writeStream));
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
        public void SaveMatrixAsMTXAsMatrixAndLoad()
        {
            var categories = MapHelper.LoadMap(MapHelper.WriteCSV(2000));
            var a = new Matrix(categories, categories);
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writeStream = new OpenWriteStreamFromFile().Invoke(fileName))
                {
                    new SaveMatrixAsMTX().Invoke((a, writeStream));
                }
                // Now that the matrix has been saved attempt to load it back in.
                using (var readStream = new OpenReadStreamFromFile().Invoke(fileName))
                {
                    var readMatrix = new TMG.Loading.LoadMatrixFromMTX()
                    {
                        Categories = Helper.CreateParameter(categories)
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
