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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace TMG
{
    static class SpanHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Span<U> NonPortableCast<T,U>(this Span<T> span) 
            where T : struct
            where U : struct
        {
            return MemoryMarshal.Cast<T,U>(span);
        }
    }
}
