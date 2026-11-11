/*
    Copyright 2026 University of Toronto

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
using System.IO.Compression;
using TMG.Loading;
using XTMF2;

using Helper = TMG.Test.Utilities.Helper;

namespace TMG.Test.Loading;

[TestClass]
public class TestLoadCategoriesFromNWP
{
    [TestMethod]
    public void LoadsCentroidNodeIdsAsSortedCategories()
    {
        var pipe = CreateNwpPipe("BASE.211", "t nodes\nheader\na* 30 1.0 2.0\na 99 3.0 4.0\na* 10 5.0 6.0\na* 50 7.0 8.0\nt links\n");
        try
        {
            var module = new LoadCategoriesFromNWP
            {
                Name = "Module",
                Input = Helper.CreateParameter(pipe.GetReadStream(null!))
            };
            var categories = module.Invoke();
            CollectionAssert.AreEqual(new[] { 10, 30, 50 }, categories.ToArray());
        }
        finally
        {
            pipe.DisposeMemoryStream();
        }
    }

    [TestMethod]
    public void ThrowsWhenBaseFileIsMissing()
    {
        AssertFails("other.txt", "t nodes\nheader\na* 1 0 0\n");
    }

    [TestMethod]
    public void ThrowsWhenNodesSectionIsMissing()
    {
        AssertFails("base.211", "header\na* 1 0 0\n");
    }

    [TestMethod]
    public void ThrowsWhenNoCentroidNodesArePresent()
    {
        AssertFails("base.211", "t nodes\nheader\na 1 0 0\nt links\n");
    }

    [TestMethod]
    public void ThrowsWhenCentroidNodeIdIsInvalid()
    {
        AssertFails("base.211", "t nodes\nheader\na* invalid 0 0\nt links\n");
    }

    private static void AssertFails(string entryName, string baseFileContent)
    {
        Helper.ThrowsException<XTMFRuntimeException>(() =>
        {
            var pipe = CreateNwpPipe(entryName, baseFileContent);
            try
            {
                var module = new LoadCategoriesFromNWP
                {
                    Name = "Module",
                    Input = Helper.CreateParameter(pipe.GetReadStream(null!))
                };
                module.Invoke();
            }
            finally
            {
                pipe.DisposeMemoryStream();
            }
        });
    }

    private static MemoryPipe CreateNwpPipe(string entryName, string baseFileContent)
    {
        var pipe = new MemoryPipe();
        using (var archive = new ZipArchive(pipe.GetWriteStream(null!), ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry(entryName);
            using var writer = new StreamWriter(entry.Open());
            writer.Write(baseFileContent);
        }
        return pipe;
    }
}
