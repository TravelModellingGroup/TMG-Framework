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

using TMG.Test.Utilities;

namespace TMG.Test.VectorHelper;

[TestClass]
public class TestFlagIfNotEquals
{
    [TestMethod]
    public void FlagIfNotEqualsVectorScalar()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> data = stackalloc float[100];
        var value = 2f;        
        for (int i = 0; i < 100; i++)
        {
            data[i] = (i % 10 == 0) ? 0f : i;
        }

        TMG.Utilities.VectorHelper.FlagIfNotEquals(dest, value, data);

        for (int i = 0; i < 100; i++)
        {
            float expected = (data[i] != value) ? 1.0f : 0f;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void FlagIfNotEqualsVectorScalarWrongSize()
    {

        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> data = stackalloc float[99];
            var value = 2f;        
            for (int i = 0; i < 99; i++)
            {
                data[i] = (i % 10 == 0) ? 0f : i;
            }

            TMG.Utilities.VectorHelper.FlagIfNotEquals(dest, value, data);
        });
    }

    [TestMethod]
    public void FlagIfNotEqualsVectorVector()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> data = stackalloc float[100];
        Span<float> value = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            data[i] = (i % 3 == 0) ? 0f : i;
            value[i] = (i % 5 == 0) ? 0f : i;
        }

        TMG.Utilities.VectorHelper.FlagIfNotEquals(dest, value, data);

        for (int i = 0; i < 100; i++)
        {
            float expected = (data[i] != value[i]) ? 1.0f : 0f;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void FlagIfNotEqualsVectorVectorWrongSize()
    {

        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> data = stackalloc float[100];
            Span<float> value = stackalloc float[99];
            for (int i = 0; i < 99; i++)
            {
                data[i] = (i % 10 == 0) ? 0f : i;
            }

            TMG.Utilities.VectorHelper.FlagIfNotEquals(dest, value, data);
        });
    }
}