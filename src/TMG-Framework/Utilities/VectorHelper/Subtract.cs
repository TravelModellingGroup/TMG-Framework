/*
    Copyright 2015-2026 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF.

    XTMF is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Runtime.Intrinsics;

namespace TMG.Utilities;

public static partial class VectorHelper
{
    /// <summary>
    /// dest[i] = left[i] - right[i]
    /// </summary>
    /// <param name="dest">The destination span where the result is stored.</param>
    /// <param name="left">The left-hand side span.</param>
    /// <param name="right">The right-hand side span.</param>
    public static void Subtract(Span<float> dest, ReadOnlySpan<float> left, ReadOnlySpan<float> right)
    {
        EnsureSameSize(dest, left, right);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) - Unsafe.Add(ref rightRef, i);
        }
    }

    /// <summary>
    /// dest[i] = left[i] - right
    /// </summary>
    /// <param name="dest">The destination span where the result is stored.</param>
    /// <param name="left">The left-hand side span.</param>
    /// <param name="right">The right-hand side scalar.</param>
    public static void Subtract(Span<float> dest, ReadOnlySpan<float> left, float scalar)
    {
        EnsureSameSize(dest, left);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vRight = Vector512.Create(scalar);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);

                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight256 = Vector256.Create(scalar);
                var vResult = vLeft - vRight256;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vRight = Vector256.Create(scalar);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) - scalar;
        }
    }

    /// <summary>
    /// dest[i] = left - right[i]
    /// </summary>
    /// <param name="dest">The destination span where the result is stored.</param>
    /// <param name="left">The left-hand side scalar.</param>
    /// <param name="right">The right-hand side span.</param>
    public static void Subtract(Span<float> dest, float left, ReadOnlySpan<float> right)
    {
        EnsureSameSize(dest, right);
        nuint i = 0;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float rightRef = ref MemoryMarshal.GetReference(right);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var end = (nuint)dest.Length - 16;
            var vLeft = Vector512.Create(left);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft256 = Vector256.Create(left);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft256 - vRight;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            var vLeft = Vector256.Create(left);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft - vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = left - Unsafe.Add(ref rightRef, i);
        }
    }
}

