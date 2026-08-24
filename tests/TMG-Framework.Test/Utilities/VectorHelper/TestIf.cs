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
public class TestIf
{
    [TestMethod]
    public void TestIfTrue()
    {
        Span<float> dest = stackalloc float[100];
        Span<float> condition = stackalloc float[100];
        Span<float> ifTrue = stackalloc float[100];
        Span<float> ifFalse = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            condition[i] = i % 2 == 0 ? 1f : 0f;
            ifTrue[i] = 10f;
            ifFalse[i] = -10f;
        }

        TMG.Utilities.VectorHelper.If(dest, condition, ifTrue, ifFalse);

        for (int i = 0; i < 100; i++)
        {
            float expected = i % 2 == 0 ? ifTrue[i] : ifFalse[i];
            // Avoid calling out to AreEqual to help performance unless it failed.
            if (dest[i] != expected)
            {
                Assert.AreEqual(expected, dest[i]);
            }
        }
    }

    [TestMethod]
    public void TestDifferentSizeFails()
    {
        Helper.ThrowsException<ArgumentException>(() =>
        {
            Span<float> dest = stackalloc float[100];
            Span<float> condition = stackalloc float[90];
            Span<float> ifTrue = stackalloc float[100];
            Span<float> ifFalse = stackalloc float[100];
            TMG.Utilities.VectorHelper.If(dest, condition, ifTrue, ifFalse);
        });
    }
}