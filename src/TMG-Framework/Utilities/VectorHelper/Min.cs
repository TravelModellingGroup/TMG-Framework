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

using System.Runtime.Intrinsics;

namespace TMG.Utilities;

public static partial class VectorHelper
{
    /// <summary>
    /// Returns the smaller of two scalar values.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns>The smaller value.</returns>
    public static float Min(float left, float right) => MathF.Min(left, right);

    /// <summary>
    /// Returns the smallest value in a span.
    /// </summary>
    /// <param name="source">The values to examine.</param>
    /// <returns>The smallest value.</returns>
    /// <exception cref="ArgumentException"><paramref name="source"/> is empty.</exception>
    public static float Min(ReadOnlySpan<float> source)
    {
        if (source.IsEmpty)
        {
            throw new ArgumentException("Cannot compute a minimum of an empty span.", nameof(source));
        }

        nuint i = 0;
        float result = float.PositiveInfinity;
        ref float sourceRef = ref MemoryMarshal.GetReference(source);

        if (Vector512.IsHardwareAccelerated && source.Length >= Vector512<float>.Count)
        {
            var end = (nuint)(source.Length - Vector512<float>.Count);
            var accumulator = Vector512.Create(float.PositiveInfinity);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                accumulator = Vector512.Min(accumulator, Vector512.LoadUnsafe(ref sourceRef, i));
            }

            Span<float> lanes = stackalloc float[Vector512<float>.Count];
            accumulator.CopyTo(lanes);
            for (int lane = 0; lane < lanes.Length; lane++)
            {
                result = MathF.Min(result, lanes[lane]);
            }

            if ((nuint)source.Length - i >= (nuint)Vector256<float>.Count)
            {
                var values = Vector256.LoadUnsafe(ref sourceRef, i);
                var accumulator256 = Vector256.Min(Vector256.Create(float.PositiveInfinity), values);
                Span<float> lanes256 = stackalloc float[Vector256<float>.Count];
                accumulator256.CopyTo(lanes256);
                for (int lane = 0; lane < lanes256.Length; lane++)
                {
                    result = MathF.Min(result, lanes256[lane]);
                }
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && source.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(source.Length - Vector256<float>.Count);
            var accumulator = Vector256.Create(float.PositiveInfinity);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                accumulator = Vector256.Min(accumulator, Vector256.LoadUnsafe(ref sourceRef, i));
            }

            Span<float> lanes = stackalloc float[Vector256<float>.Count];
            accumulator.CopyTo(lanes);
            for (int lane = 0; lane < lanes.Length; lane++)
            {
                result = MathF.Min(result, lanes[lane]);
            }
        }

        for (; i < (nuint)source.Length; i++)
        {
            result = MathF.Min(result, Unsafe.Add(ref sourceRef, i));
        }
        return result;
    }

    /// <summary>
    /// Writes the element-wise minima of two spans to the destination.
    /// </summary>
    /// <param name="dest">The span to receive the minima.</param>
    /// <param name="left">The first span of values.</param>
    /// <param name="right">The second span of values.</param>
    /// <exception cref="ArgumentException">The spans do not have the same length.</exception>
    public static void Min(Span<float> dest, ReadOnlySpan<float> left, ReadOnlySpan<float> right)
    {
        EnsureSameSize(dest, left, right);
        nuint i = 0;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector512<float>.Count);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var values = Vector512.Min(
                    Vector512.LoadUnsafe(ref leftRef, i),
                    Vector512.LoadUnsafe(ref rightRef, i));
                values.StoreUnsafe(ref destRef, i);
            }
            if ((nuint)dest.Length - i >= (nuint)Vector256<float>.Count)
            {
                var values = Vector256.Min(
                    Vector256.LoadUnsafe(ref leftRef, i),
                    Vector256.LoadUnsafe(ref rightRef, i));
                values.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var values = Vector256.Min(
                    Vector256.LoadUnsafe(ref leftRef, i),
                    Vector256.LoadUnsafe(ref rightRef, i));
                values.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = MathF.Min(Unsafe.Add(ref leftRef, i), Unsafe.Add(ref rightRef, i));
        }
    }

    /// <summary>
    /// Writes the element-wise minima of a span and a scalar to the destination.
    /// </summary>
    /// <param name="dest">The span to receive the minima.</param>
    /// <param name="source">The values to compare with the scalar.</param>
    /// <param name="scalar">The scalar value compared with each element.</param>
    /// <exception cref="ArgumentException"><paramref name="dest"/> and <paramref name="source"/> do not have the same length.</exception>
    public static void Min(Span<float> dest, ReadOnlySpan<float> source, float scalar)
    {
        EnsureSameSize(dest, source);
        nuint i = 0;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float sourceRef = ref MemoryMarshal.GetReference(source);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector512<float>.Count);
            var scalarVector = Vector512.Create(scalar);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var values = Vector512.Min(Vector512.LoadUnsafe(ref sourceRef, i), scalarVector);
                values.StoreUnsafe(ref destRef, i);
            }
            if ((nuint)dest.Length - i >= (nuint)Vector256<float>.Count)
            {
                var values = Vector256.Min(
                    Vector256.LoadUnsafe(ref sourceRef, i),
                    Vector256.Create(scalar));
                values.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            var scalarVector = Vector256.Create(scalar);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var values = Vector256.Min(Vector256.LoadUnsafe(ref sourceRef, i), scalarVector);
                values.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = MathF.Min(Unsafe.Add(ref sourceRef, i), scalar);
        }
    }

    /// <summary>
    /// Writes the element-wise minima of a scalar and a span to the destination.
    /// </summary>
    /// <param name="dest">The span to receive the minima.</param>
    /// <param name="scalar">The scalar value compared with each element.</param>
    /// <param name="source">The values to compare with the scalar.</param>
    /// <exception cref="ArgumentException"><paramref name="dest"/> and <paramref name="source"/> do not have the same length.</exception>
    public static void Min(Span<float> dest, float scalar, ReadOnlySpan<float> source)
    {
        Min(dest, source, scalar);
    }
}
