/*
    Copyright 2018 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF.

    XTMF is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.IO;
using TMG.Utilities;

namespace XTMF.Testing.TMG.Data;

[TestClass]
public class TestCSVReader
{
    private readonly string[] TestCSVFileNames = new[] { "CSVTest1.csv", "CSVTest2.csv", "CSVTest3.csv", "CSVTest4.csv", "CSVTest5.csv", "CSVTest6.csv", "CSVTest7.csv", "CSVTest8.csv" };

    [TestInitialize]
    public void CreateTestEnvironment()
    {
        if (!IsEnvironmentLoaded())
        {
            using (StreamWriter writer = new(TestCSVFileNames[0]))
            {
                writer.WriteLine("A,B,C,D,E");
                writer.WriteLine("1,2,3,4,5");
                writer.WriteLine("3,1,4,5,2");
                writer.WriteLine("1.23,4.56,7.89,10.1112,0.1314");
            }
            using (StreamWriter writer = new(TestCSVFileNames[1]))
            {
                writer.WriteLine("\"A\",\"B\",\"C\",\"D\",\"E\"");
                writer.WriteLine("\"1\",\"2\",3,\"4\",5");
                writer.WriteLine("3,1,\"4\",5,2");
                writer.WriteLine("1.23,\"4.56\",7.89,10.1112,0.1314");
            }
            using (StreamWriter writer = new(TestCSVFileNames[2]))
            {
                writer.WriteLine("A,B,C,D,E");
                writer.WriteLine("1,2,3,4,5");
                writer.WriteLine("3,1,4,5,2");
                writer.WriteLine("1.23,4.56,7.89,10.1112,0.1314");
            }
            using (StreamWriter writer = new(TestCSVFileNames[3]))
            {
                writer.WriteLine("A,B,C,D,E");
                writer.WriteLine("1,2,3,4,5");
                writer.WriteLine("3,1,4,5,2");
                writer.Write("1.23,4.56,7.89,10.1112,0.1314");
            }
            using (StreamWriter writer = new(TestCSVFileNames[4]))
            {
                writer.WriteLine("A,B,C,D,E");
                writer.WriteLine("\"abc\"\"1\",2,3,4,5");
            }

            // The smae as TestCSVFileNames[4] but forcing the line breaks to be in the Windows format.
            using (StreamWriter writer = new(TestCSVFileNames[5]))
            {
                writer.Write("A,B,C,D,E\r\n");
                writer.Write("\"abc\"\"1\",2,3,4,5\r\n");
            }

            // The smae as TestCSVFileNames[4] but forcing the line breaks to be in the Linux format.
            using (StreamWriter writer = new(TestCSVFileNames[6]))
            {
                writer.Write("A,B,C,D,E\n");
                writer.Write("\"abc\"\"1\",2,3,4,5\n");
            }

            // The smae as TestCSVFileNames[4] but forcing the line breaks to be in the Mac format.
            using (StreamWriter writer = new(TestCSVFileNames[7]))
            {
                writer.Write("A,B,C,D,E\r");
                writer.Write("\"abc\"\"1\",2,3,4,5\r");
            }
        }
    }

    public bool IsEnvironmentLoaded()
    {
        for (int i = 0; i < TestCSVFileNames.Length; i++)
        {
            if (!File.Exists(TestCSVFileNames[i])) return false;
        }
        return true;
    }

    [TestMethod]
    public void TestCSVHeaders()
    {
        using CsvReader reader = new(TestCSVFileNames[0]);
        //"A,B,C,D,E"
        var headers = reader.Headers;
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(new String((char)('A' + i), 1), headers[i]);
        }
    }

    [TestMethod]
    public void TestQuotes()
    {
        using CsvReader reader = new(TestCSVFileNames[1]);
        //"A,B,C,D,E"
        var headers = reader.Headers;
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(new String((char)('A' + i), 1), headers[i]);
        }
        //"\"1\",\"2\",3,\"4\",5"
        reader.LoadLine();
        for (int i = 0; i < 5; i++)
        {
            reader.Get(out int n, i);
            Assert.AreEqual(i + 1, n);
        }
    }

    [TestMethod]
    public void TestDoubleQuotes()
    {
        using CsvReader reader = new(TestCSVFileNames[4]);
        // first line
        reader.LoadLine();
        //"A,B,C,D,E"
        var headers = reader.Headers;
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(new String((char)('A' + i), 1), headers[i]);
        }
        //writer.WriteLine("\"abc\"\"1\",2,3,4,5");
        reader.LoadLine();
        reader.Get(out string firstItem, 0);
        Assert.AreEqual("abc\"1", firstItem);
    }

    [TestMethod]
    public void TestLoadLineBool()
    {
        using CsvReader reader = new(TestCSVFileNames[1]);
        Assert.AreEqual(1, reader.LineNumber);
        while (reader.LoadLine(out int columns))
        {
            if ((columns == 0) && (reader.LineNumber != 5))
            {
                Assert.Fail("There was a blank line besides at the end of the file!");
            }
            else if (columns > 0)
            {
                Assert.AreEqual(5, columns);
            }
        }
        Assert.AreEqual(5, reader.LineNumber);
    }

    [TestMethod]
    public void TestNoEnterLastLine()
    {
        using CsvReader reader = new(TestCSVFileNames[3]);
        Assert.AreEqual(1, reader.LineNumber);
        while (reader.LoadLine(out int columns))
        {
            if ((columns == 0) && (reader.LineNumber != 4))
            {
                Assert.Fail("There was a blank line besides at the end of the file!");
            }
            else if (columns > 0)
            {
                Assert.AreEqual(5, columns);
            }
        }
        Assert.AreEqual(4, reader.LineNumber);
    }

    [TestMethod]
    public void TestLineReadValue()
    {
        using CsvReader reader = new(TestCSVFileNames[2]);
        float lastColumnValue = float.NaN;
        while (reader.LoadLine(out int columns))
        {
            if (columns > 0)
            {
                reader.Get(out lastColumnValue, 4);
            }
        }
        Assert.AreEqual(0.1314f, lastColumnValue);
    }

    [TestMethod]
    public void TestNoEnterLastLineReadValue()
    {
        using CsvReader reader = new(TestCSVFileNames[3]);
        float lastColumnValue = float.NaN;
        while (reader.LoadLine(out int columns))
        {
            if ((columns == 0) && (reader.LineNumber != 4))
            {
                Assert.Fail("There was a blank line besides at the end of the file!");
            }
            else if (columns > 0)
            {
                Assert.AreEqual(5, columns);
            }
            reader.Get(out lastColumnValue, 4);
        }
        Assert.AreEqual(4, reader.LineNumber);
        Assert.AreEqual(0.1314f, lastColumnValue);
    }

    [TestMethod]
    public void TestReadingSpan()
    {
        using CsvReader reader = new(TestCSVFileNames[4]);
        //"\"abc\"\"1\",2,3,4,5"
        Assert.AreEqual(5, reader.Headers.Length);
        reader.LoadLine(out int columns);
        Assert.AreEqual(5, columns);
        reader.Get(out ReadOnlySpan<char> span, 0);
        Assert.AreEqual("abc\"1", new string(span));
    }

    [TestMethod]
    public void TestReadingUnixLineEndings()
    {
        using CsvReader reader = new(TestCSVFileNames[6]);
        //"\"abc\"\"1\",2,3,4,5"
        Assert.AreEqual(5, reader.Headers.Length);
        reader.LoadLine(out int columns);
        Assert.AreEqual(5, columns);
        reader.Get(out ReadOnlySpan<char> span, 0);
        Assert.AreEqual("abc\"1", new string(span));
        Assert.IsFalse(reader.LoadLine(out columns), $"There is an extra line at the end of the file with {columns} columns.");
    }
    [TestMethod]
    public void TestReadingWindowsLineEndings()
    {
        using CsvReader reader = new(TestCSVFileNames[5]);
        //"\"abc\"\"1\",2,3,4,5"
        Assert.AreEqual(5, reader.Headers.Length);
        reader.LoadLine(out int columns);
        Assert.AreEqual(5, columns);
        reader.Get(out ReadOnlySpan<char> span, 0);
        Assert.AreEqual("abc\"1", new string(span));
        Assert.IsFalse(reader.LoadLine(out columns), $"There is an extra line at the end of the file with {columns} columns.");
    }

    [TestMethod]
    public void TestReadingLinuxLineEndings()
    {
        using CsvReader reader = new(TestCSVFileNames[6]);
        //"\"abc\"\"1\",2,3,4,5"
        Assert.AreEqual(5, reader.Headers.Length);
        reader.LoadLine(out int columns);
        Assert.AreEqual(5, columns);
        reader.Get(out ReadOnlySpan<char> span, 0);
        Assert.AreEqual("abc\"1", new string(span));
        Assert.IsFalse(reader.LoadLine(out columns), $"There is an extra line at the end of the file with {columns} columns.");
    }

    [TestMethod]
    public void TestReadingMacLineEndings()
    {
        using CsvReader reader = new(TestCSVFileNames[7]);
        //"\"abc\"\"1\",2,3,4,5"
        Assert.AreEqual(5, reader.Headers.Length);
        reader.LoadLine(out int columns);
        Assert.AreEqual(5, columns);
        reader.Get(out ReadOnlySpan<char> span, 0);
        Assert.AreEqual("abc\"1", new string(span));
        Assert.IsFalse(reader.LoadLine(out columns), $"There is an extra line at the end of the file with {columns} columns.");
    }
}
