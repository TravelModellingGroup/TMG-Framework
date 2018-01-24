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

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Numerics.Vector;
namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void If(float[] destination, int destIndex, float[] cond, int condIndex, float[] ifTrue, int trueIndex, float[] ifFalse, int falseIndex, int length)
        {
            if ((destIndex | condIndex | trueIndex | falseIndex) == 0)
            {
                int i;
                var zero = Vector<float>.Zero;
                for(i = 0; i < destination.Length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    ConditionalSelect(GreaterThan(new Vector<float>(cond, i), zero), new Vector<float>(ifTrue, i), new Vector<float>(ifFalse, i)).CopyTo(destination, i);
                }
                for (i = 0; i < destination.Length; ++i)
                {
                    destination[i] = cond[i] > 0f ? ifTrue[i] : ifFalse[i];
                }
            }
            else
            {
                int i;
                var zero = Vector<float>.Zero;
                for (i = 0; i < destination.Length - Vector<float>.Count; i += Vector<float>.Count)
                {
                    ConditionalSelect(GreaterThan(new Vector<float>(cond, condIndex + i), zero), new Vector<float>(ifTrue, trueIndex + i), new Vector<float>(ifFalse, falseIndex + i)).CopyTo(destination, destIndex + i);
                }
                for (i = 0; i < destination.Length; ++i)
                {
                    destination[destIndex + i] = cond[condIndex + i] > 0f ? ifTrue[trueIndex + i] : ifFalse[falseIndex + i];
                }
            }
        }
    }
}
