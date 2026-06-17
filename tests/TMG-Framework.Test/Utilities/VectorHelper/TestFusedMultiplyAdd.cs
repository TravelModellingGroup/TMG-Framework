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
public class TestFusedMultiplyAdd
{
    [TestMethod]
    public void FusedMultiplyAddVVV()
    {
        Span<float> a = stackalloc float[100];
        Span<float> b = stackalloc float[100];
        Span<float> c = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            a[i] = i;
            b[i] = i * 2;
            c[i] = i * 3;
        }

        TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);

        for (int i = 0; i < 100; i++)
        {
            float expected = a[i] * b[i] + c[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

        [TestMethod]
    public void FusedMultiplyAddVSV()
    {
        Span<float> a = stackalloc float[100];
        float b = 5f;
        Span<float> c = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            a[i] = i;
            c[i] = i * 3;
        }

        TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);

        for (int i = 0; i < 100; i++)
        {
            float expected = a[i] * b + c[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    public void FusedMultiplyAddVSS()
    {
        Span<float> a = stackalloc float[100];
        float b = 5f;
        float c = 10f;
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            a[i] = i;
        }

        TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);

        for (int i = 0; i < 100; i++)
        {
            float expected = a[i] * b + c;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void FusedMultiplyAddSSV()
    {
        float a = 2f;
        float b = 5f;
        Span<float> c = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            c[i] = i;
        }

        TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);

        for (int i = 0; i < 100; i++)
        {
            float expected = a * b + c[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void FusedMultiplyAddVVVBadSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> a = stackalloc float[100];
            Span<float> b = stackalloc float[99];
            Span<float> c = stackalloc float[100];
            Span<float> dest = stackalloc float[100];
            TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);
        });
    }

    [TestMethod]
    public void FusedMultiplyAddVSVBadSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> a = stackalloc float[100];
            float b = 5f;
            Span<float> c = stackalloc float[99];
            Span<float> dest = stackalloc float[100];
            TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);
        });
    }

    public void FusedMultiplyAddVSSBadSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> a = stackalloc float[99];
            float b = 5f;
            float c = 10f;
            Span<float> dest = stackalloc float[100];
            TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);
        });
    }

    [TestMethod]
    public void FusedMultiplyAddSSVBadSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            float a = 2f;
            float b = 5f;
            Span<float> c = stackalloc float[100];
            Span<float> dest = stackalloc float[99];
            TMG.Utilities.VectorHelper.FusedMultiplyAdd(dest, a, b, c);
        });
    }
}