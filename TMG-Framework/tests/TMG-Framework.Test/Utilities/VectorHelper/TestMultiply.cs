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
public class TestMultiply
{
    [TestMethod]
    public void Multiply2Vectors()
    {
        Span<float> left = stackalloc float[100];
        Span<float> right = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
            right[i] = i * 2;
        }

        TMG.Utilities.VectorHelper.Multiply(dest, left, right);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] * right[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void Multiply3Vectors()
    {
        Span<float> left = stackalloc float[100];
        Span<float> right = stackalloc float[100];
        Span<float> third = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
            right[i] = i * 2;
            third[i] = i * 3;
        }

        TMG.Utilities.VectorHelper.Multiply(dest, left, right, third);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] * right[i] * third[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void Multiply4Vectors()
    {
        Span<float> left = stackalloc float[100];
        Span<float> right = stackalloc float[100];
        Span<float> third = stackalloc float[100];
        Span<float> fourth = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
            right[i] = i * 2;
            third[i] = i * 3;
            fourth[i] = i * 4;
        }

        TMG.Utilities.VectorHelper.Multiply(dest, left, right, third, fourth);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] * right[i] * third[i] * fourth[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void MultiplyVectorScalar()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> left = stackalloc float[100];
        var right = 5f;        
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
        }

        TMG.Utilities.VectorHelper.Multiply(dest, left, right);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] * right;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }        
    }

    [TestMethod]
    public void Multiply2VectorScalar()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> left = stackalloc float[100];
        Span<float> right = stackalloc float[100];        
        var third = 10f;        
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
            right[i] = i * 2;
        }

        TMG.Utilities.VectorHelper.Multiply(dest, left, right, third);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] * right[i] * third;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void Multiply3VectorScalar()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> left = stackalloc float[100];
        Span<float> right = stackalloc float[100];
        Span<float> third = stackalloc float[100];
        var fourth = 10f;
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
            right[i] = i * 2;
            third[i] = i * 3;
        }

        TMG.Utilities.VectorHelper.Multiply(dest, left, right, third, fourth);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] * right[i] * third[i] * fourth;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void Multiply2VectorsWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[99];
            Span<float> right = stackalloc float[99];
            TMG.Utilities.VectorHelper.Multiply(dest, left, right);
        });
    }

    [TestMethod]
    public void MultiplyVectorScalarWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[99];
            var right = 5f;        
            TMG.Utilities.VectorHelper.Multiply(dest, left, right);
        });
    }

    [TestMethod]
    public void Multiply3VectorsWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[100];
            Span<float> right = stackalloc float[99];
            Span<float> third = stackalloc float[100];
            TMG.Utilities.VectorHelper.Multiply(dest, left, right, third);
        });
    }

    [TestMethod]
    public void Multiply2VectorsScalarWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[100];
            Span<float> right = stackalloc float[99];
            float third = 5f;
            TMG.Utilities.VectorHelper.Multiply(dest, left, right, third);
        });
    }

    [TestMethod]
    public void Multiply4VectorsWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[100];
            Span<float> right = stackalloc float[100];
            Span<float> third = stackalloc float[99];
            Span<float> fourth = stackalloc float[100];
            TMG.Utilities.VectorHelper.Multiply(dest, left, right, third, fourth);
        });
    }

    [TestMethod]
    public void Multiply3VectorsScalarWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[100];
            Span<float> right = stackalloc float[100];
            Span<float> third = stackalloc float[99];
            float fourth = 5f;
            TMG.Utilities.VectorHelper.Multiply(dest, left, right, third, fourth);
        });
    }
}