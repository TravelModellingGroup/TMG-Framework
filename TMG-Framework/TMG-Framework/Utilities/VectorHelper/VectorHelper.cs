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

namespace TMG.Utilities
{
    /// <summary>
    /// This class is designed to help facilitate the use of the SIMD instructions available in
    /// modern .Net.
    /// </summary>
    public static partial class VectorHelper
    {
        /// <summary>
        /// A vector containing the maximum value of a float
        /// </summary>
        private static Vector<float> MaxFloat;

        /// <summary>
        /// Add up the elements in the vector
        /// </summary>
        /// <param name="v">The vector to sum</param>
        /// <returns>The sum of the elements in the vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Sum(ref Vector<float> v)
        {
            // shockingly to myself this is actually faster than doing a copy to an array
            // and manually computing the sum
            return System.Numerics.Vector.Dot(v, Vector<float>.One);
        }


        /// <summary>
        /// Sum an array
        /// </summary>
        /// <param name="array">The array to Sum</param>
        /// <param name="startIndex">The index to start summing from</param>
        /// <param name="length">The number of elements to add</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(float[] array, int startIndex, int length)
        {
            return Sum(new Span<float>(array, startIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(Span<float> array)
        {
            var vectorLength = array.Length / Vector<float>.Count;
            var remainder = array.Length % Vector<float>.Count;
            var vectorSpan = (array.Slice(0, array.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            Vector<float> acc1 = Vector<float>.Zero;
            Vector<float> acc2 = Vector<float>.Zero;
            for (; i < vectorSpan.Length - 1; i += 2)
            {
                acc1 += vectorSpan[i];
                acc2 += vectorSpan[i + 1];
            }
            i *= Vector<float>.Count;
            float sum = 0.0f;
            acc1 += acc2;
            for (; i < array.Length; i++)
            {
                sum += array[i];
            }
            return Sum(ref acc1) + sum;
        }

        /// <summary>
        /// Take the average of the absolute values
        /// </summary>
        /// <param name="first">The first vector</param>
        /// <param name="firstIndex">Where to start in the first vector</param>
        /// <param name="second">The second vector</param>
        /// <param name="secondIndex">Where to start in the second vector</param>
        /// <param name="length">The number of elements to read</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AbsDiffAverage(float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            return AbsDiffAverage(new Span<float>(first, firstIndex, length), new Span<float>(second, secondIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AbsDiffAverage(Span<float> first, Span<float> second)
        {
            if (first.Length == second.Length)
            {
                throw new ArgumentException("The length of the parameters are not the same!", nameof(second));
            }
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var acc1 = Vector<float>.Zero;
            var acc2 = Vector<float>.Zero;
            int i = 0;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                acc1 += System.Numerics.Vector.Abs(vectorFirst[i] - vectorSecond[i]);
                acc2 += System.Numerics.Vector.Abs(vectorFirst[i + 1] - vectorSecond[i + 1]);
            }
            i *= Vector<float>.Count;
            acc1 += acc2;
            float acc = 0.0f;
            for (; i < first.Length; i++)
            {
                acc += Math.Abs(first[i] - second[i]);
            }
            return (Sum(ref acc1) + acc) / first.Length;
        }

        /// <summary>
        /// Get the maximum difference from two arrays.
        /// </summary>
        /// <param name="first">The first vector</param>
        /// <param name="firstIndex">Where to start in the first vector</param>
        /// <param name="second">The second vector</param>
        /// <param name="secondIndex">Where to start in the second vector</param>
        /// <param name="length">The number of elements to read</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AbsDiffMax(float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            return AbsDiffMax(new Span<float>(first, firstIndex, length), new Span<float>(second, secondIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AbsDiffMax(Span<float> first, Span<float> second)
        {
            if (first.Length == second.Length)
            {
                throw new ArgumentException("The length of the parameters are not the same!", nameof(second));
            }
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            var acc1 = Vector<float>.Zero;
            var acc2 = Vector<float>.Zero;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                acc1 = System.Numerics.Vector.Max(System.Numerics.Vector.Abs(vectorFirst[i] - vectorSecond[i]), acc1);
                acc2 = System.Numerics.Vector.Max(System.Numerics.Vector.Abs(vectorFirst[i + 1] - vectorSecond[i + 1]), acc2);
            }
            i *= Vector<float>.Count;
            float maxAbsDiff = 0.0f;
            for (; i < first.Length; i++)
            {
                maxAbsDiff = Math.Max(Math.Abs(first[i] - second[i]), maxAbsDiff);
            }
            acc1 = System.Numerics.Vector.Max(acc1, acc2);
            float[] temp = new float[Vector<float>.Count];
            acc1.CopyTo(temp);
            for (int j = 0; j < temp.Length; j++)
            {
                maxAbsDiff = Math.Max(temp[j], maxAbsDiff);
            }
            return maxAbsDiff;
        }

        /// <summary>
        /// Sum the square differences of two arrays
        /// </summary>
        /// <param name="first">The array to Sum</param>
        /// <param name="firstIndex">The index to start summing from</param>
        /// <param name="second">The array to Sum</param>
        /// <param name="secondIndex">The index to start summing from</param>
        /// <param name="length">The number of elements to add</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SquareDiff(float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            return SquareDiff(new Span<float>(first, firstIndex, length), new Span<float>(second, secondIndex, length));
        }

        public static float SquareDiff(Span<float> first, Span<float> second)
        {
            if (first.Length == second.Length)
            {
                throw new ArgumentException("The length of the parameters are not the same!", nameof(second));
            }
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var acc1 = Vector<float>.Zero;
            var acc2 = Vector<float>.Zero;
            int i = 0;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                var diff1 = vectorFirst[i] - vectorSecond[i];
                var diff2 = vectorFirst[i + 1] - vectorSecond[i + 1];
                acc1 += diff1 * diff1;
                acc2 += diff2 * diff2;
            }
            acc1 += acc2;
            i *= Vector<float>.Count;
            var acc = 0.0f;
            for (; i < first.Length; i++)
            {
                var diff = first[i] - second[i];
                acc += diff * diff;
            }
            return Sum(ref acc1) + acc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(Span<float> dest, float value)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var vectorFirst = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vValue = new Vector<float>(value);
            int i = 0;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                vectorFirst[i] = vValue;
                vectorFirst[i + 1] = vValue;
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = value;
            }
        }

