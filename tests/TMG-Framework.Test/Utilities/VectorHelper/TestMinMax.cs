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

namespace TMG.Test.VectorHelper;

[TestClass]
public class TestMinMax
{
    [TestMethod]
    public void MinMaxOperationsHandleVectorAndScalarInputs()
    {
        Span<float> left = stackalloc float[37];
        Span<float> right = stackalloc float[37];
        Span<float> output = stackalloc float[37];
        for (int i = 0; i < left.Length; i++)
        {
            left[i] = i - 18;
            right[i] = 9 - i;
        }

        TMG.Utilities.VectorHelper.Max(output, left, right);
        AssertMinMax(output, left, right, true);

        TMG.Utilities.VectorHelper.Min(output, left, right);
        AssertMinMax(output, left, right, false);

        TMG.Utilities.VectorHelper.Max(output, left, 2.5f);
        AssertMinMax(output, left, 2.5f, true);

        TMG.Utilities.VectorHelper.Min(output, 2.5f, right);
        AssertMinMax(output, right, 2.5f, false);
    }

    [TestMethod]
    public void MinMaxReductionsReturnTheExtrema()
    {
        Span<float> values = stackalloc float[37];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = i - 18;
        }

        Assert.AreEqual(18f, TMG.Utilities.VectorHelper.Max(values));
        Assert.AreEqual(-18f, TMG.Utilities.VectorHelper.Min(values));
    }

    private static void AssertMinMax(ReadOnlySpan<float> output, ReadOnlySpan<float> values, float scalar, bool maximum)
    {
        for (int i = 0; i < output.Length; i++)
        {
            var expected = maximum ? MathF.Max(values[i], scalar) : MathF.Min(values[i], scalar);
            Assert.AreEqual(expected, output[i]);
        }
    }

    private static void AssertMinMax(ReadOnlySpan<float> output, ReadOnlySpan<float> left, ReadOnlySpan<float> right, bool maximum)
    {
        for (int i = 0; i < output.Length; i++)
        {
            var expected = maximum ? MathF.Max(left[i], right[i]) : MathF.Min(left[i], right[i]);
            Assert.AreEqual(expected, output[i]);
        }
    }
}
