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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        public static void Add(float[] dest, float[] source, float scalar)
        {
            Vector<float> constant = new Vector<float>(scalar);
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i <= source.Length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var dynamic = new Vector<float>(source, i);
                (constant + dynamic).CopyTo(dest, i);
            }
            // copy the remainder
            for (; i < source.Length; i++)
            {
                dest[i] = source[i] + scalar;
            }
        }

        public static void Add(float[] dest, int destIndex, float[] source, int sourceIndex, float scalar, int length)
        {
            Vector<float> constant = new Vector<float>(scalar);
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i <= length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var dynamic = new Vector<float>(source, sourceIndex + i);
                (constant + dynamic).CopyTo(dest, destIndex + i);
            }
            // copy the remainder
            for (; i < length; i++)
            {
                dest[destIndex + i] = source[sourceIndex + i] + scalar;
            }
        }


        public static void Add(float[] dest, float[] lhs, float[] rhs)
        {
            // copy everything we can do inside of a vector
            int i = 0;
            for (; i <= lhs.Length - Vector<float>.Count; i += Vector<float>.Count)
            {
                var lhsV = new Vector<float>(lhs, i);
                var rhsV = new Vector<float>(rhs, i);
                (lhsV + rhsV).CopyTo(dest, i);
            }
            // copy the remainder
            for (; i < lhs.Length; i++)
            {
                dest[i] = lhs[i] + rhs[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, float[] third, int thirdIndex, int length)
        {
            if ((destIndex | firstIndex | secondIndex | thirdIndex) == 0)
            {
                int i = 0;
                for (; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var f = new Vector<float>(first, i);
                    var s = new Vector<float>(second, i);
                    var t = new Vector<float>(third, i);
                    (f + s + t).CopyTo(destination, i);
                }
                // copy the remainder
                for (; i < length; i++)
                {
                    destination[i] = first[i] + second[i] + third[i];
                }
            }
            else
            {
                for (int i = 0; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    (new Vector<float>(first, i + firstIndex) + new Vector<float>(second, i + secondIndex) + new Vector<float>(third, i + thirdIndex)).CopyTo(destination, i + destIndex);
                }
                // copy the remainder
                for (int i = length - (length % Vector<float>.Count); i < length; i++)
                {
                    destination[i + destIndex] = first[i + firstIndex] + second[i + secondIndex] + third[i + thirdIndex];

                }
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
        public static void Add(float[] destination, int destIndex, float[] first, int firstIndex, float[] second, int secondIndex, int length)
        {
            if ((destIndex | firstIndex | secondIndex) == 0)
            {
                int i = 0;
                for (; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var f = new Vector<float>(first, i);
                    var s = new Vector<float>(second, i);
                    (f + s).CopyTo(destination, i);
                }
                // copy the remainder
                for (; i < length; i++)
                {
                    destination[i] = first[i] + second[i];
                }
            }
            else
            {
                for (int i = 0; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    (new Vector<float>(first, i + firstIndex) + new Vector<float>(second, i + secondIndex)).CopyTo(destination, i + destIndex);
                }
                // copy the remainder
                for (int i = length - (length % Vector<float>.Count); i < length; i++)
                {
                    destination[i + destIndex] = first[i + firstIndex] + second[i + secondIndex];
                }
            }
        }

        public static void Add(float[][] destination, float lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Vector<float> n = new Vector<float>(lhs);
                var dest = destination[row];
                var length = dest.Length;
                var denom = rhs[row];
                // copy everything we can do inside of a vector
                int i = 0;
                for (; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var d = new Vector<float>(denom, i);
                    (n + d).CopyTo(dest, i);
                }
                // copy the remainder
                for (; i < length; i++)
                {
                    dest[i] = lhs + denom[i];
                }
            });
        }

        public static void Add(float[][] destination, float[][] lhs, float rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                Vector<float> d = new Vector<float>(rhs);
                var dest = destination[row];
                var length = dest.Length;
                var num = lhs[row];
                // copy everything we can do inside of a vector
                int i = 0;
                for (; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var n = new Vector<float>(num, i);
                    (n + d).CopyTo(dest, i);
                }
                // copy the remainder
                for (; i < length; i++)
                {
                    dest[i] = num[i] + rhs;
                }
            });
        }

        public static void Add(float[][] destination, float[][] lhs, float[][] rhs)
        {
            Parallel.For(0, destination.Length, row =>
            {
                var dest = destination[row];
                var length = dest.Length;
                var num = lhs[row];
                var denom = rhs[row];
                    // copy everything we can do inside of a vector
                    int i = 0;
                for (; i <= length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    var n = new Vector<float>(num, i);
                    var d = new Vector<float>(denom, i);
                    (n + d).CopyTo(dest, i);
                }
                    // copy the remainder
                    for (; i < length; i++)
                {
                    dest[i] = num[i] + denom[i];
                }
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
