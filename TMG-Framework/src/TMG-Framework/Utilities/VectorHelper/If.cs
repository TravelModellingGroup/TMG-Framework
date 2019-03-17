/*
    Copyright 2018 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF2.

    XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Numerics.Vector;
namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void If(float[] dest, int destIndex, float[] cond, int condIndex, float[] ifTrue, int trueIndex, float[] ifFalse, int falseIndex, int length)
        {
            var zero = Vector<float>.Zero;
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var condSpan = (new Span<float>(cond, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var trueSpan = (new Span<float>(ifTrue, trueIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var falseSpan = (new Span<float>(ifFalse, falseIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            int i = 0;
            for (; i < vectorLength; i+=2)
            {
                destSpan[i] = ConditionalSelect(GreaterThan(condSpan[i], zero), trueSpan[i], falseSpan[i]);
                destSpan[i + 1] = ConditionalSelect(GreaterThan(condSpan[i + 1], zero), trueSpan[i + 1], falseSpan[i + 1]);
            }
            i *= Vector<float>.Count;
            for(; i < length; i++)
            {
                dest[destIndex + i] = cond[condIndex + i] > 0f ? ifTrue[trueIndex + i] : ifFalse[falseIndex + i];
            }
        }
    }
}
