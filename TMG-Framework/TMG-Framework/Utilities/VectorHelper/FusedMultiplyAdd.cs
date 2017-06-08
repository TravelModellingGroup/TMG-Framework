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

using System.Numerics;
using System.Threading.Tasks;

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        /// <summary>
        /// Dest[i] = hls[i] * rhs[i] + add
        /// </summary>
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, float add, int length)
        {
            if (Vector.IsHardwareAccelerated)
            {
                int i;
                var vAdd = new Vector<float>(add);
                for (i = 0; i < length - Vector<float>.Count; i++)
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
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] * rhs[rhsIndex + i] + add;
                }
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs + add
        /// </summary>
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, float add, int length)
        {
            if (Vector.IsHardwareAccelerated)
            {
                int i;
                var vAdd = new Vector<float>(add);
                var r = new Vector<float>(rhs);
                for (i = 0; i < length - Vector<float>.Count; i++)
                {
                    var l = new Vector<float>(lhs, lhsIndex + i);
                    (l * r + vAdd).CopyTo(dest, destIndex + i);
                }
                for (; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] * rhs + add;
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] * rhs + add;
                }
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs + add[i]
        /// </summary>
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, float[] add, int addIndex, int length)
        {
            if (Vector.IsHardwareAccelerated)
            {
                int i;
                var r = new Vector<float>(rhs);
                for (i = 0; i < length - Vector<float>.Count; i++)
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
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] * rhs + add[addIndex + i];
                }
            }
        }

        /// <summary>
        /// Dest[i] = hls[i] * rhs[i] + add[i]
        /// </summary>
        public static void FusedMultiplyAdd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, float[] add, int addIndex, int length)
        {
            if (Vector.IsHardwareAccelerated)
            {
                int i;
                for (i = 0; i < dest.Length - Vector<float>.Count; i++)
                {
                    var l = new Vector<float>(lhs, lhsIndex + i);
                    var r = new Vector<float>(rhs, rhsIndex + i);
                    var vAdd = new Vector<float>(add, addIndex + i);
                    (l * r + vAdd).CopyTo(dest, destIndex + i);
                }
                for (; i < dest.Length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] * rhs[rhsIndex + i] + add[addIndex + i];
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] * rhs[rhsIndex + i] + add[addIndex + i];
                }
            }
        }
    }
}