        /// <summary>
        /// Assign the given value to the whole array
        /// </summary>
        /// <param name="dest">The array to set</param>
        /// <param name="value">The value to set it to</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(float[] dest, float value)
        {
            Set(new Span<float>(dest), value);
        }

        /// <summary>
        /// Assign the given value to the whole array
        /// </summary>
        /// <param name="dest">The array to set</param>
        /// <param name="offset">The offset into the destination to start</param>
        /// <param name="value">The value to assign to it</param>
        /// <param name="length">The number of elements to assign</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(float[] dest, int offset, float value, int length)
        {
            Set(new Span<float>(dest, offset, length), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(Span<float> dest, Span<float> source)
        {
            if (dest.Length != source.Length)
            {
                throw new ArgumentException("The length of the parameters are not the same!", nameof(source));
            }
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSource = (source.Slice(0, source.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorDest.Length - 1; i += 2)
            {
                vectorDest[i] = System.Numerics.Vector.Abs(vectorSource[i]);
                vectorDest[i + 1] = System.Numerics.Vector.Abs(vectorSource[i + 1]);
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = Math.Abs(source[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(float[] dest, float[] source)
        {
            Abs(new Span<float>(dest), new Span<float>(source));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(float[][] dest, float[][] source)
        {
            for (int row = 0; row < dest.Length; row++)
            {
                Abs(dest[row], source[row]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MultiplyAndSum(Span<float> dest, Span<float> first, Span<float> second)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var acc1 = Vector<float>.Zero;
            var acc2 = Vector<float>.Zero;
            int i = 0;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                acc1 += (vectorDest[i] = vectorFirst[i] * vectorSecond[i]);
                acc2 += (vectorDest[i + 1] = vectorFirst[i + 1] * vectorSecond[i + 1]);
            }
            acc1 += acc2;
            float acc = 0.0f;
            for (; i < dest.Length; i++)
            {
                acc += (dest[i] = first[i] * second[i]);
            }
            return Sum(ref acc1) + acc;
        }

        /// <summary>
        /// Multiply the two vectors and store the results in the destination.  Return a running sum.
        /// </summary>
        /// <param name="destination">Where to save the data</param>
        /// <param name="destIndex">What index to start at</param>
        /// <param name="first">The first array to multiply</param>
        /// <param name="firstIndex">The index to start at</param>
        /// <param name="second">The second array to multiply</param>
        /// <param name="secondIndex">The index to start at for the second array</param>
        /// <param name="length">The amount of data to multiply</param>
        /// <returns>The sum of all of the multiplies</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MultiplyAndSum(float[] destination, int destIndex, float[] first, int firstIndex,
            float[] second, int secondIndex, int length)
        {
            return MultiplyAndSum(new Span<float>(destination, destIndex, length),
                new Span<float>(first, firstIndex, length),
                new Span<float>(second, secondIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MultiplyAndSum(Span<float> first, Span<float> second)
        {
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var acc1 = Vector<float>.Zero;
            var acc2 = Vector<float>.Zero;
            int i = 0;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                acc1 += vectorFirst[i] * vectorSecond[i];
                acc2 += vectorFirst[i + 1] * vectorSecond[i + 1];
            }
            acc1 += acc2;
            float acc = 0.0f;
            for (; i < first.Length; i++)
            {
                acc += first[i] * second[i];
            }
            return Sum(ref acc1) + acc;
        }

        /// <summary>
        /// Multiply the two vectors without storing the results but returning the total.
        /// </summary>
        /// <param name="first">The first array to multiply</param>
        /// <param name="firstIndex">The index to start at</param>
        /// <param name="second">The second array to multiply</param>
        /// <param name="secondIndex">The index to start at for the second array</param>
        /// <param name="length">The amount of data to multiply</param>
        /// <returns>The sum of all of the multiplies</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MultiplyAndSum(float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            return MultiplyAndSum(new Span<float>(first, firstIndex, length),
                new Span<float>(second, secondIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Multiply3AndSum(Span<float> first, Span<float> second, Span<float> third)
        {
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorThird = (third.Slice(0, third.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var acc1 = Vector<float>.Zero;
            var acc2 = Vector<float>.Zero;
            int i = 0;
            for (; i < vectorFirst.Length - 1; i += 2)
            {
                acc1 += vectorFirst[i] * vectorSecond[i] * vectorThird[i];
                acc2 += vectorFirst[i + 1] * vectorSecond[i + 1] * vectorThird[i + 1];
            }
            acc1 += acc2;
            float acc = 0.0f;
            for (; i < first.Length; i++)
            {
                acc += first[i] * second[i] * third[i];
            }
            return Sum(ref acc1) + acc;
        }

        /// <summary>
        /// Multiply the two vectors without storing the results but returning the total.
        /// </summary>
        /// <param name="first">The first array to multiply</param>
        /// <param name="firstIndex">The index to start at</param>
        /// <param name="second">The second array to multiply</param>
        /// <param name="secondIndex">The index to start at for the second array</param>
        /// <param name="thirdIndex"></param>
        /// <param name="length">The amount of data to multiply</param>
        /// <param name="third"></param>
        /// <returns>The sum of all of the multiplies</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Multiply3AndSum(float[] first, int firstIndex, float[] second, int secondIndex,
            float[] third, int thirdIndex, int length)
        {
            return Multiply3AndSum(new Span<float>(first, firstIndex, length),
                        new Span<float>(second, secondIndex, length),
                        new Span<float>(third, thirdIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply2Scalar1AndColumnSum(Span<float> dest, Span<float> first,
            Span<float> second, float scalar, Span<float> columnSum)
        {
            Vector<float> scalarV = new Vector<float>(scalar);
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorColumnSum = (columnSum.Slice(0, columnSum.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for(; i < vectorDest.Length - 1; i += 2)
            {
                vectorColumnSum[i] += (vectorDest[i] = vectorFirst[i] * vectorSecond[i] * scalarV);
                vectorColumnSum[i + 1] += (vectorDest[i + 1] = vectorFirst[i + 1] * vectorSecond[i + 1] * scalarV);
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                columnSum[i] += (dest[i] = first[i] * second[i] * scalar);
            }
        }

        /// <summary>
        /// Multiply the two vectors and store the results in the destination.  Return a running sum.
        /// </summary>
        /// <param name="destination">Where to save the data</param>
        /// <param name="destIndex">What index to start at</param>
        /// <param name="first">The first array to multiply</param>
        /// <param name="firstIndex">The index to start at</param>
        /// <param name="second">The second array to multiply</param>
        /// <param name="secondIndex">The index to start at for the second array</param>
        /// <param name="columnIndex"></param>
        /// <param name="length">The amount of data to multiply</param>
        /// <param name="scalar"></param>
        /// <param name="columnSum"></param>
        /// <returns>The sum of all of the multiplies</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply2Scalar1AndColumnSum(float[] destination, int destIndex, float[] first, int firstIndex,
            float[] second, int secondIndex, float scalar, float[] columnSum, int columnIndex, int length)
        {
            Multiply2Scalar1AndColumnSum(new Span<float>(destination, destIndex, length),
                new Span<float>(first, firstIndex, length),
                new Span<float>(second, secondIndex, length),
                scalar,
                new Span<float>(columnSum, columnIndex, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply3Scalar1AndColumnSum(Span<float> dest, Span<float> first,
            Span<float> second, Span<float> third, float scalar, Span<float> columnSum)
        {
            Vector<float> scalarV = new Vector<float>(scalar);
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorThird = (third.Slice(0, third.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorColumnSum = (columnSum.Slice(0, columnSum.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorDest.Length - 1; i += 2)
            {
                vectorColumnSum[i] += (vectorDest[i] = vectorFirst[i] * vectorSecond[i] * third[i] * scalarV);
                vectorColumnSum[i + 1] += (vectorDest[i + 1] = vectorFirst[i + 1] * vectorSecond[i + 1] * third[i + 1] * scalarV);
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                columnSum[i] += (dest[i] = first[i] * second[i] * third[i] * scalar);
            }
        }

        /// <summary>
        /// Multiply the two vectors and store the results in the destination.  Return a running sum.
        /// </summary>
        /// <param name="destination">Where to save the data</param>
        /// <param name="destIndex">What index to start at</param>
        /// <param name="first">The first array to multiply</param>
        /// <param name="firstIndex">The index to start at</param>
        /// <param name="second">The second array to multiply</param>
        /// <param name="secondIndex">The index to start at for the second array</param>
        /// <param name="columnIndex"></param>
        /// <param name="length">The amount of data to multiply</param>
        /// <param name="third"></param>
        /// <param name="thirdIndex"></param>
        /// <param name="scalar"></param>
        /// <param name="columnSum"></param>
        /// <returns>The sum of all of the multiplies</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply3Scalar1AndColumnSum(float[] destination, int destIndex, float[] first, int firstIndex,
            float[] second, int secondIndex, float[] third, int thirdIndex, float scalar, float[] columnSum, int columnIndex, int length)
        {
            Multiply3Scalar1AndColumnSum(new Span<float>(destination, destIndex, length),
                new Span<float>(first, firstIndex, length),
                new Span<float>(second, secondIndex, length),
                new Span<float>(third, thirdIndex, length),
                scalar,
                new Span<float>(columnSum, columnIndex, length));
        }

        public static void Average(Span<float> dest, Span<float> first, Span<float> second)
        {
            Vector<float> half = new Vector<float>(0.5f);
            var vectorLength = first.Length / Vector<float>.Count;
            var remainder = first.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorFirst = (first.Slice(0, first.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorSecond = (second.Slice(0, second.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorDest.Length - 1; i += 2)
            {
                vectorDest[i] = (vectorFirst[i] + vectorSecond[i]) * half;
                vectorDest[i + 1] = (vectorFirst[i + 1] + vectorSecond[i + 1]) * half;
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = (first[i] + second[i]) * 0.5f;
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
        public static void Average(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            Average(new Span<float>(destination, destIndex, length),
                new Span<float>(first, firstIndex, length),
                new Span<float>(second, secondIndex, length));
        }

        /// <summary>
        /// Produce a new vector selecting the original value if it is finite.  If it is not,
        /// select the alternative value.
        /// </summary>
        /// <param name="baseValues">The values to test for their finite property</param>
        /// <param name="alternateValues">The values to replace if the base value is not finite</param>
        /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<float> SelectIfFinite(Vector<float> baseValues, Vector<float> alternateValues)
        {
            //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
            return System.Numerics.Vector.ConditionalSelect(
                System.Numerics.Vector.BitwiseAnd(System.Numerics.Vector.LessThanOrEqual(System.Numerics.Vector.Abs(baseValues), MaxFloat), System.Numerics.Vector.GreaterThanOrEqual(baseValues, baseValues)),
                baseValues, alternateValues
                );
        }

        /// <summary>
        /// Produce a new vector selecting the original value if it is finite.  If it is not,
        /// select the alternative value.
        /// </summary>
        /// <param name="baseValues">The values to test for their finite property</param>
        /// <param name="alternateValues">The values to replace if the base value is not finite</param>
        /// <param name="minimumV"></param>
        /// <returns>A new vector containing the proper mix of the base and alternate values</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<float> SelectIfFiniteAndLessThan(Vector<float> baseValues, Vector<float> alternateValues, Vector<float> minimumV)
        {
            //If it is greater than the maximum value it is infinite, if it is not equal to itself it is NaN
            return System.Numerics.Vector.ConditionalSelect(
                System.Numerics.Vector.BitwiseAnd(System.Numerics.Vector.BitwiseAnd(System.Numerics.Vector.LessThanOrEqual(System.Numerics.Vector.Abs(baseValues), MaxFloat),
                System.Numerics.Vector.GreaterThanOrEqual(baseValues, baseValues)), System.Numerics.Vector.GreaterThanOrEqual(baseValues, minimumV)),
                baseValues, alternateValues
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceIfNotFinite(Span<float> dest, float alternateValue)
        {
            Vector<float> altV = new Vector<float>(alternateValue);
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorDest.Length - 1; i += 2)
            {
                vectorDest[i] = SelectIfFinite(vectorDest[i], altV);
                vectorDest[i + 1] = SelectIfFinite(vectorDest[i + 1], altV);
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                if (float.IsNaN(dest[i]) || float.IsInfinity(dest[i]))
                {
                    dest[i] = alternateValue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="destIndex"></param>
        /// <param name="alternateValue"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceIfNotFinite(float[] destination, int destIndex, float alternateValue, int length)
        {
            ReplaceIfNotFinite(new Span<float>(destination, destIndex, length), alternateValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceIfLessThanOrNotFinite(Span<float> dest, float alternateValue, float minimum)
        {
            Vector<float> altV = new Vector<float>(alternateValue);
            Vector<float> minV = new Vector<float>(minimum);
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorDest.Length - 1; i += 2)
            {
                vectorDest[i] = SelectIfFiniteAndLessThan(vectorDest[i], altV, minV);
                vectorDest[i + 1] = SelectIfFiniteAndLessThan(vectorDest[i + 1], altV, minV);
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                if (float.IsInfinity(dest[i]) || !(dest[i] >= minimum))
                {
                    dest[i] = alternateValue;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceIfLessThanOrNotFinite(float[] destination, int destIndex, float alternateValue, float minimum, int length)
        {
            ReplaceIfLessThanOrNotFinite(new Span<float>(destination, destIndex, length), alternateValue, minimum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyGreaterThan(Span<float> data, float rhs)
        {
            var rhsV = new Vector<float>(rhs);
            var vectorLength = data.Length / Vector<float>.Count;
            var remainder = data.Length % Vector<float>.Count;
            var vectorData = (data.Slice(0, data.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorData.Length - 1; i += 2)
            {
                if(System.Numerics.Vector.GreaterThanAny(vectorData[i], rhsV)
                    | System.Numerics.Vector.GreaterThanAny(vectorData[i + 1], rhsV))
                {
                    return true;
                }
            }
            i *= Vector<float>.Count;
            for (; i < data.Length; i++)
            {
                if(data[i] > rhs)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyGreaterThan(float[] data, int dataIndex, float rhs, int length)
        {
            return AnyGreaterThan(new Span<float>(data, dataIndex, length), rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreBoundedBy(Span<float> data, float baseNumber, float maxVarriation)
        {
            var baseV = new Vector<float>(baseNumber);
            var maxmumVariationV = new Vector<float>(maxVarriation);
            var vectorLength = data.Length / Vector<float>.Count;
            var remainder = data.Length % Vector<float>.Count;
            var vectorData = (data.Slice(0, data.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorData.Length - 1; i += 2)
            {
                if (System.Numerics.Vector.GreaterThanAny(System.Numerics.Vector.Abs(vectorData[i] - baseV), maxmumVariationV)
                    | System.Numerics.Vector.GreaterThanAny(System.Numerics.Vector.Abs(vectorData[i + 1] - baseV), maxmumVariationV))
                {
                    return true;
                }
            }
            i *= Vector<float>.Count;
            for (; i < data.Length; i++)
            {
                if (Math.Abs(data[i] - baseNumber) > maxVarriation)
                {
                    return true;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreBoundedBy(float[] data, int dataIndex, float baseNumber, float maxVarriation, int length)
        {
            return AreBoundedBy(new Span<float>(data, dataIndex, length), baseNumber, maxVarriation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(Span<float> dest, Span<float> x)
        {
            for (int i = 0; i < dest.Length; i++)
            {
                dest[i] = (float)Math.Exp(x[i]);
            }
        }

        /// <summary>
        /// Applies exp(x) for each element in the array
        /// </summary>
        /// <param name="destination">Where to save the results.</param>
        /// <param name="destIndex">An offset into the array to start saving.</param>
        /// <param name="x">The vector to use as the exponent.</param>
        /// <param name="xIndex">The offset into the exponent vector to start from.</param>
        /// <param name="length">The number of elements to convert.</param>
        /// <remarks>The series is unrolled 30 times which approximates the .Net implementation from System.Math.Exp</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(float[] destination, int destIndex, float[] x, int xIndex, int length)
        {
            Exp(new Span<float>(destination, destIndex, length), new Span<float>(x, xIndex, length));
        }

        /// <summary>
        /// Computes the Arithmetic Geometric mean for the given values.
        /// </summary>
        /// <param name="x">The first parameter vector. This parameter must be non negative!</param>
        /// <param name="y">The second parameter vector. This parameter must be non negative!</param>
        /// <seealso>
        ///     <cref>https://en.wikipedia.org/wiki/Arithmetic–geometric_mean</cref>
        /// </seealso>
        /// <returns>The AGM for each element in the parameters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<float> ArithmeticGeometricMean(Vector<float> x, Vector<float> y)
        {
            var half = new Vector<float>(0.5f);
            var a = half * (x + y);
            var g = System.Numerics.Vector.SquareRoot(x * y);
            // 5 expansions seems to be sufficient for 32-bit floating point numbers
            for (int i = 0; i < 5; i++)
            {
                var tempA = half * (a + g);
                g = System.Numerics.Vector.SquareRoot(a * g);
                a = tempA;
            }
            return a;
        }

        /// <summary>
        /// Computes the natural logarithm for each element in x
        /// </summary>
        /// <param name="x">The values to compute the logarithms of</param>
        /// <returns>The vector of logarithms</returns>
        /// <see>
        ///     <cref>https://en.wikipedia.org/wiki/Natural_logarithm</cref>
        /// </see>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<float> Log(Vector<float> x)
        {
            var two = new Vector<float>(2.0f);
            var pi = new Vector<float>((float)Math.PI);
            var mTimesln2 = new Vector<float>(0.693147181f * 16.0f);
            var denom = new Vector<float>(4.0f) / (x * new Vector<float>(65536.0f));
            return (pi / (two * ArithmeticGeometricMean(Vector<float>.One, denom))) - mTimesln2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(Span<float> dest, Span<float> x)
        {
            var vectorLength = dest.Length / Vector<float>.Count;
            var remainder = dest.Length % Vector<float>.Count;
            var vectorDest = (dest.Slice(0, dest.Length - remainder)).NonPortableCast<float, Vector<float>>();
            var vectorX = (x.Slice(0, x.Length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorDest.Length - 1; i += 2)
            {
                vectorDest[i] = Log(vectorX[i]);
                vectorDest[i + 1] = Log(vectorX[i + 1]);
            }
            i *= Vector<float>.Count;
            for (; i < dest.Length; i++)
            {
                dest[i] = (float)Math.Log(x[i]);
            }
        }

        /// <summary>
        /// Applies log(x) for each element in the array and saves it into the destination.
        /// </summary>
        /// <param name="destination">Where to save the results.</param>
        /// <param name="destIndex">An offset into the array to start saving.</param>
        /// <param name="x">The vector to take the log of.</param>
        /// <param name="xIndex">The offset into the array to start from.</param>
        /// <param name="length">The number of elements to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(float[] destination, int destIndex, float[] x, int xIndex, int length)
        {
            Log(new Span<float>(destination, destIndex, length),
                new Span<float>(x, xIndex, length));
        }
    }
}
