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

namespace TMG.Test.Utilities
{
    internal static class VectorHelper
    {
        internal static string WriteVectorCSV(Map map, float[] data)
        {
            if(data.Length != map.Count)
            {
                Assert.Fail("data length needs to be the same size as the map!");
            }
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writer = new StreamWriter(File.OpenWrite(fileName)))
                {
                    writer.WriteLine("Zone,Data");
                    for (int i = 0; i < map.Count; i++)
                    {
                        writer.Write(map.GetSparseIndex(i));
                        writer.Write(',');
                        writer.WriteLine(data[i]);
                    }
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
    }
}
