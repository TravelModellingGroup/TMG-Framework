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
        public static void Add(float[] dest, float[] source, float scalar)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var sourceSpan = (new Span<float>(source, 0, source.Length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> constant = new Vector<float>(scalar);
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = sourceSpan[i] + constant;
                destSpan[i + 1] = sourceSpan[i + 1] + constant;
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = source[i] + scalar;
            }
        }

        public static void Add(float[] dest, int destIndex, float[] source, int sourceIndex, float scalar, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var sourceSpan = (new Span<float>(source, sourceIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> constant = new Vector<float>(scalar);
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = sourceSpan[i] + constant;
                destSpan[i + 1] = sourceSpan[i + 1] + constant;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = source[sourceIndex + i] + scalar;
            }
        }


        public static void Add(float[] dest, float[] lhs, float[] rhs)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, 0, lhs.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, 0, rhs.Length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] + rhsSpan[i];
                destSpan[i + 1] = lhsSpan[i + 1] + rhsSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = lhs[i] + rhs[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(float[] dest, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, float[] third, int thirdIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var secondSpan = (new Span<float>(second, secondIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var thirdSpan = (new Span<float>(second, thirdIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] + secondSpan[i] + thirdSpan[i];
                destSpan[i + 1] = firstSpan[i + 1] + secondSpan[i + 1] + thirdSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = first[firstIndex + i] + second[secondIndex + i] + third[thirdIndex + i];
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
        public static void Add(float[] dest, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var secondSpan = (new Span<float>(second, secondIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] + secondSpan[i];
                destSpan[i + 1] = firstSpan[i + 1] + secondSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = first[firstIndex + i] + second[secondIndex + i];
            }
        }

        public static void Add(float[][] destination, float lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Add(destination[row], rhs[row], lhs);
            });
        }

        public static void Add(float[][] destination, float[][] lhs, float rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Add(destination[row], lhs[row], rhs);
            });
        }

        public static void Add(float[][] destination, float[][] lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Add(destination[row], lhs[row], rhs[row]);
            });
        }

        public static void AddHorizontal(float[][] destination, float[][] lhs, float[] rhs)
        {
            Parallel.For(0, destination.Length, i =>
            {
                Add(destination[i], 0, lhs[i], 0, rhs, 0, destination[i].Length);
            });
        }

        public static void AddVertical(float[][] destination, float[][] lhs, float[] rhs)
        {
            Parallel.For(0, destination.Length, i =>
            {
                Add(destination[i], lhs[i], rhs[i]);
            });
        }
    }
}
