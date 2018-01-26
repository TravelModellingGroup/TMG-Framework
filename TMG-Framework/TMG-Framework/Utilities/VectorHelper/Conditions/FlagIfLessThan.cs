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
using static System.Numerics.Vector;

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfLessThan(float[] dest, int destIndex, float lhs, float[] rhs, int rhsIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsV = new Vector<float>(lhs);
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> zero = Vector<float>.Zero;
            Vector<float> one = Vector<float>.One;
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = ConditionalSelect(LessThan(lhsV, rhsSpan[i]), one, zero);
                destSpan[i + 1] = ConditionalSelect(LessThan(lhsV, rhsSpan[i]), one, zero);
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs < rhs[rhsIndex + i] ? 1.0f : 0.0f;
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfLessThan(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> zero = Vector<float>.Zero;
            Vector<float> one = Vector<float>.One;
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = ConditionalSelect(LessThan(lhsSpan[i], rhsSpan[i]), one, zero);
                destSpan[i + 1] = ConditionalSelect(LessThan(lhsSpan[i + 1], zero), one, zero);
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] <= rhs[rhsIndex + i] ? 1.0f : 0.0f;
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfLessThan(float[][] dest, float[][] data, float literalValue)
        {
            Parallel.For(0, dest.Length, i =>
            {
                FlagIfLessThan(dest[i], data[i], literalValue);
            });
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfLessThan(float[] dest, float[] data, float literalValue)
        {
            // operator flips when moving rhs to lhs
            FlagIfGreaterThan(dest, 0, literalValue, data, 0, dest.Length);
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfLessThan(float[][] dest, float[][] lhs, float[][] rhs)
        {
            Parallel.For(0, dest.Length, i =>
            {
                FlagIfLessThan(dest[i], 0, lhs[i], 0, rhs[i], 0, dest.Length);
            });
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagIfLessThan(float[][] v1, float literalValue, float[][] v2)
        {
            Parallel.For(0, v1.Length, i =>
            {
                FlagIfGreaterThan(v1[i], 0, literalValue, v2[i], 0, v1[i].Length);
            });
        }
    }
}
