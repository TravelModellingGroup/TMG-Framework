/*
    Copyright 2021 University of Toronto

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

using System.IO;
using XTMF2;
using TMG.Loading;

using Helper = TMG.Test.Utilities.Helper;


namespace TMG.Test.Loading;

[TestClass]
public class TestLoadCategoryMapFromThirdNormalizedCSV
{
    /// <summary>
    /// Test if we can execute the LoadCategoryMapFromThirdNormalizedCSV where it dynamically creates the destination category
    /// </summary>
    [TestMethod]
    public void LoadMapWithoutDestinations()
    {
        string? error = null;
        if (!Categories.CreateCategories(new List<int> { 1, 2, 3, 4 }, out var baseCategories, ref error))
        {
            Assert.Fail(error);
        }
        var pipe = CreateMapPipe();
        try
        {
            var module = new LoadCategoryMapFromThirdNormalizedCSV()
            {
                Name = "Module",
                BaseColumn = Helper.CreateParameter(0),
                DestinationColumn = Helper.CreateParameter(1),
                BaseCategories = Helper.CreateParameter(baseCategories),
                DestinationCategories = null,
                CSVStream = Helper.CreateParameter(pipe.GetReadStream(null!))
            };
            var map = module.Invoke();
            Assert.IsNotNull(map);
            var index = map.CreateIndex();
            Assert.AreEqual(0, (int)index[0]);
            Assert.AreEqual(1, (int)index[1]);
            Assert.AreEqual(0, (int)index[2]);
            Assert.AreEqual(1, (int)index[3]);
        }
        finally
        {
            pipe.DisposeMemoryStream();
        }
    }

    /// <summary>
    /// Test if we can execute the LoadCategoryMapFromThirdNormalizedCSV where we specify the destination category
    /// </summary>
    [TestMethod]
    public void LoadMapWithDestinations()
    {
        string? error = null;
        if (!Categories.CreateCategories(new List<int> { 1, 2, 3, 4 }, out var baseCategories, ref error))
        {
            Assert.Fail(error);
        }
        if (!Categories.CreateCategories(new List<int> { 1, 2 }, out var destinationCategories, ref error))
        {
            Assert.Fail(error);
        }
        var pipe = CreateMapPipe();
        try
        {
            var module = new LoadCategoryMapFromThirdNormalizedCSV()
            {
                Name = "Module",
                BaseColumn = Helper.CreateParameter(0),
                DestinationColumn = Helper.CreateParameter(1),
                BaseCategories = Helper.CreateParameter(baseCategories),
                DestinationCategories = Helper.CreateParameter(destinationCategories),
                CSVStream = Helper.CreateParameter(pipe.GetReadStream(null!))
            };
            var map = module.Invoke();
            Assert.IsNotNull(map);
            var index = map.CreateIndex();
            Assert.AreEqual(0, (int)index[0]);
            Assert.AreEqual(1, (int)index[1]);
            Assert.AreEqual(0, (int)index[2]);
            Assert.AreEqual(1, (int)index[3]);
        }
        finally
        {
            pipe.DisposeMemoryStream();
        }
    }

    /// <summary>
    /// Make sure that if there is an invalid destination specified that we throw an exception
    /// </summary>
    [TestMethod]
    public void LoadMapWithBadDestinations()
    {
        Helper.ThrowsException<XTMFRuntimeException>(() =>
        {
            string? error = null;
            if (!Categories.CreateCategories(new List<int> { 1, 2, 3, 4 }, out var baseCategories, ref error))
            {
                Assert.Fail(error);
            }
            if (!Categories.CreateCategories(new List<int> { 1 }, out var destinationCategories, ref error))
            {
                Assert.Fail(error);
            }
            var pipe = CreateMapPipe();
            try
            {
                var module = new LoadCategoryMapFromThirdNormalizedCSV()
                {
                    Name = "Module",
                    BaseColumn = Helper.CreateParameter(0),
                    DestinationColumn = Helper.CreateParameter(1),
                    BaseCategories = Helper.CreateParameter(baseCategories),
                    DestinationCategories = Helper.CreateParameter(destinationCategories),
                    CSVStream = Helper.CreateParameter(pipe.GetReadStream(null!))
                };
                // We expect this to throw
                var _ = module.Invoke();
            }
            finally
            {
                pipe.DisposeMemoryStream();
            }
        });
    }

    /// <summary>
    /// Make sure that if there is an invalid base specified that we throw an exception
    /// </summary>
    [TestMethod]
    public void LoadMapWithBadBase()
    {
        Helper.ThrowsException<XTMFRuntimeException>(() =>
        {
            string? error = null;
            if (!Categories.CreateCategories(new List<int> { 1, 2, 4 }, out var baseCategories, ref error))
            {
                Assert.Fail(error);
            }
            if (!Categories.CreateCategories(new List<int> { 1, 2 }, out var destinationCategories, ref error))
            {
                Assert.Fail(error);
            }
            var pipe = CreateMapPipe();
            try
            {
                var module = new LoadCategoryMapFromThirdNormalizedCSV()
                {
                    Name = "Module",
                    BaseColumn = Helper.CreateParameter(0),
                    DestinationColumn = Helper.CreateParameter(1),
                    BaseCategories = Helper.CreateParameter(baseCategories),
                    DestinationCategories = Helper.CreateParameter(destinationCategories),
                    CSVStream = Helper.CreateParameter(pipe.GetReadStream(null!))
                };
                // We expect this to throw
                var _ = module.Invoke();
            }
            finally
            {
                pipe.DisposeMemoryStream();
            }
        });
    }

    /// <summary>
    /// Creates a memory pipe to read the map from.
    /// </summary>
    /// <returns></returns>
    private static MemoryPipe CreateMapPipe()
    {
        var pipe = new MemoryPipe();
        var writeStream = pipe.GetWriteStream(null!);
        var writer = new StreamWriter(writeStream);
        writer.WriteLine("Zone,PD");
        writer.WriteLine("1,1");
        writer.WriteLine("2,2");
        writer.WriteLine("3,1");
        writer.WriteLine("4,2");
        writer.Flush();
        return pipe;
    }
}
