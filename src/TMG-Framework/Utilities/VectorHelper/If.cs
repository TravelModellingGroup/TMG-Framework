/*
    Copyright 2018-2026 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        /// <summary>
        /// Sets dest[i] = ifTrue[i] if cond[i] != 0 else ifFalse[i] for all i
        /// </summary>
        /// <param name="dest">The location to store the results to.</param>
        /// <param name="cond">The variable containing the condition values.</param>
        /// <param name="ifTrue">The values to set if the condition is not zero.</param>
        /// <param name="ifFalse">The values to set if the condition is zero.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void If(Span<float> dest, Span<float> cond, Span<float> ifTrue, Span<float> ifFalse)
        {
            EnsureSameSize(dest, cond, ifTrue, ifFalse);
            nuint i = 0;
            var end = (nuint)dest.Length - 16;
            ref float destRef = ref MemoryMarshal.GetReference(dest);
            ref float condRef = ref MemoryMarshal.GetReference(cond);
            ref float ifTrueRef = ref MemoryMarshal.GetReference(ifTrue);
            ref float ifFalseRef = ref MemoryMarshal.GetReference(ifFalse);

            if (Vector512.IsHardwareAccelerated && dest.Length >= Vector512<float>.Count)
            {
                var vZero = Vector512<float>.Zero;
                for (; i <= end; i += (nuint)Vector512<float>.Count)
                {
                    var vCond = Vector512.LoadUnsafe(ref condRef, i);
                    var vIfTrue = Vector512.LoadUnsafe(ref ifTrueRef, i);
                    var vIfFalse = Vector512.LoadUnsafe(ref ifFalseRef, i);
                    var mask = Vector512.Equals(vCond, vZero);
                    // The mask is for the false case so true and false are swapped here
                    var vResult = Vector512.ConditionalSelect(mask, vIfFalse, vIfTrue);
                    vResult.StoreUnsafe(ref destRef, i);
                }
                // Check to see if we can perform a final 256-bit operation
                if (i < (nuint)(dest.Length - Vector256<float>.Count))
                {
                    var vCond = Vector256.LoadUnsafe(ref condRef, i);
                    var vIfTrue = Vector256.LoadUnsafe(ref ifTrueRef, i);
                    var vIfFalse = Vector256.LoadUnsafe(ref ifFalseRef, i);
                    var mask = Vector256.Equals(vCond, Vector256<float>.Zero);
                    // The mask is for the false case so true and false are swapped here
                    var vResult = Vector256.ConditionalSelect(mask, vIfFalse, vIfTrue);
                    vResult.StoreUnsafe(ref destRef, i);
                    i += (nuint)Vector256<float>.Count;
                }
            }
            else if (Vector256.IsHardwareAccelerated && dest.Length >= Vector256<float>.Count)
            {
                var vZero = Vector256<float>.Zero;
                for (; i <= end; i += (nuint)Vector256<float>.Count)
                {
                    var vCond = Vector256.LoadUnsafe(ref condRef, i);
                    var vIfTrue = Vector256.LoadUnsafe(ref ifTrueRef, i);
                    var vIfFalse = Vector256.LoadUnsafe(ref ifFalseRef, i);
                    var mask = Vector256.Equals(vCond, vZero);
                    // The mask is for the false case so true and false are swapped here
                    var vResult = Vector256.ConditionalSelect(mask, vIfFalse, vIfTrue);
                    vResult.StoreUnsafe(ref destRef, i);
                }
            }

            for (; i < (nuint)dest.Length; i++)
            {
                float c = Unsafe.Add(ref condRef, i);
                Unsafe.Add(ref destRef, i) = c != 0.0f ? Unsafe.Add(ref ifTrueRef, i) : Unsafe.Add(ref ifFalseRef, i);
            }
        }
    }
}
