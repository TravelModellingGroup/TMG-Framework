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
    /// <summary>
    /// Dest[i] = lhs[i] * rhs[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right)
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
                var vResult = vLeft * vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft * vRight;
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
                var vResult = vLeft * vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i);
        }
    }

    /// <summary>
    /// Dest[i] = lhs[i] * rhs[i] * third[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="third">The third value that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right, ReadOnlySpan<float> third)
    {
        EnsureSameSize(dest, left, right, third);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);
        ref float thirdRef = ref MemoryMarshal.GetReference(third);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vThird = Vector512.LoadUnsafe(ref thirdRef, i);
                var vResult = vLeft * vRight * vThird;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vResult = vLeft * vRight * vThird;
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
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vResult = vLeft * vRight * vThird;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i) * Unsafe.Add(ref thirdRef, i);
        }
    }

    /// <summary>
    /// Dest[i] = lhs[i] * rhs[i] * third[i] * fourth[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="third">The third value that is being multiplied.</param>
    /// <param name="fourth">The fourth value that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right, ReadOnlySpan<float> third,
        ReadOnlySpan<float> fourth)
    {
        EnsureSameSize(dest, left, right, third, fourth);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);
        ref float thirdRef = ref MemoryMarshal.GetReference(third);
        ref float fourthRef = ref MemoryMarshal.GetReference(fourth);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vThird = Vector512.LoadUnsafe(ref thirdRef, i);
                var vFourth = Vector512.LoadUnsafe(ref fourthRef, i);
                var vResult = (vLeft * vRight) * (vThird * vFourth);
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vFourth = Vector256.LoadUnsafe(ref fourthRef, i);
                var vResult = (vLeft * vRight) * (vThird * vFourth);
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
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vFourth = Vector256.LoadUnsafe(ref fourthRef, i);
                var vResult = (vLeft * vRight) * (vThird * vFourth);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = (Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i)) * (Unsafe.Add(ref thirdRef, i) 
                * Unsafe.Add(ref fourthRef, i));
        }
    }

    /// <summary>
    /// Dest[i] = lhs[i] * rhs
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        ReadOnlySpan<float> left, float right)
    {
        EnsureSameSize(dest, left);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vRight = Vector512.Create(right);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vResult = vLeft * vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight256 = Vector256.Create(right);
                var vResult = vLeft * vRight256;
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
                var vResult = vLeft * vRight;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * right;
        }
    }

    /// <summary>
    /// Dest[i] = lhs * rhs[i]
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The scalar value that is being multiplied.</param>
    /// <param name="right">The span of values that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        float left, ReadOnlySpan<float> right)
    {
        Multiply(dest, right, left);
    }

    /// <summary>
    /// Dest[i] = lhs[i] * rhs[i] * third
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="third">The third value that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right, float third)
    {
        EnsureSameSize(dest, left, right);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            var vThird = Vector512.Create(third);
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft * vRight * vThird;
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vThird256 = Vector256.Create(third);
                var vResult = vLeft * vRight * vThird256;
                vResult.StoreUnsafe(ref destRef, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
        {
            var vThird = Vector256.Create(third);
            for (; i <= end; i += (nuint)Vector256<float>.Count)
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vResult = vLeft * vRight * third;
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i) * third;
        }
    }

    /// <summary>
    /// Dest[i] = lhs[i] * rhs[i] * third[i] * forth
    /// </summary>
    /// <param name="dest">The location to store the values into.</param>
    /// <param name="left">The first value that is being multiplied.</param>
    /// <param name="right">The second value that is being multiplied.</param>
    /// <param name="third">The third value that is being multiplied.</param>
    /// <param name="fourth">The fourth value that is being multiplied.</param>
    public static void Multiply(Span<float> dest,
        ReadOnlySpan<float> left, ReadOnlySpan<float> right, ReadOnlySpan<float> third, float fourth)
    {
        EnsureSameSize(dest, left, right, third);
        nuint i = 0;
        var end = (nuint)dest.Length - 16;
        ref float destRef = ref MemoryMarshal.GetReference(dest);
        ref float leftRef = ref MemoryMarshal.GetReference(left);
        ref float rightRef = ref MemoryMarshal.GetReference(right);
        ref float thirdRef = ref MemoryMarshal.GetReference(third);

        if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
        {
            for (; i <= end; i += (nuint)Vector512<float>.Count)
            {
                var vLeft = Vector512.LoadUnsafe(ref leftRef, i);
                var vRight = Vector512.LoadUnsafe(ref rightRef, i);
                var vThird = Vector512.LoadUnsafe(ref thirdRef, i);
                var vResult = (vLeft * vRight) * (vThird * fourth);
                vResult.StoreUnsafe(ref destRef, i);
            }
            // Check to see if we can perform a final 256-bit operation
            if (i < (nuint)(dest.Length - Vector256<float>.Count))
            {
                var vLeft = Vector256.LoadUnsafe(ref leftRef, i);
                var vRight = Vector256.LoadUnsafe(ref rightRef, i);
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vResult = (vLeft * vRight) * (vThird * fourth);
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
                var vThird = Vector256.LoadUnsafe(ref thirdRef, i);
                var vResult = (vLeft * vRight) * (vThird * fourth);
                vResult.StoreUnsafe(ref destRef, i);
            }
        }

        for (; i < (nuint)dest.Length; i++)
        {
            Unsafe.Add(ref destRef, i) = (Unsafe.Add(ref leftRef, i) 
                * Unsafe.Add(ref rightRef, i)) * (Unsafe.Add(ref thirdRef, i) * fourth);
        }
    }

}

