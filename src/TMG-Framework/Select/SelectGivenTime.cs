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

namespace TMG.Select
{
    [Module(Name = "Select Matrix By Time Period", Description = "Get a piece of data given a time period.  If no results are possible the first data is returned.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class SelectGivenTime<T> : BaseFunction<Time, T>
    {
        [SubModule(Index = 0, Name = "Time Period Data", Required = true, Description = "The possible results for given time periods")]
        public IFunction<(TimePeriod period, T data)>[] TimePeriodData;

        [Parameter(Index = 1, Name = "Default Time Period", DefaultValue = "0", Required = true, Description = "The time period to select if no time periods are applicable.")]
        public IFunction<int> DefaultTimePeriod;

        private TimePeriod[] _TimeOperations;

        public override T Invoke(Time context)
        {
            var local = _TimeOperations;
            if (local != null)
            {
                for (int i = 0; i < local.Length; i++)
                {
                    if (local[i].Contains(context))
                    {
                        return TimePeriodData[i].Invoke().data;
                    }
                }
                return ReturnDefaultData();
            }
            return BuildIndexAndReturnResult(context);
        }

        private T ReturnDefaultData()
        {
            var index = DefaultTimePeriod.Invoke();
            if (index < 0)
            {
                throw new XTMFRuntimeException(this, "Unable to use a default index less than zero!");
            }
            else if (index >= TimePeriodData.Length)
            {
                throw new XTMFRuntimeException(this, "Unable to use a default index higher than the number of time periods!");
            }
            return TimePeriodData[index].Invoke().data;
        }

        private T BuildIndexAndReturnResult(Time context)
        {
            var local = new TimePeriod[TimePeriodData.Length];
            for (int i = 0; i < TimePeriodData.Length; i++)
            {
                var res = TimePeriodData[i].Invoke();
                local[i] = res.period;
            }
            _TimeOperations = local;
            return Invoke(context);
        }

        public override bool RuntimeValidation(ref string error)
        {
            if (TimePeriodData.Length <= 0)
            {
                error = "You must have at least one matrix available!";
                return false;
            }
            return true;
        }
    }
}
