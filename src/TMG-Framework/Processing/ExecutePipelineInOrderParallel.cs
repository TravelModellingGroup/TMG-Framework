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
using System.Linq;
using XTMF2;

namespace TMG.Processing
{
    [Module(Name = "Execute Pipeline In Order Parallel", Description = "Execute a given pipeline",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class ExecutePipelineInOrderParallel<T> : BaseFunction<IEnumerable<T>, IEnumerable<T>>
    {
        [SubModule(Index = 0, Name = "To Execute In Parallel", Required = true, Description = "The functions in order to execute the data through in parallel.")]
        public IFunction<T, T> ToExecuteInParallel;

        [SubModule(Index = 1, Name = "To Execute In Serial", Required = false, Description = "The functions in order to execute the data through in parallel.")]
        public IFunction<T,T>[] ToExecuteNotInParallel;

        public override IEnumerable<T> Invoke(IEnumerable<T> context)
        {
            var current = context.AsParallel().AsOrdered().Select(element => ToExecuteInParallel.Invoke(element)).AsSequential();
            for (int i = 0; i < ToExecuteNotInParallel.Length; i++)
            {
                int localI = i;
                current = current.Select(element => ToExecuteNotInParallel[localI].Invoke(element));
            }
            return current.AsEnumerable();
        }
    }
}
