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
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
using static System.Numerics.Vector;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace TMG.Utilities;

public static partial class VectorHelper
{
    /// <summary>
    /// dest[i] = value != 0 OR data != 0 ? 1.0f : 0.0f
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The scalar value to compare against.</param>
    /// <param name="rhs">The data span.</param>
    public static void FlagOr(Span<float> dest, float lhs, ReadOnlySpan<float> rhs)
    {
        EnsureSameSize(dest, rhs);
        // check if we are supposed to just clear everything and use a faster function for that
        if (lhs == 1.0f)
        {
            dest.Fill(1.0f);
            return;
        }

        // Since we know that the lhs is not zero, we only need to check rhs for zero.
        nuint i = 0;
        var length = (nuint)rhs.Length;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pRhs = ref MemoryMarshal.GetReference(rhs);
        if (Vector512.IsHardwareAccelerated && length >= (nuint)Vector512<float>.Count)
        {
            var vZero = Vector512<float>.Zero;
            var vOne = Vector512<float>.One;
            for (; i <= length - (nuint)Vector512<float>.Count; i += (nuint)Vector512<float>.Count)
            {
                var dataV = Vector512.LoadUnsafe(ref pRhs, i);
                var destV = Blend(vOne, vZero, Vector512.Equals(dataV, vZero));
                destV.StoreUnsafe(ref pDest, i);
            }

            if (i <= length - (nuint)Vector256<float>.Count)
            {
                var vZero256 = Vector256<float>.Zero;
                var vOne256 = Vector256<float>.One;
                var dataV = Vector256.LoadUnsafe(ref pRhs, i);
                var destV = Blend(vOne256, vZero256, Vector256.Equals(dataV, vZero256));
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
                var dataV = Vector256.LoadUnsafe(ref pRhs, i);
                var destV = Blend(vOne, vZero, Vector256.Equals(dataV, vZero));
                destV.StoreUnsafe(ref pDest, i);
            }
        }
        // Process the remainder.
        for (; i < length; i++)
        {
            Unsafe.Add(ref pDest, i) = (Unsafe.Add(ref pRhs, i) == 0) ? 0.0f : 1.0f;
        }

    }

    /// <summary>
    /// dest[i] = value[i] != 0 OR data != 0 ? 1.0f : 0.0f
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The scalar value to compare against.</param>
    /// <param name="rhs">The data span.</param>
    public static void FlagOr(Span<float> dest, ReadOnlySpan<float> lhs, ReadOnlySpan<float> rhs)
    {
        EnsureSameSize(dest, lhs, rhs);
        // check if we are supposed to just clear everything and use a faster function for that
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
                var destV = Blend(vOne, vZero, Vector512.BitwiseAnd(Vector512.Equals(rhsV, vZero), Vector512.Equals(lhsV, vZero)));
                destV.StoreUnsafe(ref pDest, i);
            }

            if (i <= length - (nuint)Vector256<float>.Count)
            {
                var vZero256 = Vector256<float>.Zero;
                var vOne256 = Vector256<float>.One;
                var rhsV = Vector256.LoadUnsafe(ref pRhs, i);
                var lhsV = Vector256.LoadUnsafe(ref pLhs, i);
                var destV = Blend(vOne256, vZero256, Vector256.BitwiseAnd(Vector256.Equals(rhsV, vZero256), Vector256.Equals(lhsV, vZero256)));
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
                var destV = Blend(vOne, vZero, Vector256.BitwiseAnd(Vector256.Equals(rhsV, vZero), Vector256.Equals(lhsV, vZero)));
                destV.StoreUnsafe(ref pDest, i);
            }
        }
        // Process the remainder.
        for (; i < length; i++)
        {
            Unsafe.Add(ref pDest, i) = ((Unsafe.Add(ref pRhs, i) == 0) & (Unsafe.Add(ref pLhs, i) == 0)) ? 0.0f : 1.0f;
        }
    }


    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[] dest, float value, float[] data)
    {
        FlagOr(dest.AsSpan(), value, new ReadOnlySpan<float>(data));
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[] dest, int destIndex, float value, float[] data, int dataIndex, int length)
    {
        FlagOr(new Span<float>(dest, destIndex, length), value, new ReadOnlySpan<float>(data, dataIndex, length));
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, int length)
    {
        FlagOr(new Span<float>(dest, destIndex, length), new ReadOnlySpan<float>(lhs, lhsIndex, length), new ReadOnlySpan<float>(rhs, rhsIndex, length));
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[][] dest, float[][] data, float literalValue)
    {
        Parallel.For(0, dest.Length, i =>
        {
            FlagOr(dest[i], data[i], literalValue);
        });
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[] dest, float[] data, float literalValue)
    {
        FlagOr(dest.AsSpan(), literalValue, new ReadOnlySpan<float>(data));
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[][] dest, float[][] lhs, float[][] rhs)
    {
        Parallel.For(0, dest.Length, i =>
        {
            FlagOr(dest[i], 0, lhs[i], 0, rhs[i], 0, dest.Length);
        });
    }

    /// <summary>
    /// Set the value to one if the condition is met.
    /// </summary>
    public static void FlagOr(float[][] v1, float literalValue, float[][] v2)
    {
        Parallel.For(0, v1.Length, i =>
        {
            FlagOr(v1[i], literalValue, v2[i]);
        });
    }
}
