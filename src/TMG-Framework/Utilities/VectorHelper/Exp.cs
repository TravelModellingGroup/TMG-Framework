/*
    Copyright 2026 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace TMG.Utilities;

public static partial class VectorHelper
{
    /// <summary>
    /// Converts all of the values in src to their exponentiated versions.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="src"></param>
    public static void Exp(Span<float> destination, ReadOnlySpan<float> src)
    {
        EnsureSameSize(destination, src);
        nuint i = 0;
        ref var pSrc = ref MemoryMarshal.GetReference(src);
        ref var pDest = ref MemoryMarshal.GetReference(destination);
        if (Vector512.IsHardwareAccelerated && destination.Length >= Vector512<float>.Count)
        {
            for (; i <= (nuint)(destination.Length - Vector512<float>.Count); i += (nuint)Vector512<float>.Count)
            {
                var temp = Vector512.LoadUnsafe(ref pSrc, i);
                temp = Exp(temp);
                Vector512.StoreUnsafe(temp, ref pDest, i);
            }

            if (i <= (nuint)(destination.Length - Vector256<float>.Count))
            {
                var temp = Vector256.LoadUnsafe(ref pSrc, i);
                temp = Exp(temp);
                Vector256.StoreUnsafe(temp, ref pDest, i);
                i += (nuint)Vector256<float>.Count;
            }
        }
        // Fall back to 256 bit instructions
        else if (Vector256.IsHardwareAccelerated && destination.Length >= Vector256<float>.Count)
        {
            for (; i <= (nuint)(destination.Length - Vector256<float>.Count); i += (nuint)Vector256<float>.Count)
            {
                var temp = Vector256.LoadUnsafe(ref pSrc, i);
                temp = Exp(temp);
                Vector256.StoreUnsafe(temp, ref pDest, i);
            }
        }
        // Fallback to basic for everything not accelerated
        for (; i < (nuint)destination.Length; i++)
        {
            destination[(int)i] = MathF.Exp(src[(int)i]);
        }
    }

    /// <summary>
    /// Converts the given vectors elements to exp(x) at double precision.
    /// </summary>
    /// <param name="x">The values to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector512<float> Exp(Vector512<float> x)
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
        lowD = Vector512.Exp(lowD);
        highD = Vector512.Exp(highD);
        if (Avx.IsSupported)
        {
            low = Avx512F.ConvertToVector256Single(lowD);
            high = Avx512F.ConvertToVector256Single(highD);
        }
        else
        {
            low = Vector256.Create((float)lowD.GetElement(0), (float)lowD.GetElement(1), (float)lowD.GetElement(2), (float)lowD.GetElement(3),
                (float)lowD.GetElement(4), (float)lowD.GetElement(5), (float)lowD.GetElement(6), (float)lowD.GetElement(7));
            high = Vector256.Create((float)highD.GetElement(0), (float)highD.GetElement(1), (float)highD.GetElement(2), (float)highD.GetElement(3),
                (float)highD.GetElement(4), (float)highD.GetElement(5), (float)highD.GetElement(6), (float)highD.GetElement(7));
        }
        return Vector512.Create(low, high);
    }

    /// <summary>
    /// Converts the given vectors elements to exp(x) at double precision.
    /// </summary>
    /// <param name="x">The values to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector256<float> Exp(Vector256<float> x)
    {
        Vector128<float> low = x.GetLower();
        Vector128<float> high = x.GetUpper();
        Vector256<double> lowD;
        Vector256<double> highD;
        if (Avx.IsSupported)
        {
            lowD = Avx.ConvertToVector256Double(low);
            highD = Avx.ConvertToVector256Double(high);
        }
        else
        {
            lowD = Vector256.Create((double)low.GetElement(0), (double)low.GetElement(1), (double)low.GetElement(2), (double)low.GetElement(3));
            highD = Vector256.Create((double)high.GetElement(0), (double)high.GetElement(1), (double)high.GetElement(2), (double)high.GetElement(3));
        }
        lowD = Vector256.Exp(lowD);
        highD = Vector256.Exp(highD);
        if (Avx.IsSupported)
        {
            low = Avx.ConvertToVector128Single(lowD);
            high = Avx.ConvertToVector128Single(highD);
        }
        else
        {
            low = Vector128.Create((float)lowD.GetElement(0), (float)lowD.GetElement(1), (float)lowD.GetElement(2), (float)lowD.GetElement(3));
            high = Vector128.Create((float)highD.GetElement(0), (float)highD.GetElement(1), (float)highD.GetElement(2), (float)highD.GetElement(3));
        }
        return Vector256.Create(low, high);
    }

}
