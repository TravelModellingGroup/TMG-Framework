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

namespace TMG.Test.VectorHelper;

[TestClass]
public class TestPow
{
    [TestMethod]
    public void TestPowVV()
    {
        Span<float> x = stackalloc float[100];
        Span<float> y = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            x[i] = i + 1;
            y[i] = i * 0.1f;
        }

        TMG.Utilities.VectorHelper.Pow(dest, x, y);
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(MathF.Pow(x[i], y[i]), dest[i], 1e-6);
        }
    }

    [TestMethod]
    public void TestPowVS()
    {
        Span<float> x = stackalloc float[100];
        float y = 0.1f;
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            x[i] = i + 1;
        }

        TMG.Utilities.VectorHelper.Pow(dest, x, y);
        for (int i = 0; i < 100; i++)
        {
            var expected = MathF.Pow(x[i], y);
            Assert.AreEqual(expected, dest[i], 1e-6);
        }
    }

    [TestMethod]
    public void TestPowSV()
    {
        float x = 2.5f;
        Span<float> y = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            y[i] = i * 0.1f;
        }

        TMG.Utilities.VectorHelper.Pow(dest, x, y);
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(MathF.Pow(x, y[i]), dest[i], 1e-6);
        }
    }

    [TestMethod]
    public void TestNegPowVV()
    {
        Span<float> x = stackalloc float[100];
        Span<float> y = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            x[i] = i + 1;
            y[i] = i * -0.1f;
        }

        TMG.Utilities.VectorHelper.Pow(dest, x, y);
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(MathF.Pow(x[i], y[i]), dest[i], 1e-6);
        }
    }

    [TestMethod]
    public void TestNegPowVS()
    {
        Span<float> x = stackalloc float[100];
        float y = 0.1f;
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            x[i] = -i - 1;
        }

        TMG.Utilities.VectorHelper.Pow(dest, x, y);
        for (int i = 0; i < 100; i++)
        {
            var expected = MathF.Pow(x[i], y);
            Assert.AreEqual(expected, dest[i], 1e-6);
        }
    }

    [TestMethod]
    public void TestNegPowSV()
    {
        float x = 2.5f;
        Span<float> y = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            y[i] = i * -0.1f;
        }

        TMG.Utilities.VectorHelper.Pow(dest, x, y);
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(MathF.Pow(x, y[i]), dest[i], 1e-6);
        }
    }
}
