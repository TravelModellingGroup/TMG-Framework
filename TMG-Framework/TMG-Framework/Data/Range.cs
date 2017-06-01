/*
    Copyright 2014-2017 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
using System.Collections.Generic;

namespace TMG
{
    public struct Range
    {
        public readonly int Start;
        public readonly int Stop;

        public Range(int start, int stop)
        {
            Start = start;
            Stop = stop;
        }

        public static bool operator !=(Range first, Range other)
        {
            return (first.Start != other.Start) | (first.Stop != other.Stop);
        }

        public static bool operator ==(Range first, Range other)
        {
            return (first.Start == other.Start) & (first.Stop == other.Stop);
        }

        public override bool Equals(object obj)
        {
            if (obj is Range other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() * Stop.GetHashCode();
        }

        /// <summary>
        /// Checks if a given value is inside the range defined by [Start, Stop)
        /// </summary>
        /// <param name="i"> The int value to check.</param>
        /// <returns>True IFF i is greater than or equal to Start and i is less than Stop.</returns>
        public bool Contains(int i)
        {
            return (i >= Start) & (i < Stop);
        }

        /// <summary>
        /// Checks if a given value is inside the range defined by (Start, Stop)
        /// </summary>
        /// <param name="i">The int value to check</param>
        /// <returns>True IFF i is less than Start and i is less than Stop.</returns>
        public bool ContainsExcusive(int i)
        {
            return (i > Start) & (i < Stop);
        }

        /// <summary>
        /// Checks if a given value is inside the range defined by (Start, Stop)
        /// </summary>
        /// <param name="valueToFind">The value to check for</param>
        /// <returns>True if the value is contained within the range</returns>
        public bool ContainsExcusive(float valueToFind)
        {
            return (valueToFind > Start) & (valueToFind < Stop);
        }

        /// <summary>
        /// Checks if a given value is inside the range defined by [Start, Stop]
        /// </summary>
        /// <param name="i">The int value to check</param>
        /// <returns>True IFF i is greater than or equal to Start and i is less than or equal to Stop.</returns>
        public bool ContainsInclusive(int i)
        {
            return (i >= Start) & (i <= Stop);
        }

        /// <summary>
        /// Checks if a given value is inside the range defined by [Start, Stop]
        /// </summary>
        /// <param name="valueToFind">The value to check for</param>
        /// <returns>True if the value is contained within the range</returns>
        public bool ContainsInclusive(float valueToFind)
        {
            return (valueToFind >= Start) & (valueToFind <= Stop);
        }

        /// <summary>
        /// Checks if another Range overlaps this one.
        /// </summary>
        /// <param name="other">The other range to check against.</param>
        /// <returns></returns>
        public bool Overlaps(Range other)
        {
            return ContainsInclusive(other.Start) || ContainsInclusive(other.Stop);
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", Start, Stop);
        }
    }
}