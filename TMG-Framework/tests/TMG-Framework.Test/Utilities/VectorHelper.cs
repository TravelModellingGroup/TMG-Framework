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
    internal static class VectorHelper
    {
        internal static string WriteVectorCSV(Categories categories, float[] data)
        {
            if(data.Length != categories.Count)
            {
                Assert.Fail("data length needs to be the same size as the map!");
            }
            var fileName = Path.GetTempFileName();
            try
            {
                var vector = new Vector(categories);
                Array.Copy(data, vector.Data, vector.Data.Length);
                var save = new SaveVectorAsCSV()
                {
                    MapColumnName = Helper.CreateParameter("Zone"),
                    DataColumnName = Helper.CreateParameter("Data")
                };
                using (var writeStream = Helper.CreateWriteStreamFromFile(fileName))
                {
                    save.Invoke((vector, writeStream));
                }
                return fileName;
            }
            catch
            {
                File.Delete(fileName);
                Assert.Fail("Unable to write vector CSV file");
                return null;
            }
        }

        internal static bool Compare(Vector expected, Vector test, ref string error)
        {
            if (expected.Categories != test.Categories)
            {
                error = "The categories are not the same!";
                return false;
            }
            // compare the data
            var expectedData = expected.Data;
            var testData = test.Data;
            for (int i = 0; i < testData.Length; i++)
            {
                if (expectedData[i] != testData[i])
                {
                    error = $"Found different elements at position {i}: {expectedData[i]} != {testData[i]}!";
                    return false;
                }
            }
            return true;
        }
    }
}
