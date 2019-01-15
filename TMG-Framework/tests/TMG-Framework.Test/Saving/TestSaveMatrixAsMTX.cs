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
