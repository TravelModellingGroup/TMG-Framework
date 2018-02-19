/*
    Copyright 2015-2018 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        // Are we hardware accelerated
        static VectorHelper()
        {
            MaxFloat = new Vector<float>(float.MaxValue);
        }
        /// <summary>
        /// Dest[i] = hls[i] * rhs[i] + add
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, float add, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var vAdd = new Vector<float>(add);
            int i = 0;
            for (; i < destSpan.Length - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] * rhsSpan[i] + vAdd;
                destSpan[i + 1] = lhsSpan[i + 1] * rhsSpan[i + 1] + vAdd;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] * rhs[rhsIndex + i] + add;
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs + add
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, float add, int length)
        {
            var vAdd = new Vector<float>(add);
            var r = new Vector<float>(rhs);
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < destSpan.Length - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] * r + vAdd;
                destSpan[i + 1] = lhsSpan[i + 1] * rhs + vAdd;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] * rhs + add;
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs + add[i]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, float[] add, int addIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var r = new Vector<float>(rhs);
            var addSpan = (new Span<float>(add, addIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < destSpan.Length - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] * r + addSpan[i];
                destSpan[i + 1] = lhsSpan[i + 1] * r + addSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] * rhs + add[addIndex + i];
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs[i] + add[i]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, float[] add, int addIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var addSpan = (new Span<float>(add, addIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < destSpan.Length - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] * rhsSpan[i] + addSpan[i];
                destSpan[i + 1] = lhsSpan[i + 1] * rhsSpan[i + 1] + addSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] * rhs[rhsIndex + i] + add[addIndex + i];
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs[i] + add[i]
        /// </summary>
        public static void FusedMultiplyAdd(float[] dest, float[] lhs, float[] rhs, float[] add, int offset, int length)
        {
            if (dest == null || lhs == null || rhs == null || add == null)
            {
                throw new ArgumentNullException();
            }
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
            var addSpan = (new Span<float>(add, offset, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < destSpan.Length - 1; i+=2)
            {
                destSpan[i] = lhsSpan[i] * rhsSpan[i] + addSpan[i];
                destSpan[i + 1] = lhsSpan[i + 1] * rhsSpan[i + 1] + addSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[offset + i] = lhs[offset + i] * rhs[offset + i] + add[offset + i];
            }
        }
    }
}
