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

namespace TMG.Utilities;

public static partial class VectorHelper
{
        /// <summary>
    /// Raises each element of the lhs span to the power of rhs and stores the result in dest.
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The left-hand side scalar.</param>
    /// <param name="rhs">The right-hand side span.</param>
    public static void Pow(Span<float> dest, ReadOnlySpan<float> lhs, ReadOnlySpan<float> rhs)
    {
        EnsureSameSize(dest, lhs, rhs);
        nuint i = 0;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pLhs = ref MemoryMarshal.GetReference(lhs);
        ref var pRhs = ref MemoryMarshal.GetReference(rhs);
        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            for (; i <= (nuint)(rhs.Length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                var vx = Vector512.LoadUnsafe(ref pLhs, i);
                var vy = Vector512.LoadUnsafe(ref pRhs, i);
                var res = Pow(vx, vy);
                Vector512.StoreUnsafe(res, ref pDest, i);
            }
            
            // If there are remaining 256 bit vectors, process them
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                for (; i <= (nuint)(rhs.Length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
                {
                    var vx = Vector256.LoadUnsafe(ref pLhs, i);
                    var vy = Vector256.LoadUnsafe(ref pRhs, i);
                    var res = Pow(vx, vy);
                    Vector256.StoreUnsafe(res, ref pDest, i);
                }
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            for (; i <= (nuint)(dest.Length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var vx = Vector256.LoadUnsafe(ref pLhs, i);
                var vy = Vector256.LoadUnsafe(ref pRhs, i);
                var res = Pow(vx, vy);
                Vector256.StoreUnsafe(res, ref pDest, i);
            }
        }
        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref pDest, i) = MathF.Pow(Unsafe.Add(ref pLhs, i), Unsafe.Add(ref pRhs, i));
        }
    }

    /// <summary>
    /// Raises each element of the lhs span to the power of rhs and stores the result in dest.
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The left-hand side span.</param>
    /// <param name="rhs">The right-hand side scalar.</param>
    public static void Pow(Span<float> dest, ReadOnlySpan<float> lhs, float rhs)
    {
        EnsureSameSize(dest, lhs);
        nuint i = 0;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pLhs = ref MemoryMarshal.GetReference(lhs);
        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vy = Vector512.Create(rhs);
            for (; i <= (nuint)(lhs.Length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                var vx = Vector512.LoadUnsafe(ref pLhs, i);
                var res = Pow(vx, vy);
                Vector512.StoreUnsafe(res, ref pDest, i);
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vy = Vector256.Create(rhs);
            for (; i <= (nuint)(dest.Length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var vx = Vector256.LoadUnsafe(ref pLhs, i);
                var res = Pow(vx, vy);
                Vector256.StoreUnsafe(res, ref pDest, i);
            }
        }
        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref pDest, i) = MathF.Pow(Unsafe.Add(ref pLhs, i), rhs);
        }
    }

    /// <summary>
    /// Raises each element of the lhs span to the power of rhs and stores the result in dest.
    /// </summary>
    /// <param name="dest">The destination span.</param>
    /// <param name="lhs">The left-hand side scalar.</param>
    /// <param name="rhs">The right-hand side span.</param>
    public static void Pow(Span<float> dest, float lhs, ReadOnlySpan<float> rhs)
    {
        EnsureSameSize(dest, rhs);
        nuint i = 0;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pRhs = ref MemoryMarshal.GetReference(rhs);
        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vx = Vector512.Create(lhs);
            for (; i <= (nuint)(rhs.Length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                var vy = Vector512.LoadUnsafe(ref pRhs, i);
                var res = Pow(vx, vy);
                Vector512.StoreUnsafe(res, ref pDest, i);
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vx = Vector256.Create(lhs);
            for (; i <= (nuint)(dest.Length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var vy = Vector256.LoadUnsafe(ref pRhs, i);
                var res = Pow(vx, vy);
                Vector256.StoreUnsafe(res, ref pDest, i);
            }
        }
        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref pDest, i) = MathF.Pow(lhs, Unsafe.Add(ref pRhs, i));
        }
    }

    /// <summary>
    /// Computes x^y for each element in the vector.
    /// </summary>
    /// <param name="x">The base of the exponent.</param>
    /// <param name="y">The exponential term</param>
    /// <returns>A vector with x^y</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]   
    public static Vector512<float> Pow(Vector512<float> x, Vector512<float> y)
    {
        // Upscale to double percision
        (var xlow, var xhigh) = ConvertToDouble(x);
        (var ylow, var yhigh) = ConvertToDouble(y);
        var low = Pow(xlow, ylow);
        var high = Pow(xhigh, yhigh);
        // Downscale to single precision
        return ConvertToFloat(low, high);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector512<double> Pow(Vector512<double> x, Vector512<double> y)
    {
        // We need to handle cases where y is negative but is an integer
        // If it is not an integer, let it fail like normal
        if (Vector512.LessThanAny(x, Vector512<double>.Zero))
        {
            return SlowPow(x, y);
        }
        return Vector512.Exp(y * Vector512.Log(x));
    }

    private static Vector512<double> SlowPow(Vector512<double> x, Vector512<double> y)
    {
        var integerMask = Vector512.Equals(Vector512.Floor(y), y);
        // Only take the abs of integer exponents, everything else should be NaN
        var result = Vector512.Exp(y * Vector512.Log(Blend(x, Vector512.Abs(x), integerMask)));
        // if it is odd then we need to negate the result
        var iy = Vector512.ConvertToInt64Native(y);
        var isOdd = Vector512.Equals(Vector512.BitwiseAnd(iy, Vector512<long>.One), Vector512<long>.One)
            .As<long, double>();
        // Negative mask to only apply to odd exponents where the base is negative
        var negativeMask = Vector512.BitwiseAnd(
                Vector512.IsNegative(x),
                isOdd);
        result = Blend(result, Vector512.Negate(result), negativeMask);
        return result;
    }

    /// <summary>
    /// Computes x^y for each element in the vector.
    /// </summary>
    /// <param name="x">The base of the exponent.</param>
    /// <param name="y">The exponential term</param>
    /// <returns>A vector with x^y</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector256<float> Pow(Vector256<float> x, Vector256<float> y)
    {
        // Upscale to double percision
        (var xlow, var xhigh) = ConvertToDouble(x);
        (var ylow, var yhigh) = ConvertToDouble(y);
        var low = Pow(xlow, ylow);
        var high = Pow(xhigh, yhigh);
        // Downscale to single precision
        return ConvertToFloat(low, high);
    }

    /// <summary>
    /// Computes x^y for each element in the vector.
    /// </summary>
    /// <param name="x">The base of the exponent.</param>
    /// <param name="y">The exponential term</param>
    /// <returns>A vector with x^y</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector256<double> Pow(Vector256<double> x, Vector256<double> y)
    {
        // We need to handle cases where y is negative but is an integer
        // If it is not an integer, let it fail like normal
        if (Vector256.LessThanAny(x, Vector256<double>.Zero))
        {
            return SlowPow(x, y);
        }
        return Vector256.Exp(y * Vector256.Log(x));
    }

    private static Vector256<double> SlowPow(Vector256<double> x, Vector256<double> y)
    {
        var integerMask = Vector256.Equals(Vector256.Floor(y), y);
        // Only take the abs of integer exponents, everything else should be NaN
        var result = Vector256.Exp(y * Vector256.Log(Blend(x, Vector256.Abs(x), integerMask)));
        var iy = Vector256.ConvertToInt64Native(y);
        var isOdd = Vector256.Equals(Vector256.BitwiseAnd(iy, Vector256<long>.One), Vector256<long>.One)
            .As<long, double>();
        // Negative mask to only apply to odd exponents where the base is negative
        var negativeMask = Vector256.BitwiseAnd(
                Vector256.IsNegative(x),
                isOdd);
        result = Blend(result, Vector256.Negate(result), negativeMask);
        return result;
    }

    /// <summary>
    /// Computes x^y for each element in the vector.
    /// </summary>
    /// <param name="x">The base of the exponent.</param>
    /// <param name="y">The exponential term</param>
    /// <returns>A vector with x^y</returns>
    public static Vector512<float> Pow(float x, Vector512<float> y)
    {
        var vx = Vector512.Create(x);
        return Pow(vx, y);
    }

    /// <summary>
    /// Computes x^y for each element in the vector.
    /// </summary>
    /// <param name="x">The base of the exponent.</param>
    /// <param name="y">The exponential term</param>
    /// <returns>A vector with x^y</returns>
    public static Vector256<float> Pow(float x, Vector256<float> y)
    {
        var vx = Vector256.Create(x);
        return Pow(vx, y);
    }

}
