/*
    Copyright 2015-2016 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
            int i;
            var vAdd = new Vector<float>(add);
            for (i = 0; i < length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var l = new Vector<float>(lhs, lhsIndex + i);
                var r = new Vector<float>(rhs, rhsIndex + i);
                (l * r + vAdd).CopyTo(dest, destIndex + i);
            }
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
            int i;
            var vAdd = new Vector<float>(add);
            var r = new Vector<float>(rhs);
            for (i = 0; i < length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var l = new Vector<float>(lhs, lhsIndex + i);
                (l * r + vAdd).CopyTo(dest, destIndex + i);
            }
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
            int i;
            var r = new Vector<float>(rhs);
            for (i = 0; i < length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var l = new Vector<float>(lhs, lhsIndex + i);
                var vAdd = new Vector<float>(add, addIndex + i);
                (l * r + vAdd).CopyTo(dest, destIndex + i);
            }
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
            
            int i;
            var vLength = length - Vector<float>.Count;
            for (i = 0; i < vLength; i += Vector<float>.Count)
            {
                var l = new Vector<float>(lhs, lhsIndex + i);
                var r = new Vector<float>(rhs, rhsIndex + i);
                var vAdd = new Vector<float>(add, addIndex + i);
                (l * r + vAdd).CopyTo(dest, destIndex + i);
            }
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
            if(dest == null || lhs == null || rhs == null || add == null)
            {
                throw new ArgumentNullException();
            }
            if(length - Vector<float>.Count >= rhs.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            int i;
            for (i = 0; i < dest.Length - Vector<float>.Count && i < length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var l = new Vector<float>(lhs, offset + i);
                var r = new Vector<float>(rhs, offset + i);
                var vAdd = new Vector<float>(add, offset + i);
                (l * r + vAdd).CopyTo(dest, offset + i);
            }
            for (; i < length; i++)
            {
                dest[offset + i] = lhs[offset + i] * rhs[offset + i] + add[offset + i];
            }
        }
    }
}
