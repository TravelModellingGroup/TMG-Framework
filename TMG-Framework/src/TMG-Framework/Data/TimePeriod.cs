/*
    Copyright 2017 University of Toronto

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

namespace TMG
{
    /// <summary>
    /// This class represents an identified range of time [Start, End).
    /// </summary>
    public struct TimePeriod
    {
        /// <summary>
        /// The start of the time period
        /// </summary>
        public Time Start { get; private set; }

        /// <summary>
        /// The end of the time period
        /// </summary>
        public Time End { get; private set; }

        /// <summary>
        /// Construct a time period from the given times.
        /// </summary>
        /// <param name="start">The start time of the time period.(Inclusive)</param>
        /// <param name="end">THe end time of the time period. (Exclusive)</param>
        public TimePeriod(Time start, Time end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Is the given time within this time period.
        /// </summary>
        /// <param name="time">The time to test for.</param>
        /// <returns>True if it is in the time period, false otherwise.</returns>
        public bool Contains(Time time) => (Start <= time) & (time < End);
    }
}
