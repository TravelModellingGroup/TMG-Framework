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
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
using static System.Numerics.Vector;

namespace TMG.Utilities;

public static partial class VectorHelper
{
    /// <summary>
    /// dest[i] = lhs >= rhs[i] ? 1.0f : 0.0f
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The scalar value to compare against.</param>
    /// <param name="rhs">The data span.</param>
    public static void FlagIfGreaterThanOrEqual(Span<float> dest, float lhs, ReadOnlySpan<float> rhs)
    {
        EnsureSameSize(dest, rhs);

        nuint i = 0;
        var length = (nuint)rhs.Length;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pRhs = ref MemoryMarshal.GetReference(rhs);
        if (Vector512.IsHardwareAccelerated && length >= (nuint)Vector512<float>.Count)
        {
            var vZero = Vector512<float>.Zero;
            var vOne = Vector512<float>.One;
            var lhsV = Vector512.Create(lhs);
            for (; i <= length - (nuint)Vector512<float>.Count; i += (nuint)Vector512<float>.Count)
            {
                var rhsV = Vector512.LoadUnsafe(ref pRhs, i);
                var destV = Blend(vZero, vOne, Vector512.GreaterThanOrEqual(lhsV, rhsV));
                destV.StoreUnsafe(ref pDest, i);
            }

            if (i <= length - (nuint)Vector256<float>.Count)
            {
                var vZero256 = Vector256<float>.Zero;
                var vOne256 = Vector256<float>.One;
                var rhsV = Vector256.LoadUnsafe(ref pRhs, i);
                var destV = Blend(vZero256, vOne256, Vector256.GreaterThanOrEqual(lhsV.GetLower(), rhsV));
                destV.StoreUnsafe(ref pDest, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<float>.Count)
        {
            var vZero = Vector256<float>.Zero;
            var vOne = Vector256<float>.One;
            var lhsV = Vector256.Create(lhs);
            for (; i <= length - (nuint)Vector256<float>.Count; i += (nuint)Vector256<float>.Count)
            {
                var rhsV = Vector256.LoadUnsafe(ref pRhs, i);
                var destV = Blend(vZero, vOne, Vector256.GreaterThanOrEqual(lhsV, rhsV));
                destV.StoreUnsafe(ref pDest, i);
            }
        }
        // Process the remainder.
        for (; i < length; i++)
        {
            Unsafe.Add(ref pDest, i) = (lhs >= Unsafe.Add(ref pRhs, i)) ? 1.0f : 0.0f;
        }

    }

    /// <summary>
    /// dest[i] = lhs[i] >= rhs[i] ? 1.0f : 0.0f
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The left hand side span.</param>
    /// <param name="rhs">The right hand side span.</param>
    public static void FlagIfGreaterThanOrEqual(Span<float> dest, ReadOnlySpan<float> lhs, ReadOnlySpan<float> rhs)
    {
        EnsureSameSize(dest, lhs, rhs);

        nuint i = 0;
        var length = (nuint)rhs.Length;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pLhs = ref MemoryMarshal.GetReference(lhs);
        ref var pRhs = ref MemoryMarshal.GetReference(rhs);
        if (Vector512.IsHardwareAccelerated && length >= (nuint)Vector512<float>.Count)
        {
            var vZero = Vector512<float>.Zero;
            var vOne = Vector512<float>.One;
            for (; i <= length - (nuint)Vector512<float>.Count; i += (nuint)Vector512<float>.Count)
            {
                var rhsV = Vector512.LoadUnsafe(ref pRhs, i);
                var lhsV = Vector512.LoadUnsafe(ref pLhs, i);
                var destV = Blend(vZero, vOne, Vector512.GreaterThanOrEqual(lhsV, rhsV));
                destV.StoreUnsafe(ref pDest, i);
            }

            if (i <= length - (nuint)Vector256<float>.Count)
            {
                var vZero256 = Vector256<float>.Zero;
                var vOne256 = Vector256<float>.One;
                var rhsV = Vector256.LoadUnsafe(ref pRhs, i);
                var lhsV = Vector256.LoadUnsafe(ref pLhs, i);
                var destV = Blend(vZero256, vOne256, Vector256.GreaterThanOrEqual(lhsV, rhsV));
                destV.StoreUnsafe(ref pDest, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<float>.Count)
        {
            var vZero = Vector256<float>.Zero;
            var vOne = Vector256<float>.One;
            for (; i <= length - (nuint)Vector256<float>.Count; i += (nuint)Vector256<float>.Count)
            {
                var rhsV = Vector256.LoadUnsafe(ref pRhs, i);
                var lhsV = Vector256.LoadUnsafe(ref pLhs, i);
                var destV = Blend(vZero, vOne, Vector256.GreaterThanOrEqual(lhsV, rhsV));
                destV.StoreUnsafe(ref pDest, i);
            }
        }
        // Process the remainder.
        for (; i < length; i++)
        {
            Unsafe.Add(ref pDest, i) = (Unsafe.Add(ref pLhs, i) >= Unsafe.Add(ref pRhs, i)) ? 1.0f : 0.0f;
        }
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagIfGreaterThanOrEqual(float[] dest, int destIndex, float lhs, float[] rhs, int rhsIndex, int length)
    {
        FlagIfGreaterThanOrEqual(new Span<float>(dest, destIndex, length), lhs, new ReadOnlySpan<float>(rhs, rhsIndex, length));
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagIfGreaterThanOrEqual(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, int length)
    {
        FlagIfLessThanOrEqual(dest, destIndex, rhs, lhs, lhsIndex, length);
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagIfGreaterThanOrEqual(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, int length)
    {
        FlagIfGreaterThanOrEqual(new Span<float>(dest, destIndex, length), new ReadOnlySpan<float>(lhs, lhsIndex, length), new ReadOnlySpan<float>(rhs, rhsIndex, length));
    }
}
