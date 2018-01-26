/*
    Copyright 2015-2018 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
        public static void Subtract(float[] dest, float[] lhs, float rhs)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, 0, lhs.Length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> rhsV = new Vector<float>(rhs);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] - rhsV;
                destSpan[i + 1] = lhsSpan[i + 1] - rhsV;
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = lhs[i] - rhs;
            }
        }

        public static void Subtract(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> rhsV = new Vector<float>(rhs);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] - rhsV;
                destSpan[i + 1] = lhsSpan[i + 1] - rhsV;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] - rhs;
            }
        }

        public static void Subtract(float[] dest, float lhs, float[] rhs)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, 0, rhs.Length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> lhsV = new Vector<float>(lhs);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsV - rhsSpan[i];
                destSpan[i + 1] = lhsV - rhsSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = lhs - rhs[i];
            }
        }

        public static void Subtract(float[] dest, int destIndex, float lhs, float[] rhs, int rhsIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> lhsV = new Vector<float>(lhs);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsV - rhsSpan[i];
                destSpan[i + 1] = lhsV - rhsSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs - rhs[rhsIndex + i];
            }
        }

        public static void Subtract(float[][] destination, float lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Subtract(destination[row], lhs, rhs[row]);
            });
        }

        public static void Subtract(float[][] destination, float[][] lhs, float rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Subtract(destination[row], lhs[row], rhs);
            });
        }

        public static void Subtract(float[][] destination, float[][] lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Subtract(destination[row], 0, lhs[row], 0, rhs[row], 0, destination[row].Length);
            });
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] - rhsSpan[i];
                destSpan[i + 1] = lhsSpan[i + 1] - rhsSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] - rhs[rhsIndex + i];
            }
        }
    }
}
