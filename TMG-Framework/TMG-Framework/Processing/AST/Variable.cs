/*
    Copyright 2016 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
using System.Linq;
using XTMF2;

namespace TMG.Frameworks.Data.Processing.AST
{
    public sealed class Variable : Value
    {
        public readonly string Name;

        public Variable(int start, string name) : base(start)
        {
            Name = name;
        }

        public override ComputationResult Evaluate(IModule[] dataSources)
        {
            var source = dataSources.FirstOrDefault(d => d.Name == Name);
            if (source == null)
            {
                return new ComputationResult("Unable to find a data source named '" + Name + "'!");
            }
            if (source is IFunction<SparseMatrix> odSource)
            {
                return new ComputationResult(odSource.Invoke(), false);
            }
            if (source is IFunction<SparseVector> vectorSource)
            {
                return new ComputationResult(vectorSource.Invoke(), false);
            }
            if (source is IFunction<float> valueSource)
            {
                return new ComputationResult(valueSource.Invoke());
            }
            return new ComputationResult("The data source '" + Name + "' was not of a valid resource type!");
        }
    }
}
