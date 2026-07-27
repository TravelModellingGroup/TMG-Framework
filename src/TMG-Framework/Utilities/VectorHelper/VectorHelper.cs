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
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace TMG.Utilities;

/// <summary>
/// This class is designed to help facilitate the use of the SIMD instructions available in
/// modern .Net.
/// </summary>
public static partial class VectorHelper
{

    /// <summary>
    /// A vector containing the maximum value of a float
    /// </summary>
    private static Vector<float> MaxFloat;

    private static Vector256<float> MaxFloat256;

    private static Vector512<float> MaxFloat512;

    static VectorHelper()
    {
        MaxFloat = new Vector<float>(float.MaxValue);
        MaxFloat256 = Vector256.Create(float.MaxValue);
        MaxFloat512 = Vector512.Create(float.MaxValue);
    }

    /// <summary>
    /// Sum an array
    /// </summary>
    /// <param name="array">The array to Sum</param>
    /// <param name="startIndex">The index to start summing from</param>
    /// <param name="length">The number of elements to add</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sum(float[] array, int startIndex, int length)
    {
        return Sum(new Span<float>(array, startIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sum(Span<float> array)
    {
        var length = array.Length;
        ref var rf = ref MemoryMarshal.GetReference(array);
        nuint i = 0;
        float acc = 0.0f;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported

        if (Vector512.IsHardwareAccelerated && length >= Vector512<float>.Count)
        {
            var end = (nuint)(length - 16);
            Vector512<float> acc1 = Vector512<float>.Zero;
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                acc1 += f;
            }
            acc += Vector512.Sum(acc1);
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                acc += Vector256.Sum(f);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= Vector256<float>.Count * 2)
        {
            var end = (nuint)(length - Vector256<float>.Count * 2);
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> acc1 = Vector256<float>.Zero;
            Vector256<float> acc2 = Vector256<float>.Zero;
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                acc1 += f;
                acc2 += f2;
            }
            acc += Vector256.Sum(acc1 + acc2);
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                acc += Vector256.Sum(f);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            acc += Unsafe.Add(ref rf, i);
        }
        return acc;
    }

    /// <summary>
    /// Take the average of the absolute values
    /// </summary>
    /// <param name="first">The first vector</param>
    /// <param name="firstIndex">Where to start in the first vector</param>
    /// <param name="second">The second vector</param>
    /// <param name="secondIndex">Where to start in the second vector</param>
    /// <param name="length">The number of elements to read</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AbsDiffAverage(float[] first, int firstIndex, float[] second, int secondIndex, int length)
    {
        return AbsDiffAverage(new Span<float>(first, firstIndex, length), new Span<float>(second, secondIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AbsDiffAverage(Span<float> first, Span<float> second)
    {
        EnsureSameSize(first, second);
        var length = first.Length;
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        nuint i = 0;
        float acc = 0.0f;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> acc1 = Vector512<float>.Zero;
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var result = Vector512.Abs(f - s);
                acc1 += result;
            }
            acc += Vector512.Sum(acc1);
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = Vector256.Abs(f - s);
                acc += Vector256.Sum(result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> acc1 = Vector256<float>.Zero;
            Vector256<float> acc2 = Vector256<float>.Zero;
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var result1 = Vector256.Abs(f - s);
                var result2 = Vector256.Abs(f2 - s2);
                acc1 += result1;
                acc2 += result2;
            }
            acc += Vector256.Sum(acc1 + acc2);
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = Vector256.Abs(f - s);
                acc += Vector256.Sum(result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = MathF.Abs(Unsafe.Add(ref rf, i) - Unsafe.Add(ref rs, i));
            acc += result;
        }
        return acc / length;
    }

    /// <summary>
    /// Get the maximum difference from two arrays.
    /// </summary>
    /// <param name="first">The first vector</param>
    /// <param name="firstIndex">Where to start in the first vector</param>
    /// <param name="second">The second vector</param>
    /// <param name="secondIndex">Where to start in the second vector</param>
    /// <param name="length">The number of elements to read</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AbsDiffMax(float[] first, int firstIndex, float[] second, int secondIndex, int length)
    {
        return AbsDiffMax(new Span<float>(first, firstIndex, length), new Span<float>(second, secondIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AbsDiffMax(Span<float> first, Span<float> second)
    {
        EnsureSameSize(first, second);
        var remainder = first.Length % Vector<float>.Count;
        var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
        var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
        int i = 0;
        var acc1 = Vector<float>.Zero;
        var acc2 = Vector<float>.Zero;
        for (; i < vectorFirst.Length - 1; i += 2)
        {
            acc1 = System.Numerics.Vector.Max(System.Numerics.Vector.Abs(vectorFirst[i] - vectorSecond[i]), acc1);
            acc2 = System.Numerics.Vector.Max(System.Numerics.Vector.Abs(vectorFirst[i + 1] - vectorSecond[i + 1]), acc2);
        }
        i *= Vector<float>.Count;
        float maxAbsDiff = 0.0f;
        for (; i < first.Length; i++)
        {
            maxAbsDiff = Math.Max(Math.Abs(first[i] - second[i]), maxAbsDiff);
        }
        acc1 = System.Numerics.Vector.Max(acc1, acc2);
        float[] temp = new float[Vector<float>.Count];
        acc1.CopyTo(temp);
        for (int j = 0; j < temp.Length; j++)
        {
            maxAbsDiff = Math.Max(temp[j], maxAbsDiff);
        }
        return maxAbsDiff;
    }

    /// <summary>
    /// Sum the square differences of two arrays
    /// </summary>
    /// <param name="first">The array to Sum</param>
    /// <param name="firstIndex">The index to start summing from</param>
    /// <param name="second">The array to Sum</param>
    /// <param name="secondIndex">The index to start summing from</param>
    /// <param name="length">The number of elements to add</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SquareDiff(float[] first, int firstIndex, float[] second, int secondIndex, int length)
    {
        return SquareDiff(new Span<float>(first, firstIndex, length), new Span<float>(second, secondIndex, length));
    }

    public static float SquareDiff(Span<float> first, Span<float> second)
    {
        EnsureSameSize(first, second);
        var length = first.Length;
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        nuint i = 0;
        float acc = 0.0f;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> acc1 = Vector512<float>.Zero;
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var result = f - s;
                acc1 += result * result;
            }
            acc += Vector512.Sum(acc1);
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = f * s;
                acc += Vector256.Sum(result * result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> acc1 = Vector256<float>.Zero;
            Vector256<float> acc2 = Vector256<float>.Zero;
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var result1 = f - s;
                var result2 = f2 - s2;
                acc1 += result1 * result1;
                acc2 += result2 * result2;
            }
            acc += Vector256.Sum(acc1 + acc2);
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = f - s;
                acc += Vector256.Sum(result * result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = MathF.Abs(Unsafe.Add(ref rf, i) - Unsafe.Add(ref rs, i));
            acc += result * result;
        }
        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(Span<float> dest, float value)
    {
        dest.Fill(value);
    }

    /// <summary>
    /// Assign the given value to the whole array
    /// </summary>
    /// <param name="dest">The array to set</param>
    /// <param name="value">The value to set it to</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(float[] dest, float value)
    {
        Array.Fill(dest, value);
    }

    /// <summary>
    /// Assign the given value to the whole array
    /// </summary>
    /// <param name="dest">The array to set</param>
    /// <param name="offset">The offset into the destination to start</param>
    /// <param name="value">The value to assign to it</param>
    /// <param name="length">The number of elements to assign</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(float[] dest, int offset, float value, int length)
    {
        Set(new Span<float>(dest, offset, length), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Abs(Span<float> dest, Span<float> source)
    {
        EnsureSameSize(dest, source);
        nuint i = 0;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pSource = ref MemoryMarshal.GetReference(source);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            // copy everything we can do inside of a vector
            for (; i <= (nuint)(dest.Length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                var x = Vector512.LoadUnsafe(ref pSource, i);
                var local = Vector512.Abs(x);
                Vector512.StoreUnsafe(local, ref pDest, i);
            }

            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var x = Vector256.LoadUnsafe(ref pSource, i);
                var local = Vector256.Abs(x);
                Vector256.StoreUnsafe(local, ref pDest, i);
                i += (nuint)Vector256<float>.Count;
            }

        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            for (; i <= (nuint)(dest.Length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var x = Vector256.LoadUnsafe(ref pSource, i);
                var local = Vector256.Abs(x);
                Vector256.StoreUnsafe(local, ref pDest, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref pDest, i) = MathF.Abs(Unsafe.Add(ref pSource, i));
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Abs(float[] dest, float[] source)
    {
        Abs(new Span<float>(dest), new Span<float>(source));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Abs(float[][] dest, float[][] source)
    {
        for (int row = 0; row < dest.Length; row++)
        {
            Abs(dest[row], source[row]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MultiplyAndSum(Span<float> dest, Span<float> first, Span<float> second)
    {
        EnsureSameSize(dest, first, second);
        var length = dest.Length;
        ref var rd = ref MemoryMarshal.GetReference(dest);
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        nuint i = 0;
        float acc = 0.0f;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> acc1 = Vector512<float>.Zero;
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var result = (f * s);
                Vector512.StoreUnsafe(result, ref rd, i);
                acc1 += result;
            }
            acc += Vector512.Sum(acc1);
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = f * s;
                Vector256.StoreUnsafe(result, ref rd, i);
                acc += Vector256.Sum(result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> acc1 = Vector256<float>.Zero;
            Vector256<float> acc2 = Vector256<float>.Zero;
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var result1 = f * s;
                var result2 = f2 * s2;
                acc1 += result1;
                acc2 += result2;
                Vector256.StoreUnsafe(result1, ref rd, i);
                Vector256.StoreUnsafe(result2, ref rd, i + (nuint)Vector256<float>.Count);
            }
            acc += Vector256.Sum(acc1 + acc2);
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = f * s;
                acc += Vector256.Sum(result);
                Vector256.StoreUnsafe(result, ref rd, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = Unsafe.Add(ref rf, i) * Unsafe.Add(ref rs, i);
            Unsafe.Add(ref rd, i) = result;
            acc += result;
        }
        return acc;
    }

    /// <summary>
    /// Multiply the two vectors and store the results in the destination.  Return a running sum.
    /// </summary>
    /// <param name="destination">Where to save the data</param>
    /// <param name="destIndex">What index to start at</param>
    /// <param name="first">The first array to multiply</param>
    /// <param name="firstIndex">The index to start at</param>
    /// <param name="second">The second array to multiply</param>
    /// <param name="secondIndex">The index to start at for the second array</param>
    /// <param name="length">The amount of data to multiply</param>
    /// <returns>The sum of all of the multiplies</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MultiplyAndSum(float[] destination, int destIndex, float[] first, int firstIndex,
        float[] second, int secondIndex, int length)
    {
        return MultiplyAndSum(new Span<float>(destination, destIndex, length),
            new Span<float>(first, firstIndex, length),
            new Span<float>(second, secondIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MultiplyAndSum(Span<float> first, Span<float> second)
    {
        EnsureSameSize(first, second);
        var length = first.Length;
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        nuint i = 0;
        float acc = 0.0f;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> acc1 = Vector512<float>.Zero;
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var result = (f * s);
                acc1 += result;
            }
            acc += Vector512.Sum(acc1);
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = f * s;
                acc += Vector256.Sum(result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> acc1 = Vector256<float>.Zero;
            Vector256<float> acc2 = Vector256<float>.Zero;
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var result1 = f * s;
                var result2 = f2 * s2;
                acc1 += result1;
                acc2 += result2;
            }
            acc += Vector256.Sum(acc1 + acc2);
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = f * s;
                acc += Vector256.Sum(result);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = Unsafe.Add(ref rf, i) * Unsafe.Add(ref rs, i);
            acc += result;
        }
        return acc;
    }

    /// <summary>
    /// Multiply the two vectors without storing the results but returning the total.
    /// </summary>
    /// <param name="first">The first array to multiply</param>
    /// <param name="firstIndex">The index to start at</param>
    /// <param name="second">The second array to multiply</param>
    /// <param name="secondIndex">The index to start at for the second array</param>
    /// <param name="length">The amount of data to multiply</param>
    /// <returns>The sum of all of the multiplies</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MultiplyAndSum(float[] first, int firstIndex, float[] second, int secondIndex, int length)
    {
        return MultiplyAndSum(new Span<float>(first, firstIndex, length),
            new Span<float>(second, secondIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Multiply3AndSum(Span<float> first, Span<float> second, Span<float> third)
    {
        EnsureSameSize(first, second, third);
        var length = first.Length;
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        ref var rt = ref MemoryMarshal.GetReference(third);
        nuint i = 0;
        float acc = 0.0f;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> acc1 = Vector512<float>.Zero;
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var t = Vector512.LoadUnsafe(ref rt, i);
                acc1 += (f * s * t);
            }
            acc += Vector512.Sum(acc1);
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var t = Vector256.LoadUnsafe(ref rt, i);
                acc += Vector256.Sum(f * s * t);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> acc1 = Vector256<float>.Zero;
            Vector256<float> acc2 = Vector256<float>.Zero;
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var t = Vector256.LoadUnsafe(ref rt, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var t2 = Vector256.LoadUnsafe(ref rt, i + (nuint)Vector256<float>.Count);
                acc1 += (f * s * t);
                acc2 += (f2 * s2 * t2);
            }
            acc += Vector256.Sum(acc1 + acc2);
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var t = Vector256.LoadUnsafe(ref rt, i);
                acc += Vector256.Sum(f * s * t);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            acc += Unsafe.Add(ref rf, i) * Unsafe.Add(ref rs, i) * Unsafe.Add(ref rt, i);
        }
        return acc;
    }

    /// <summary>
    /// Multiply the two vectors without storing the results but returning the total.
    /// </summary>
    /// <param name="first">The first array to multiply</param>
    /// <param name="firstIndex">The index to start at</param>
    /// <param name="second">The second array to multiply</param>
    /// <param name="secondIndex">The index to start at for the second array</param>
    /// <param name="thirdIndex"></param>
    /// <param name="length">The amount of data to multiply</param>
    /// <param name="third"></param>
    /// <returns>The sum of all of the multiplies</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Multiply3AndSum(float[] first, int firstIndex, float[] second, int secondIndex,
        float[] third, int thirdIndex, int length)
    {
        return Multiply3AndSum(new Span<float>(first, firstIndex, length),
                    new Span<float>(second, secondIndex, length),
                    new Span<float>(third, thirdIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply2Scalar1AndColumnSum(Span<float> dest, Span<float> first,
        Span<float> second, float scalar, Span<float> columnSum)
    {
        EnsureSameSize(dest, first, second, columnSum);
        var length = dest.Length;
        ref var rd = ref MemoryMarshal.GetReference(dest);
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        ref var rc = ref MemoryMarshal.GetReference(columnSum);
        nuint i = 0;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> t = Vector512.Create(scalar);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var c = Vector512.LoadUnsafe(ref rc, i);
                Vector512<float> result = (f * s * t);
                Vector512.StoreUnsafe(result, ref rd, i);
                Vector512.StoreUnsafe(c + result, ref rc, i);
            }
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var c = Vector256.LoadUnsafe(ref rc, i);
                var result = f * s * t.GetLower();
                Vector256.StoreUnsafe(result, ref rd, i);
                Vector256.StoreUnsafe(c + result, ref rc, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> t = Vector256.Create(scalar);
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var c = Vector256.LoadUnsafe(ref rc, i);
                var c2 = Vector256.LoadUnsafe(ref rc, i + (nuint)Vector256<float>.Count);
                var result1 = f * s * t;
                var result2 = f2 * s2 * t;
                Vector256.StoreUnsafe(result1, ref rd, i);
                Vector256.StoreUnsafe(result2, ref rd, i + (nuint)Vector256<float>.Count);
                Vector256.StoreUnsafe(c + result1, ref rc, i);
                Vector256.StoreUnsafe(c2 + result2, ref rc, i + (nuint)Vector256<float>.Count);
            }
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var c = Vector256.LoadUnsafe(ref rc, i);
                var result = f * s * t;
                Vector256.StoreUnsafe(result, ref rd, i);
                Vector256.StoreUnsafe(c + result, ref rc, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = Unsafe.Add(ref rf, i) * Unsafe.Add(ref rs, i) * scalar;
            Unsafe.Add(ref rd, i) = result;
            Unsafe.Add(ref rc, i) += result;
        }
    }

    /// <summary>
    /// Multiply the two vectors and store the results in the destination.  Return a running sum.
    /// </summary>
    /// <param name="destination">Where to save the data</param>
    /// <param name="destIndex">What index to start at</param>
    /// <param name="first">The first array to multiply</param>
    /// <param name="firstIndex">The index to start at</param>
    /// <param name="second">The second array to multiply</param>
    /// <param name="secondIndex">The index to start at for the second array</param>
    /// <param name="columnIndex"></param>
    /// <param name="length">The amount of data to multiply</param>
    /// <param name="scalar"></param>
    /// <param name="columnSum"></param>
    /// <returns>The sum of all of the multiplies</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply2Scalar1AndColumnSum(float[] destination, int destIndex, float[] first, int firstIndex,
        float[] second, int secondIndex, float scalar, float[] columnSum, int columnIndex, int length)
    {
        Multiply2Scalar1AndColumnSum(new Span<float>(destination, destIndex, length),
            new Span<float>(first, firstIndex, length),
            new Span<float>(second, secondIndex, length),
            scalar,
            new Span<float>(columnSum, columnIndex, length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply3Scalar1AndColumnSum(Span<float> dest, Span<float> first,
        Span<float> second, Span<float> third, float scalar, Span<float> columnSum)
    {
        EnsureSameSize(dest, first, second, third, columnSum);
        var length = dest.Length;
        ref var rd = ref MemoryMarshal.GetReference(dest);
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        ref var rt = ref MemoryMarshal.GetReference(third);
        ref var rc = ref MemoryMarshal.GetReference(columnSum);
        nuint i = 0;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> vScalar = Vector512.Create(scalar);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                var t = Vector512.LoadUnsafe(ref rt, i);
                var c = Vector512.LoadUnsafe(ref rc, i);
                Vector512<float> result = (f * s) * (t * vScalar);
                Vector512.StoreUnsafe(result, ref rd, i);
                Vector512.StoreUnsafe(c + result, ref rc, i);
            }
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var t = Vector256.LoadUnsafe(ref rt, i);
                var c = Vector256.LoadUnsafe(ref rc, i);
                var result = (f * s) * (t * vScalar.GetLower());
                Vector256.StoreUnsafe(result, ref rd, i);
                Vector256.StoreUnsafe(c + result, ref rc, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> vScalar = Vector256.Create(scalar);
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var t = Vector256.LoadUnsafe(ref rt, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var t2 = Vector256.LoadUnsafe(ref rt, i + (nuint)Vector256<float>.Count);
                var c = Vector256.LoadUnsafe(ref rc, i);
                var c2 = Vector256.LoadUnsafe(ref rc, i + (nuint)Vector256<float>.Count);
                var result1 = (f * s) * (t * vScalar);
                var result2 = (f2 * s2) * (t2 * vScalar);
                Vector256.StoreUnsafe(result1, ref rd, i);
                Vector256.StoreUnsafe(result2, ref rd, i + (nuint)Vector256<float>.Count);
                Vector256.StoreUnsafe(c + result1, ref rc, i);
                Vector256.StoreUnsafe(c2 + result2, ref rc, i + (nuint)Vector256<float>.Count);
            }
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var t = Vector256.LoadUnsafe(ref rt, i);
                var c = Vector256.LoadUnsafe(ref rc, i);
                var result = (f * s) * (t * vScalar);
                Vector256.StoreUnsafe(result, ref rd, i);
                Vector256.StoreUnsafe(c + result, ref rc, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = Unsafe.Add(ref rf, i) * Unsafe.Add(ref rs, i) * Unsafe.Add(ref rt, i) * scalar;
            Unsafe.Add(ref rd, i) = result;
            Unsafe.Add(ref rc, i) += result;
        }
    }

    /// <summary>
    /// Multiply the two vectors and store the results in the destination.  Return a running sum.
    /// </summary>
    /// <param name="destination">Where to save the data</param>
    /// <param name="destIndex">What index to start at</param>
    /// <param name="first">The first array to multiply</param>
    /// <param name="firstIndex">The index to start at</param>
    /// <param name="second">The second array to multiply</param>
    /// <param name="secondIndex">The index to start at for the second array</param>
    /// <param name="columnIndex"></param>
    /// <param name="length">The amount of data to multiply</param>
    /// <param name="third"></param>
    /// <param name="thirdIndex"></param>
    /// <param name="scalar"></param>
    /// <param name="columnSum"></param>
    /// <returns>The sum of all of the multiplies</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply3Scalar1AndColumnSum(float[] destination, int destIndex, float[] first, int firstIndex,
        float[] second, int secondIndex, float[] third, int thirdIndex, float scalar, float[] columnSum, int columnIndex, int length)
    {
        Multiply3Scalar1AndColumnSum(new Span<float>(destination, destIndex, length),
            new Span<float>(first, firstIndex, length),
            new Span<float>(second, secondIndex, length),
            new Span<float>(third, thirdIndex, length),
            scalar,
            new Span<float>(columnSum, columnIndex, length));
    }

    public static void Average(Span<float> dest, Span<float> first, Span<float> second)
    {
        EnsureSameSize(dest, first, second);
        var length = dest.Length;
        ref var rd = ref MemoryMarshal.GetReference(dest);
        ref var rf = ref MemoryMarshal.GetReference(first);
        ref var rs = ref MemoryMarshal.GetReference(second);
        nuint i = 0;
        // 16 floats per Vector512, we hard code this here just in case Vector512 is not supported
        var end = (nuint)(length - 16);
        if (Vector512.IsHardwareAccelerated && length >= 16)
        {
            Vector512<float> half = Vector512.Create(0.5f);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var f = Vector512.LoadUnsafe(ref rf, i);
                var s = Vector512.LoadUnsafe(ref rs, i);
                Vector512<float> result = (f * half) + (s * half);
                Vector512.StoreUnsafe(result, ref rd, i);
            }
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var half256 = Vector256.Create(0.5f);
                var result = (f * half256) + (s * half256);
                Vector256.StoreUnsafe(result, ref rd, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= 16)
        {
            // Vector256 needs to be doubled to match the same results as Vector512
            Vector256<float> half = Vector256.Create(0.5f);
            for (; i <= end; i += (nuint)(Vector256<float>.Count * 2))
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var f2 = Vector256.LoadUnsafe(ref rf, i + (nuint)Vector256<float>.Count);
                var s2 = Vector256.LoadUnsafe(ref rs, i + (nuint)Vector256<float>.Count);
                var result1 = (f * half) + (s * half);
                var result2 = (f2 * half) + (s2 * half);
                Vector256.StoreUnsafe(result1, ref rd, i);
                Vector256.StoreUnsafe(result2, ref rd, i + (nuint)Vector256<float>.Count);
            }
            // If there is one more Vector256 left, add it in
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var f = Vector256.LoadUnsafe(ref rf, i);
                var s = Vector256.LoadUnsafe(ref rs, i);
                var result = (f * half) + (s * half);
                Vector256.StoreUnsafe(result, ref rd, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Add the remainder
        for (; i < (nuint)length; i++)
        {
            var result = (Unsafe.Add(ref rf, i) * 0.5f) + (Unsafe.Add(ref rs, i) * 0.5f);
            Unsafe.Add(ref rd, i) = result;
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
    public static void Average(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
    {
        Average(new Span<float>(destination, destIndex, length),
            new Span<float>(first, firstIndex, length),
            new Span<float>(second, secondIndex, length));
    }

    /// <summary>
    /// Produce a new vector selecting the original value if it is finite.  If it is not,
    /// select the alternative value.
    /// </summary>
    /// <param name="baseValues">The values to test for their finite property</param>
    /// <param name="alternateValues">The values to replace if the base value is not finite</param>
    /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> SelectIfFinite(Vector512<float> baseValues, Vector512<float> alternateValues)
    {
        //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
        return Vector512.ConditionalSelect(
            Vector512.BitwiseAnd(Vector512.LessThanOrEqual(Vector512.Abs(baseValues), MaxFloat512), Vector512.GreaterThanOrEqual(baseValues, baseValues)),
            baseValues, alternateValues
            );
    }

    /// <summary>
    /// Produce a new vector selecting the original value if it is finite.  If it is not,
    /// select the alternative value.
    /// </summary>
    /// <param name="baseValues">The values to test for their finite property</param>
    /// <param name="alternateValues">The values to replace if the base value is not finite</param>
    /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> SelectIfFinite(Vector256<float> baseValues, Vector256<float> alternateValues)
    {
        //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
        return Vector256.ConditionalSelect(
            Vector256.BitwiseAnd(Vector256.LessThanOrEqual(Vector256.Abs(baseValues), MaxFloat256), Vector256.GreaterThanOrEqual(baseValues, baseValues)),
            baseValues, alternateValues
            );
    }

    /// <summary>
    /// Produce a new vector selecting the original value if it is finite.  If it is not,
    /// select the alternative value.
    /// </summary>
    /// <param name="baseValues">The values to test for their finite property</param>
    /// <param name="alternateValues">The values to replace if the base value is not finite</param>
    /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector<float> SelectIfFinite(Vector<float> baseValues, Vector<float> alternateValues)
    {
        //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
        return System.Numerics.Vector.ConditionalSelect(
            System.Numerics.Vector.BitwiseAnd(System.Numerics.Vector.LessThanOrEqual(System.Numerics.Vector.Abs(baseValues), MaxFloat), System.Numerics.Vector.GreaterThanOrEqual(baseValues, baseValues)),
            baseValues, alternateValues
            );
    }

    /// <summary>
    /// Produce a new vector selecting the original value if it is finite.  If it is not,
    /// select the alternative value.
    /// </summary>
    /// <param name="baseValues">The values to test for their finite property</param>
    /// <param name="alternateValues">The values to replace if the base value is not finite</param>
    /// <param name="minimumV"></param>
    /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> SelectIfFiniteAndLessThan(Vector512<float> baseValues, Vector512<float> alternateValues, Vector512<float> minimumV)
    {
        //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
        return Vector512.ConditionalSelect(
            Vector512.BitwiseAnd(Vector512.BitwiseAnd(Vector512.LessThanOrEqual(Vector512.Abs(baseValues), MaxFloat512),
            Vector512.GreaterThanOrEqual(baseValues, baseValues)), Vector512.GreaterThanOrEqual(baseValues, minimumV)),
            baseValues, alternateValues
            );
    }

    /// <summary>
    /// Produce a new vector selecting the original value if it is finite.  If it is not,
    /// select the alternative value.
    /// </summary>
    /// <param name="baseValues">The values to test for their finite property</param>
    /// <param name="alternateValues">The values to replace if the base value is not finite</param>
    /// <param name="minimumV"></param>
    /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> SelectIfFiniteAndLessThan(Vector256<float> baseValues, Vector256<float> alternateValues, Vector256<float> minimumV)
    {
        //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
        return Vector256.ConditionalSelect(
            Vector256.BitwiseAnd(Vector256.BitwiseAnd(Vector256.LessThanOrEqual(Vector256.Abs(baseValues), MaxFloat256),
            Vector256.GreaterThanOrEqual(baseValues, baseValues)), Vector256.GreaterThanOrEqual(baseValues, minimumV)),
            baseValues, alternateValues
            );
    }

    /// <summary>
    /// Produce a new vector selecting the original value if it is finite.  If it is not,
    /// select the alternative value.
    /// </summary>
    /// <param name="baseValues">The values to test for their finite property</param>
    /// <param name="alternateValues">The values to replace if the base value is not finite</param>
    /// <param name="minimumV"></param>
    /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector<float> SelectIfFiniteAndLessThan(Vector<float> baseValues, Vector<float> alternateValues, Vector<float> minimumV)
    {
        //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
        return System.Numerics.Vector.ConditionalSelect(
            System.Numerics.Vector.BitwiseAnd(System.Numerics.Vector.BitwiseAnd(System.Numerics.Vector.LessThanOrEqual(System.Numerics.Vector.Abs(baseValues), MaxFloat),
            System.Numerics.Vector.GreaterThanOrEqual(baseValues, baseValues)), System.Numerics.Vector.GreaterThanOrEqual(baseValues, minimumV)),
            baseValues, alternateValues
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNotFinite(Span<float> dest, float alternateValue)
    {
        var length = dest.Length;
        ref var destination = ref MemoryMarshal.GetReference(dest);
        nuint i = 0;
        if (Vector512.IsHardwareAccelerated && length >= Vector512<float>.Count)
        {
            var altV = Vector512.Create(alternateValue);
            for (; i <= (nuint)(length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                Vector512.StoreUnsafe(SelectIfFinite(Vector512.LoadUnsafe(ref destination, i), altV), ref destination, i);
            }

            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var current = Vector256.LoadUnsafe(ref destination, i);
                Vector256.StoreUnsafe(SelectIfFinite(current, altV.GetLower()), ref destination, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= Vector256<float>.Count)
        {
            var altV = Vector256.Create(alternateValue);
            for (; i <= (nuint)(length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var current = Vector256.LoadUnsafe(ref destination, i);
                Vector256.StoreUnsafe(SelectIfFinite(current, altV), ref destination, i);
            }
        }

        for (; i < (nuint)length; i++)
        {
            var value = Unsafe.Add(ref destination, i);
            if (!float.IsFinite(value))
            {
                Unsafe.Add(ref destination, i) = alternateValue;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="destIndex"></param>
    /// <param name="alternateValue"></param>
    /// <param name="length"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNotFinite(float[] destination, int destIndex, float alternateValue, int length)
    {
        ReplaceIfNotFinite(new Span<float>(destination, destIndex, length), alternateValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfLessThanOrNotFinite(Span<float> dest, float alternateValue, float minimum)
    {
        var length = dest.Length;
        ref var destination = ref MemoryMarshal.GetReference(dest);
        nuint i = 0;
        if (Vector512.IsHardwareAccelerated && length >= Vector512<float>.Count)
        {
            var altV = Vector512.Create(alternateValue);
            var minV = Vector512.Create(minimum);
            for (; i <= (nuint)(length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                Vector512.StoreUnsafe(SelectIfFiniteAndLessThan(Vector512.LoadUnsafe(ref destination, i), altV, minV), ref destination, i);
            }

            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var current = Vector256.LoadUnsafe(ref destination, i);
                Vector256.StoreUnsafe(SelectIfFiniteAndLessThan(current, altV.GetLower(), minV.GetLower()), ref destination, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= Vector256<float>.Count)
        {
            var altV = Vector256.Create(alternateValue);
            var minV = Vector256.Create(minimum);
            for (; i <= (nuint)(length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var current = Vector256.LoadUnsafe(ref destination, i);
                Vector256.StoreUnsafe(SelectIfFiniteAndLessThan(current, altV, minV), ref destination, i);
            }
        }

        for (; i < (nuint)length; i++)
        {
            var value = Unsafe.Add(ref destination, i);
            if (!float.IsFinite(value) || value < minimum)
            {
                Unsafe.Add(ref destination, i) = alternateValue;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfLessThanOrNotFinite(float[] destination, int destIndex, float alternateValue, float minimum, int length)
    {
        ReplaceIfLessThanOrNotFinite(new Span<float>(destination, destIndex, length), alternateValue, minimum);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyGreaterThan(Span<float> data, float rhs)
    {
        var rhsV = new Vector<float>(rhs);
        var remainder = data.Length % Vector<float>.Count;
        var vectorData = (data.Slice(0, data.Length - remainder)).NonPortableCast<float, Vector<float>>();
        int i = 0;
        for (; i < vectorData.Length - 1; i += 2)
        {
            if (System.Numerics.Vector.GreaterThanAny(vectorData[i], rhsV)
                | System.Numerics.Vector.GreaterThanAny(vectorData[i + 1], rhsV))
            {
                return true;
            }
        }
        i *= Vector<float>.Count;
        for (; i < data.Length; i++)
        {
            if (data[i] > rhs)
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyGreaterThan(float[] data, int dataIndex, float rhs, int length)
    {
        return AnyGreaterThan(new Span<float>(data, dataIndex, length), rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreBoundedBy(Span<float> data, float baseNumber, float maxVarriation)
    {
        var baseV = new Vector<float>(baseNumber);
        var maxmumVariationV = new Vector<float>(maxVarriation);
        var remainder = data.Length % Vector<float>.Count;
        var vectorData = (data.Slice(0, data.Length - remainder)).NonPortableCast<float, Vector<float>>();
        int i = 0;
        for (; i < vectorData.Length - 1; i += 2)
        {
            if (System.Numerics.Vector.GreaterThanAny(System.Numerics.Vector.Abs(vectorData[i] - baseV), maxmumVariationV)
                | System.Numerics.Vector.GreaterThanAny(System.Numerics.Vector.Abs(vectorData[i + 1] - baseV), maxmumVariationV))
            {
                return true;
            }
        }
        i *= Vector<float>.Count;
        for (; i < data.Length; i++)
        {
            if (Math.Abs(data[i] - baseNumber) > maxVarriation)
            {
                return true;
            }
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreBoundedBy(float[] data, int dataIndex, float baseNumber, float maxVarriation, int length)
    {
        return AreBoundedBy(new Span<float>(data, dataIndex, length), baseNumber, maxVarriation);
    }

    internal static void ReplaceIfNaN(Span<float> dest, Span<float> source, Span<float> replacement)
    {
        if (dest.Length != source.Length || dest.Length != replacement.Length)
        {
            throw new ArgumentException("All spans must be the same length");
        }
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pSource = ref MemoryMarshal.GetReference(source);
        ref var pReplacement = ref MemoryMarshal.GetReference(replacement);

        nuint length = (nuint)dest.Length;
        nuint i = 0;
        if (Vector512.IsHardwareAccelerated && length >= (nuint)Vector512<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector512<float>.Count);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vSource = Vector512.LoadUnsafe(ref pSource, i);
                var vReplace = Vector512.LoadUnsafe(ref pReplacement, i);
                var result = SelectIfFinite(vSource, vReplace);
                Vector512.StoreUnsafe(result, ref pDest, i);
            }
            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var vSource = Vector256.LoadUnsafe(ref pSource, i);
                var vReplace = Vector256.LoadUnsafe(ref pReplacement, i);
                var result = SelectIfFinite(vSource, vReplace);
                Vector256.StoreUnsafe(result, ref pDest, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<float>.Count)
        {
            var end = (nuint)(dest.Length - Vector256<float>.Count);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vSource = Vector256.LoadUnsafe(ref pSource, i);
                var vReplace = Vector256.LoadUnsafe(ref pReplacement, i);
                var result = SelectIfFinite(vSource, vReplace);
                Vector256.StoreUnsafe(result, ref pDest, i);
            }
        }
        
        // Process the remainder
        for (; i < length; i++)
        {
            dest[(int)i] = !float.IsNaN(source[(int)i]) ? source[(int)i] : replacement[(int)i];
        }
        
    }

    internal static void ReplaceIfNaN(float[] dest, float[] source, float[] replacement, int offset, int length)
    {
        if (dest == null || source == null || replacement == null)
        {
            throw new ArgumentNullException();
        }
        var remainder = length % Vector<float>.Count;
        var destSpan = (new Span<float>(dest, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
        var sourceSpan = (new Span<float>(source, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
        var replacementSpan = (new Span<float>(replacement, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
        int i = 0;
        for (; i < destSpan.Length - 1; i += 2)
        {
            destSpan[i] = System.Numerics.Vector.ConditionalSelect(System.Numerics.Vector.GreaterThanOrEqual(sourceSpan[i], sourceSpan[i]), sourceSpan[i], replacementSpan[i]);
            destSpan[i + 1] = System.Numerics.Vector.ConditionalSelect(System.Numerics.Vector.GreaterThanOrEqual(sourceSpan[i + 1], sourceSpan[i + 1]), sourceSpan[i + 1], replacementSpan[i + 1]);
        }
        i *= Vector<float>.Count;
        for (; i < length; i++)
        {
            dest[offset + i] = !float.IsNaN(source[offset + i]) ? source[offset + i] : replacement[offset + i];
        }
    }

    public static void Negate(float[] dest, float[] source, int offset, int length)
    {
        if (dest == null || source == null)
        {
            throw new ArgumentNullException();
        }
        var remainder = length % Vector<float>.Count;
        var destSpan = (new Span<float>(dest, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
        var sourceSpan = (new Span<float>(source, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
        int i = 0;
        for (; i < destSpan.Length - 1; i += 2)
        {
            destSpan[i] = System.Numerics.Vector.Negate(sourceSpan[i]);
            destSpan[i + 1] = System.Numerics.Vector.Negate(sourceSpan[i + 1]);
        }
        i *= Vector<float>.Count;
        for (; i < length; i++)
        {
            dest[offset + i] = -source[offset + i];
        }
    }

    public static void Negate(Span<float> dest, Span<float> source)
    {
        if (dest.Length != source.Length)
        {
            throw new ArgumentException("Both spans must be the same length");
        }
        nuint i = 0;
        nuint length = (nuint)dest.Length;
        ref var pDest = ref MemoryMarshal.GetReference(dest);
        ref var pSource = ref MemoryMarshal.GetReference(source);

        if (Vector512.IsHardwareAccelerated && length >= (nuint)Vector512<float>.Count)
        {
            for (; i <= length - (nuint)Vector512<float>.Count; i += (nuint)Vector512<float>.Count)
            {
                var vSource = Vector512.LoadUnsafe(ref pSource, i);
                var result = Vector512.Negate(vSource);
                Vector512.StoreUnsafe(result, ref pDest, i);
            }

            if ((nuint)length - i >= (nuint)Vector256<float>.Count)
            {
                var vSource = Vector256.LoadUnsafe(ref pSource, i);
                var result = Vector256.Negate(vSource);
                Vector256.StoreUnsafe(result, ref pDest, i);
                i += (nuint)Vector256<float>.Count;
            }    
        }
        else if(Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<float>.Count)
        {
            for (; i <= length - (nuint)Vector256<float>.Count; i += (nuint)Vector256<float>.Count)
            {
                var vSource = Vector256.LoadUnsafe(ref pSource, i);
                var result = Vector256.Negate(vSource);
                Vector256.StoreUnsafe(result, ref pDest, i);
            }
        }

        // Process the remaining data.
        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref pDest, i) = -Unsafe.Add(ref pSource, i);
        }
    }

    /// <summary>
    /// Is the mask is 1, then the RHS is selected LHS if 0.
    /// </summary>
    /// <param name="lhs">The values to select if the LHS was selected.</param>
    /// <param name="rhs">The values to select if the RHS was selected.</param>
    /// <param name="mask">0 to select the LHS, 1 to select the RHS.</param>
    /// <returns>A new vector with the selected elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector512<float> Blend(Vector512<float> lhs, Vector512<float> rhs, Vector512<float> mask)
    {
        if (Avx512F.IsSupported)
        {
            return Avx512F.BlendVariable(lhs, rhs, mask);
        }
        else
        {
            var invMask = Vector512.OnesComplement(mask);
            return Vector512.BitwiseOr(Vector512.BitwiseAnd(lhs, invMask),
                Vector512.BitwiseAnd(rhs, mask));
        }
    }

    /// <summary>
    /// Is the mask is 1, then the RHS is selected LHS if 0.
    /// </summary>
    /// <param name="lhs">The values to select if the LHS was selected.</param>
    /// <param name="rhs">The values to select if the RHS was selected.</param>
    /// <param name="mask">0 to select the LHS, 1 to select the RHS.</param>
    /// <returns>A new vector with the selected elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector512<double> Blend(Vector512<double> lhs, Vector512<double> rhs, Vector512<double> mask)
    {
        if (Avx512F.IsSupported)
        {
            return Avx512F.BlendVariable(lhs, rhs, mask);
        }
        else
        {
            var invMask = Vector512.OnesComplement(mask);
            return Vector512.BitwiseOr(Vector512.BitwiseAnd(lhs, invMask),
                Vector512.BitwiseAnd(rhs, mask));
        }
    }

    /// <summary>
    /// Is the mask is 1, then the RHS is selected LHS if 0.
    /// </summary>
    /// <param name="lhs">The values to select if the LHS was selected.</param>
    /// <param name="rhs">The values to select if the RHS was selected.</param>
    /// <param name="mask">0 to select the LHS, 1 to select the RHS.</param>
    /// <returns>A new vector with the selected elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector256<float> Blend(Vector256<float> lhs, Vector256<float> rhs, Vector256<float> mask)
    {
        if (Avx.IsSupported)
        {
            return Avx.BlendVariable(lhs, rhs, mask);
        }
        else
        {
            var invMask = Vector256.OnesComplement(mask);
            return Vector256.BitwiseOr(Vector256.BitwiseAnd(lhs, invMask),
                Vector256.BitwiseAnd(rhs, mask));
        }
    }

    /// <summary>
    /// Is the mask is 1, then the RHS is selected LHS if 0.
    /// </summary>
    /// <param name="lhs">The values to select if the LHS was selected.</param>
    /// <param name="rhs">The values to select if the RHS was selected.</param>
    /// <param name="mask">0 to select the LHS, 1 to select the RHS.</param>
    /// <returns>A new vector with the selected elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector256<double> Blend(Vector256<double> lhs, Vector256<double> rhs, Vector256<double> mask)
    {
        if (Avx.IsSupported)
        {
            return Avx.BlendVariable(lhs, rhs, mask);
        }
        else
        {
            var invMask = Vector256.OnesComplement(mask);
            return Vector256.BitwiseOr(Vector256.BitwiseAnd(lhs, invMask),
                Vector256.BitwiseAnd(rhs, mask));
        }
    }

    /// <summary>
    /// Convert a single precision vector into two double precision vectors
    /// </summary>
    /// <param name="x">The single precision vector to convert</param>
    /// <returns>The low and high positioned double precision vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static (Vector512<double> low, Vector512<double> high) ConvertToDouble(Vector512<float> x)
    {
        Vector256<float> low = x.GetLower();
        Vector256<float> high = x.GetUpper();
        Vector512<double> lowD;
        Vector512<double> highD;
        if (Avx512F.IsSupported)
        {
            lowD = Avx512F.ConvertToVector512Double(low);
            highD = Avx512F.ConvertToVector512Double(high);
        }
        else
        {
            lowD = Vector512.Create((double)low.GetElement(0), (double)low.GetElement(1), (double)low.GetElement(2), (double)low.GetElement(3),
                (double)low.GetElement(4), (double)low.GetElement(5), (double)low.GetElement(6), (double)low.GetElement(7));
            highD = Vector512.Create((double)high.GetElement(0), (double)high.GetElement(1), (double)high.GetElement(2), (double)high.GetElement(3),
                (double)high.GetElement(4), (double)high.GetElement(5), (double)high.GetElement(6), (double)high.GetElement(7));
        }
        return (lowD, highD);
    }

    /// <summary>
    /// Convert a single precision vector into two double precision vectors
    /// </summary>
    /// <param name="x">The single precision vector to convert</param>
    /// <returns>The low and high positioned double precision vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static (Vector256<double> low, Vector256<double> high) ConvertToDouble(Vector256<float> x)
    {
        Vector128<float> low = x.GetLower();
        Vector128<float> high = x.GetUpper();
        Vector256<double> lowD;
        Vector256<double> highD;
        if (Avx512F.IsSupported)
        {
            lowD = Avx512F.ConvertToVector256Double(low);
            highD = Avx512F.ConvertToVector256Double(high);
        }
        else
        {
            lowD = Vector256.Create((double)low.GetElement(0), (double)low.GetElement(1), (double)low.GetElement(2), (double)low.GetElement(3));

            highD = Vector256.Create((double)high.GetElement(0), (double)high.GetElement(1), (double)high.GetElement(2), (double)high.GetElement(3));
        }
        return (lowD, highD);
    }

    /// <summary>
    /// Converts two double precision vectors into a single precision vector
    /// </summary>
    /// <param name="lowD">The lower half of the double precision vector</param>
    /// <param name="highD">The upper half of the double precision vector</param>
    /// <returns>A single precision vector with the combined lower and upper halves</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector512<float> ConvertToFloat(Vector512<double> lowD, Vector512<double> highD)
    {
        Vector512<float> result;
        if (Avx512F.IsSupported)
        {
            result = Vector512.Create(Avx512F.ConvertToVector256Single(lowD), Avx512F.ConvertToVector256Single(highD));
        }
        else
        {
            var low = Vector256.Create((float)lowD.GetElement(0), (float)lowD.GetElement(1), (float)lowD.GetElement(2), (float)lowD.GetElement(3),
                (float)lowD.GetElement(4), (float)lowD.GetElement(5), (float)lowD.GetElement(6), (float)lowD.GetElement(7));
            var high = Vector256.Create((float)highD.GetElement(0), (float)highD.GetElement(1), (float)highD.GetElement(2), (float)highD.GetElement(3),
                (float)highD.GetElement(4), (float)highD.GetElement(5), (float)highD.GetElement(6), (float)highD.GetElement(7));
            result = Vector512.Create(low, high);
        }
        return result;
    }


    /// <summary>
    /// Converts two double precision vectors into a single precision vector
    /// </summary>
    /// <param name="lowD">The lower half of the double precision vector</param>
    /// <param name="highD">The upper half of the double precision vector</param>
    /// <returns>A single precision vector with the combined lower and upper halves</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector256<float> ConvertToFloat(Vector256<double> lowD, Vector256<double> highD)
    {
        Vector256<float> result;
        if (Avx512F.IsSupported)
        {
            result = Vector256.Create(Avx512F.ConvertToVector128Single(lowD), Avx512F.ConvertToVector128Single(highD));
        }
        else
        {
            var low = Vector128.Create((float)lowD.GetElement(0), (float)lowD.GetElement(1), (float)lowD.GetElement(2), (float)lowD.GetElement(3));
            var high = Vector128.Create((float)highD.GetElement(0), (float)highD.GetElement(1), (float)highD.GetElement(2), (float)highD.GetElement(3));
            result = Vector256.Create(low, high);
        }
        return result;
    }

    [DoesNotReturn]
    private static void ThrowNotSameSize()
    {
        throw new ArgumentException("The length of the parameters are not the same!");
    }

    private static bool EnsureSameSize(ReadOnlySpan<float> first, ReadOnlySpan<float> second)
    {
        if (first.Length != second.Length)
        {
            ThrowNotSameSize();
        }
        return true;
    }

    private static bool EnsureSameSize(ReadOnlySpan<float> first, ReadOnlySpan<float> second, ReadOnlySpan<float> third)
    {
        if (first.Length != second.Length)
        {
            ThrowNotSameSize();
        }
        if (first.Length != third.Length)
        {
            ThrowNotSameSize();
        }
        return true;
    }


    private static bool EnsureSameSize(ReadOnlySpan<float> first, ReadOnlySpan<float> second, ReadOnlySpan<float> third, ReadOnlySpan<float> fourth)
    {
        if (first.Length != second.Length)
        {
            ThrowNotSameSize();
        }
        if (first.Length != third.Length)
        {
            ThrowNotSameSize();
        }
        if (first.Length != fourth.Length)
        {
            ThrowNotSameSize();
        }
        return true;
    }

    private static bool EnsureSameSize(ReadOnlySpan<float> first, ReadOnlySpan<float> second, ReadOnlySpan<float> third, ReadOnlySpan<float> fourth
        , ReadOnlySpan<float> fifth)
    {
        if (first.Length != second.Length)
        {
            ThrowNotSameSize();
        }
        if (first.Length != third.Length)
        {
            ThrowNotSameSize();
        }
        if (first.Length != fourth.Length)
        {
            ThrowNotSameSize();
        }
        if (first.Length != fifth.Length)
        {
            ThrowNotSameSize();
        }
        return true;
    }
}
