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
using System.Threading.Tasks;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfEqual(float[] dest, int destIndex, float lhs, float[] rhs, int rhsIndex, int length)
        {
            if (System.Numerics.Vector.IsHardwareAccelerated)
            {
                int i;
                Vector<float> zero = Vector<float>.Zero;
                Vector<float> one = Vector<float>.One;
                Vector<float> vLhs = new Vector<float>(lhs);
                for (i = 0; i < length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var vRhs = new Vector<float>(rhs, rhsIndex + i);
                    System.Numerics.Vector.ConditionalSelect(System.Numerics.Vector.Equals(vLhs, vRhs), one, zero).CopyTo(dest, i);
                }
                for (; i < length; i++)
                {
                    dest[destIndex + i] = lhs == rhs[rhsIndex + i] ? 1 : 0;
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs == rhs[rhsIndex + i] ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfEqual(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, int length)
        {
            if (System.Numerics.Vector.IsHardwareAccelerated)
            {
                int i;
                Vector<float> zero = Vector<float>.Zero;
                Vector<float> one = Vector<float>.One;
                Vector<float> vRhs = new Vector<float>(rhs);
                for (i = 0; i < length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var vLhs = new Vector<float>(lhs, lhsIndex + i);
                    System.Numerics.Vector.ConditionalSelect(System.Numerics.Vector.Equals(vLhs, vRhs), one, zero).CopyTo(dest, destIndex + i);
                }
                for (; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] == rhs ? 1 : 0;
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] == rhs ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfEqual(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, int length)
        {
            if (System.Numerics.Vector.IsHardwareAccelerated)
            {
                int i;
                Vector<float> zero = Vector<float>.Zero;
                Vector<float> one = Vector<float>.One;
                for (i = 0; i < length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var vLhs = new Vector<float>(lhs, lhsIndex + i);
                    var vRhs = new Vector<float>(rhs, rhsIndex + i);
                    System.Numerics.Vector.ConditionalSelect(System.Numerics.Vector.Equals(vLhs, vRhs), one, zero).CopyTo(dest, destIndex + i);
                }
                for (; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] == rhs[rhsIndex + i] ? 1 : 0;
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    dest[destIndex + i] = lhs[lhsIndex + i] == rhs[rhsIndex + i] ? 1 : 0;
                }
            }
        }
    }
}
