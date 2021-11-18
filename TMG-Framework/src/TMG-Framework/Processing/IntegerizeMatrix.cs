/*
    Copyright 2021 University of Toronto

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
using TMG;

namespace TMG.Processing
{
    [Module(Name = "Integerize Matrix", Description = "Takes in a matrix and then integerizes it trying to keep the total by planning district.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class IntegerizeMatrix : BaseFunction<Matrix>
    {
        [SubModule(Required = true, Index = 0, Name = "Input Matrix", Description = "The matrix that will be integerized.")]
        public IFunction<Matrix> InputMatrix;

        [Parameter(Index = 1, Name = "Random Seed", Description = "The number used to initialize the random number generator.")]
        public int RandomSeed;

        [SubModule(Required = true, Index = 2, Name = "Zone To PD Map", Description = "A mapping between zone numbers and")]
        public IFunction<CategoryMap> ZoneToPDMap;

        public override Matrix Invoke()
        {
            var baseMatrix = InputMatrix!.Invoke();
            throw new NotImplementedException();
        }
    }
}
