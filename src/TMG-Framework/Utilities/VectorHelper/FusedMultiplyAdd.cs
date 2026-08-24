/*
    Copyright 2015-2026 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF2.

    XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Numerics;
using System.Runtime.Intrinsics;

namespace TMG.Utilities;

public static partial class VectorHelper
{
    /// <summary>
    /// Dest[i] = lhs[i] * rhs[i] + add[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="add">The value that is being added.</param>
    public static void FusedMultiplyAdd(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right, ReadOnlySpan<float> add)
    {
        EnsureSameSize(dest, left, right, add);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);
        ref float addRef = ref MemoryMarshal.GetReference(add);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vAdd = Vector512.LoadUnsafe(ref addRef, i);
                var vResult = Vector512.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vAdd = Vector256.LoadUnsafe(ref addRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd);
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
                var vAdd = Vector256.LoadUnsafe(ref addRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i) 
                + Unsafe.Add(ref addRef, i);
        }
    }

    /// <summary>
    /// Dest[i] = lhs[i] * rhs + add[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="add">The value that is being added.</param>
    public static void FusedMultiplyAdd(Span<float> dest,
        ReadOnlySpan<float> left, float right, ReadOnlySpan<float> add)
    {
        EnsureSameSize(dest, left, add);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float addRef = ref MemoryMarshal.GetReference(add);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vRight = Vector512.Create(right);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                
                var vAdd = Vector512.LoadUnsafe(ref addRef, i);
                var vResult = Vector512.FusedMultiplyAdd(vLeft, vRight, vAdd);
                
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight256 = Vector256.Create(right);
                var vAdd = Vector256.LoadUnsafe(ref addRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight256, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vRight = Vector256.Create(right);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vAdd = Vector256.LoadUnsafe(ref addRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * right 
                + Unsafe.Add(ref addRef, i);
        }
    }

        /// <summary>
    /// Dest[i] = lhs[i] * rhs + add
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="add">The value that is being added.</param>
    public static void FusedMultiplyAdd(Span<float> dest,
        ReadOnlySpan<float> left, float right, float add)
    {
        EnsureSameSize(dest, left);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vRight = Vector512.Create(right);
            var vAdd = Vector512.Create(add);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vResult = Vector512.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }

            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight256 = Vector256.Create(right);
                var vAdd256 = Vector256.Create(add);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight256, vAdd256);
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vRight = Vector256.Create(right);
            var vAdd = Vector256.Create(add);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * right 
                + add;
        }
    }

        /// <summary>
    /// Dest[i] = lhs[i] * rhs[i] + add
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="add">The value that is being added.</param>
    public static void FusedMultiplyAdd(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right, float add)
    {
        EnsureSameSize(dest, left, right);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vAdd = Vector512.Create(add);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vResult = Vector512.FusedMultiplyAdd(vLeft, vRight, vAdd);
                
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vAdd256 = Vector256.Create(add);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd256);
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vAdd = Vector256.Create(add);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i) 
                + add;
        }
    }

    /// <summary>
    /// Dest[i] = lhs * rhs + add[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="add">The value that is being added.</param>
    public static void FusedMultiplyAdd(Span<float> dest,
        float left, float right, ReadOnlySpan<float> add)
    {
        EnsureSameSize(dest, add);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float addRef = ref MemoryMarshal.GetReference(add);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vLeft = Vector512.Create(left);
            var vRight = Vector512.Create(right);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vAdd = Vector512.LoadUnsafe(ref addRef, i);
                var vResult = Vector512.FusedMultiplyAdd(vLeft, vRight, vAdd);                
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft256 = Vector256.Create(left);
                var vRight256 = Vector256.Create(right);
                var vAdd = Vector256.LoadUnsafe(ref addRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft256, vRight256, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vLeft = Vector256.Create(left);
            var vRight = Vector256.Create(right);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vAdd = Vector256.LoadUnsafe(ref addRef, i);
                var vResult = Vector256.FusedMultiplyAdd(vLeft, vRight, vAdd);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = left * right + Unsafe.Add(ref addRef, i);
        }
    }
   
}

