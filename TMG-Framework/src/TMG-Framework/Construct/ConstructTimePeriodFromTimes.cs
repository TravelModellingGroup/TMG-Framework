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
using XTMF2;

namespace TMG.Construct
{
    [Module(Name = "Construct Time Period from Times", Description = "Given the start and end times construct a time period.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class ConstructTimePeriodFromTimes : BaseFunction<TimePeriod>
    {
        [Parameter(Index = 0, Name = "Start Time", Required = true, Description = "The time to use as the starting point of the time period (Inclusive).")]
        public IFunction<Time> StartTime;

        [Parameter(Index = 1, Name = "End Time", Required = true, Description = "The time to use as the ending point of the time period (Exclusive).")]
        public IFunction<Time> EndTime;

        public override TimePeriod Invoke()
        {
            return new TimePeriod(StartTime.Invoke(), EndTime.Invoke());
        }
    }
}
