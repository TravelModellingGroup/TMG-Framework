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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TMG.Test.Utilities;

namespace TMG.Test.VectorHelper;

[TestClass]
public class TestFlagIfLessThanOrEqual
{
    [TestMethod]
    public void FlagLessThanOrEqualVectorScalar()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> data = stackalloc float[100];
        var value = 50f;        
        for (int i = 0; i < 100; i++)
        {
            data[i] = i;
        }

        TMG.Utilities.VectorHelper.FlagIfLessThanOrEqual(dest, value, data);

        for (int i = 0; i < 100; i++)
        {
            float expected = (value <= data[i]) ? 1.0f : 0f;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void FlagLessThanOrEqualVectorScalarWrongSize()
    {

        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> data = stackalloc float[99];
            var value = 50f;        
            for (int i = 0; i < 99; i++)
            {
                data[i] = i;
            }

            TMG.Utilities.VectorHelper.FlagIfLessThanOrEqual(dest, value, data);
        });
    }

    [TestMethod]
    public void FlagLessThanOrEqualVectorVector()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> data = stackalloc float[100];
        Span<float> value = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            data[i] = (i % 3 == 0) ? 100f : i;
            value[i] = (i % 5 == 0) ? 0f : i / 2f;
        }

        TMG.Utilities.VectorHelper.FlagIfLessThanOrEqual(dest, value, data);

        for (int i = 0; i < 100; i++)
        {
            float expected = (value[i] <= data[i]) ? 1.0f : 0f;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void FlagLessThanOrEqualVectorVectorWrongSize()
    {

        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> data = stackalloc float[100];
            Span<float> value = stackalloc float[99];
            for (int i = 0; i < 99; i++)
            {
                data[i] = i;
            }

            TMG.Utilities.VectorHelper.FlagIfLessThanOrEqual(dest, value, data);
        });
    }
}
