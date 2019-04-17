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
        public static void Divide(float[] dest, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
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
                destSpan[i] = firstSpan[i] / secondSpan[i];
                destSpan[i + 1] = firstSpan[i + 1] / secondSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = first[firstIndex + i] / second[secondIndex + i];
            }
        }

        public static void Divide(float[][] destination, float numerator, float[][] denominator)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Vector<float> n = new Vector<float>(numerator);
                var length = destination[row].Length;
                var vectorLength = length / Vector<float>.Count;
                var remainder = length % Vector<float>.Count;
                var destSpan = (new Span<float>(destination[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                var denomSpan = (new Span<float>(denominator[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                // copy everything we can do inside of a vector
                int i = 0;
                for (; i < vectorLength - 1; i += 2)
                {
                    destSpan[i] = n / denomSpan[i];
                    destSpan[i + 1] = n / denomSpan[i + 1];
                }
                i *= Vector<float>.Count;
                for (; i < destination[row].Length; i++)
                {
                    destination[row][i] = numerator / denominator[row][i];
                }
            });
        }

        public static void Divide(float[][] destination, float[][] numerator, float denominator)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Vector<float> d = new Vector<float>(denominator);
                var length = destination[row].Length;
                var vectorLength = length / Vector<float>.Count;
                var remainder = length % Vector<float>.Count;
                var destSpan = (new Span<float>(destination[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                var numSpan = (new Span<float>(numerator[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                // copy everything we can do inside of a vector
                int i = 0;
                for (; i < vectorLength - 1; i += 2)
                {
                    destSpan[i] = numSpan[i] / d;
                    destSpan[i + 1] = numSpan[i + 1] / d;
                }
                i *= Vector<float>.Count;
                for (; i < destination[row].Length; i++)
                {
                    destination[row][i] = numerator[row][i] / denominator;
                }
            });
        }

        public static void Divide(float[][] destination, float[][] numerator, float[][] denominator)
        {
            Parallel.For(0, destination.Length, row =>
            {
                var length = destination[row].Length;
                var vectorLength = length / Vector<float>.Count;
                var remainder = length % Vector<float>.Count;
                var destSpan = (new Span<float>(destination[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                var numSpan = (new Span<float>(numerator[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                var denomSpan = (new Span<float>(denominator[row], 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                // copy everything we can do inside of a vector
                int i = 0;
                for (; i < vectorLength - 1; i += 2)
                {
                    destSpan[i] = numSpan[i] / denomSpan[i];
                    destSpan[i + 1] = numSpan[i + 1] / denomSpan[i + 1];
                }
                i *= Vector<float>.Count;
                for (; i < destination[row].Length; i++)
                {
                    destination[row][i] = numerator[row][i] / denominator[row][i];
                }
            });
        }

        public static void Divide(float[] dest, float[] lhs, float rhs)
        {
            Vector<float> d = new Vector<float>(rhs);
            var length = dest.Length;
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
            var numSpan = (new Span<float>(lhs, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = numSpan[i] / d;
                destSpan[i + 1] = numSpan[i + 1] / d;
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = lhs[i] / rhs;
            }
        }

        public static void Divide(float[] dest, int destIndex, float[] lhs, int lhsIndex, float rhs, int length)
        {
            Vector<float> d = new Vector<float>(rhs);
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var numSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = numSpan[i] / d;
                destSpan[i + 1] = numSpan[i + 1] / d;
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] / rhs;
            }
        }

        public static void Divide(float[] dest, float lhs, float[] rhs)
        {
            var length = dest.Length;
            Vector<float> n = new Vector<float>(lhs);
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
            var denomSpan = (new Span<float>(rhs, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = n / denomSpan[i];
                destSpan[i + 1] = n / denomSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = lhs / rhs[i];
            }
        }

        public static void Divide(float[] dest, int destIndex, float lhs, float[] rhs, int rhsIndex, int length)
        {
            Vector<float> n = new Vector<float>(lhs);
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var denomSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = n / denomSpan[i];
                destSpan[i + 1] = n / denomSpan[i + 1];
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs / rhs[rhsIndex + i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Divide(Span<float> dest, Span<float> lhs, float rhs)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var destSpan = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (lhs.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vRhs = new Vector<float>(rhs);
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = lhsSpan[i] / vRhs;
                destSpan[i + 1] = lhsSpan[i + 1] / vRhs;
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = lhs[i] / rhs;
            }
        }
    }
}
