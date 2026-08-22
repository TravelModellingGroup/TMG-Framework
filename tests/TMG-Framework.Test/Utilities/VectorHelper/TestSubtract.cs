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
public class TestSubtract
{
    [TestMethod]
    public void Subtract2Vectors()
    {
        Span<float> left = stackalloc float[100];
        Span<float> right = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
            right[i] = i * 2;
        }

        TMG.Utilities.VectorHelper.Subtract(dest, left, right);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] - right[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void SubtractVectorScalar()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> left = stackalloc float[100];
        var right = 5f;        
        for (int i = 0; i < 100; i++)
        {
            left[i] = i;
        }

        TMG.Utilities.VectorHelper.Subtract(dest, left, right);

        for (int i = 0; i < 100; i++)
        {
            float expected = left[i] - right;
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void SubtractScalarVector()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> right = stackalloc float[100];
        var left = 5f;        
        for (int i = 0; i < 100; i++)
        {
            right[i] = i;
        }

        TMG.Utilities.VectorHelper.Subtract(dest, left, right);

        for (int i = 0; i < 100; i++)
        {
            float expected = left - right[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void Subtract2VectorsWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[99];
            Span<float> right = stackalloc float[99];
            TMG.Utilities.VectorHelper.Subtract(dest, left, right);
        });
    }

    [TestMethod]
    public void SubtractVectorScalarWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> left = stackalloc float[99];
            var right = 5f;
            TMG.Utilities.VectorHelper.Subtract(dest, left, right);
        });
    }

    [TestMethod]
    public void SubtractScalarVectorWrongSizes()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> right = stackalloc float[99];
            var left = 5f;
            TMG.Utilities.VectorHelper.Subtract(dest, left, right);
        });
    }

}
