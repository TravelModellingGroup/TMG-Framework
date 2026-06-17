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
public class TestExp
{
    [TestMethod]
    public void TestExpV()
    {
        Span<float> src = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            src[i] = i * 0.1f;
        }

        TMG.Utilities.VectorHelper.Exp(dest, src);
        for (int i = 0; i < 100; i++)
        {
            // For large values the difference between the percisions increases.
            Assert.AreEqual(MathF.Exp(src[i]), dest[i], 1e-10);
        }
    }

    [TestMethod]
    public void TestNegExpV()
    {
        Span<float> src = stackalloc float[100];
        Span<float> dest = stackalloc float[100];
        for (int i = 0; i < 100; i++)
        {
            src[i] = i * -0.1f;
        }

        TMG.Utilities.VectorHelper.Exp(dest, src);
        for (int i = 0; i < 100; i++)
        {
            // For large values the difference between the percisions increases.
            Assert.AreEqual(MathF.Exp(src[i]), dest[i], 1e-10);
        }
    }
}