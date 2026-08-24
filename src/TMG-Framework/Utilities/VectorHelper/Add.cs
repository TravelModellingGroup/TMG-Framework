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

using System.Numerics;
using System.Runtime.Intrinsics;

namespace TMG.Utilities;

public static partial class VectorHelper
{
    public static void Add(Span<float> dest, ReadOnlySpan<float> left, ReadOnlySpan<float> right)
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
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft + vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft + vRight;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft + vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) + Unsafe.Add(ref rightRef, i);
        }
    }

    public static void Add(Span<float> dest, ReadOnlySpan<float> left, float scalar)
    {
        EnsureSameSize(dest, left);
        nuint i = 0;
        
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector512<float>.Count);
            var vRight = Vector512.Create(scalar);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vResult = vLeft + vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight256 = Vector256.Create(scalar);
                var vResult = vLeft + vRight256;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            var vRight = Vector256.Create(scalar);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vResult = vLeft + vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) + scalar;
        }
    }

    public static void Add(Span<float> dest, float lhs, ReadOnlySpan<float> rhs)
    {
        Add(dest, rhs, lhs);
    }

    public static void Add(Span<float> dest, ReadOnlySpan<float> left, ReadOnlySpan<float> right, ReadOnlySpan<float> third)
    {
        EnsureSameSize(dest, left, right, third);
        nuint i = 0;
        
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);
        ref float thirdRef = ref MemoryMarshal.GetReference(third);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector512<float>.Count);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vThird = Vector512.LoadUnsafe(ref thirdRef, i);
                var vResult = vLeft + vRight + vThird;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vResult = vLeft + vRight + vThird;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vResult = vLeft + vRight + vThird;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                + Unsafe.Add(ref rightRef, i) + Unsafe.Add(ref thirdRef, i);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="destIndex"></param>
    /// <param name="first"></param>
    /// <param name="firstIndex"></param>
    /// <param name="second"></param>
    /// <param name="secondIndex"></param>
    /// <param name="length"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(float[] dest, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
    {
        var vectorLength = length / Vector<float>.Count;
        var remainder = length % Vector<float>.Count;
        var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
        var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
        var secondSpan = (new Span<float>(second, secondIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
        // copy everything we can do inside of a vector
        int i = 0;
        for (; i < vectorLength - 1; i += 2)
        {
            destSpan[i] = firstSpan[i] + secondSpan[i];
            destSpan[i + 1] = firstSpan[i + 1] + secondSpan[i + 1];
        }
        i *= Vector<float>.Count;
        for (; i < length; i++)
        {
            dest[destIndex + i] = first[firstIndex + i] + second[secondIndex + i];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="destIndex"></param>
    /// <param name="lhs"></param>
    /// <param name="lhsIndex"></param>
    /// <param name="rhs"></param>
    /// <param name="rhsIndex"></param>
    /// <param name="length"></param>
    internal static void Add(Span<float> dest, int destIndex, Span<float> lhs, int lhsIndex, Span<float> rhs, int rhsIndex, int length)
    {
        var vectorLength = length / Vector<float>.Count;
        var remainder = length % Vector<float>.Count;
        var destSpan = (dest.Slice(destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
        var lhsSpan = (lhs.Slice(lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
        var rhsSpan = (rhs.Slice(rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
        int i = 0;
        for (; i < vectorLength - 1; i += 2)
        {
            destSpan[i] = lhsSpan[i] + rhsSpan[i];
            destSpan[i + 1] = lhsSpan[i + 1] + rhsSpan[i + 1];
        }
        i *= Vector<float>.Count;
        for (; i < length; i++)
        {
            dest[destIndex + i] = lhs[lhsIndex + i] + rhs[rhsIndex + i];
        }
    }

    public static void Add(float[][] destination, float lhs, float[][] rhs)
    {
        Parallel.For(0, destination.Length, row =>
        {
            Add(destination[row], rhs[row], lhs);
        });
    }

    public static void Add(float[][] destination, float[][] lhs, float rhs)
    {
        Parallel.For(0, destination.Length, row =>
        {
            Add(destination[row], lhs[row], rhs);
        });
    }

    public static void Add(float[][] destination, float[][] lhs, float[][] rhs)
    {
        Parallel.For(0, destination.Length, row =>
        {
            Add(destination[row], lhs[row], rhs[row]);
        });
    }

    public static void AddHorizontal(float[][] destination, float[][] lhs, float[] rhs)
    {
        Parallel.For(0, destination.Length, i =>
        {
            Add(destination[i], 0, lhs[i], 0, rhs, 0, destination[i].Length);
        });
    }

    public static void AddVertical(float[][] destination, float[][] lhs, float[] rhs)
    {
        Parallel.For(0, destination.Length, i =>
        {
            Add(destination[i], lhs[i], rhs[i]);
        });
    }
}
