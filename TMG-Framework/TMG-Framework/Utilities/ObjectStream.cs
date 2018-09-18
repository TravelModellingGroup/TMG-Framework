/*
    Copyright 2018 University of Toronto

    This file is part of TMG-Framework for XTMF2.

    TMG-Framework for XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TMG-Framework for XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TMG-Framework for XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;
using XTMF2;

namespace TMG
{
    public abstract class ObjectStream<OUT> : BaseFunction<OUT>
    {
        /// <summary>
        /// Invoke this function to signal that the stream should
        /// restart the stream.
        /// </summary>
        public abstract void Reset();
    }

    public abstract class ObjectStream<IN,OUT> : BaseFunction<IN,OUT>
    {
        /// <summary>
        /// Invoke this function to signal that the stream should
        /// restart the stream.
        /// </summary>
        public abstract void Reset();
    }
}
