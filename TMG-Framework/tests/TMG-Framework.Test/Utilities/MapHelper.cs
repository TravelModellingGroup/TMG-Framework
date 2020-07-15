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
using XTMF2.RuntimeModules;

namespace TMG.Test.Utilities
{
    static class MapHelper
    {
        internal static string WriteCSV(int elements)
        {
            var tempName = Path.GetTempFileName();
            try
            {
                using (StreamWriter writer = File.CreateText(tempName))
                {
                    writer.WriteLine("Zone");
                    for (int i = 0; i < elements; i++)
                    {
                        writer.WriteLine(i + 1);
                    }
                }
            }
            catch
            {
                File.Delete(tempName);
                Assert.Fail("Unable to create map file!");
            }
            return tempName;
        }

        internal static string WriteBackwardsCSV(int elements)
        {
            var tempName = Path.GetTempFileName();
            try
            {
                using (StreamWriter writer = File.CreateText(tempName))
                {
                    writer.WriteLine("Zone");
                    for (int i = elements - 1; i >= 0; i--)
                    {
                        writer.WriteLine(i + 1);
                    }
                }
            }
            catch
            {
                File.Delete(tempName);
                Assert.Fail("Unable to create map file!");
            }
            return tempName;
        }

        internal static Categories LoadMap(string fileName)
        {
            try
            {
                var mapLoader = new LoadCategoriesFromCSV();
                using (var reader = Helper.CreateReadStreamFromFile(fileName))
                {
                    return mapLoader.Invoke(reader);
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Generate categories that are 1 indexed.
        /// </summary>
        /// <param name="numberOfElements">The number of elements to have.</param>
        /// <returns>A new Categories with sparse space in [1..numberOfElements]</returns>
        internal static Categories LoadMap(int numberOfElements)
        {
            string error = null;
            return Categories.CreateCategories(Enumerable.Range(1, numberOfElements).ToList(), ref error);
        }
    }
}
