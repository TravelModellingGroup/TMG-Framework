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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(destination, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var secondSpan = (new Span<float>(second, secondIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] * secondSpan[i];
                destSpan[i + 1] = firstSpan[i + 1] * secondSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                destination[destIndex + i] = first[firstIndex + i] * second[secondIndex + i];
            }
        }

        public static void Multiply(float[] dest, float[] source, float scalar)
        {
            var length = dest.Length;
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(source, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
            var vScalar = new Vector<float>(scalar);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] * vScalar;
                destSpan[i + 1] = firstSpan[i + 1] * vScalar;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[i] = source[i] * scalar;
            }
        }

        public static void Multiply(float[][] destination, float lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Multiply(destination[row], rhs[row], lhs);
            });
        }

        public static void Multiply(float[][] destination, float[][] lhs, float rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Multiply(destination[row], lhs[row], rhs);
            });
        }

        public static void Multiply(float[][] destination, float[][] lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Multiply(destination[row], 0, lhs[row], 0, rhs[row], 0, destination[row].Length);
            });
        }

        /// <summary>
        /// Multiply an array by a scalar and store it in another array.
        /// </summary>
        /// <param name="destination">Where to store the results</param>
        /// <param name="destIndex">The offset to start at</param>
        /// <param name="first">The array to multiply</param>
        /// <param name="firstIndex">The first index to multiply</param>
        /// <param name="scalar">The value to multiply against</param>
        /// <param name="length">The number of elements to multiply</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(float[] destination, int destIndex, float[] first, int firstIndex, float scalar, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(destination, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var scalarV = new Vector<float>(scalar);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] * scalarV;
                destSpan[i + 1] = firstSpan[i + 1] * scalarV;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                destination[destIndex + i] = first[firstIndex + i] * scalar;
            }
        }

        /// <summary>
        /// Multiply first, second, and the scalar and save into the destination vector
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="destIndex"></param>
        /// <param name="first"></param>
        /// <param name="firstIndex"></param>
        /// <param name="second"></param>
        /// <param name="secondIndex"></param>
        /// <param name="scalar"></param>
        /// <param name="length"></param>
        internal static void Multiply(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, float scalar, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(destination, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var secondSpan = (new Span<float>(second, secondIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var scalarV = new Vector<float>(scalar);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] * secondSpan[i] * scalarV;
                destSpan[i + 1] = firstSpan[i + 1] * secondSpan[i + 1] * scalarV;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                destination[destIndex + i] = first[firstIndex + i] * second[secondIndex + i] * scalar;
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
        /// <param name="third"></param>
        /// <param name="thirdIndex"></param>
        /// <param name="fourth"></param>
        /// <param name="fourthIndex"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex,
            float[] third, int thirdIndex, float[] fourth, int fourthIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(destination, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var firstSpan = (new Span<float>(first, firstIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var secondSpan = (new Span<float>(second, secondIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var thirdSpan = (new Span<float>(third, thirdIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var fourthSpan = (new Span<float>(fourth, fourthIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = firstSpan[i] * secondSpan[i] * thirdSpan[i] * fourthSpan[i];
                destSpan[i + 1] = firstSpan[i + 1] * secondSpan[i + 1] * thirdSpan[i + 1] * fourthSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                destination[destIndex + i] = first[firstIndex + i] * second[secondIndex + i] * third[thirdIndex + i] * fourth[fourthIndex + i];
            }
        }
    }
}
