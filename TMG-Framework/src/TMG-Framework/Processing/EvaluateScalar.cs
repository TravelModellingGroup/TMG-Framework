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
using TMG.Frameworks.Data.Processing.AST;

namespace TMG.Processing
{
    [Module(Name = "Evaluate Scalar", Description = "Evaluate a scalar given the expression.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class EvaluateScalar : BaseFunction<float>
    {
        [Parameter(Name = "Expression", Index = 0, Description = "The expression to compute using the following variables.")]
        public IFunction<string> Expression;

        [SubModule(Name = "Variables", Description = "The variables to use in our expression", Index = 1)]
        public IModule[] Variables;

        public override float Invoke()
        {
            string error = null;
            // compile and optimize the expression
            if (!TMG.Frameworks.Data.Processing.AST.Compiler.Compile(Expression.Invoke(), out var expression, ref error))
            {
                throw new XTMFRuntimeException(this, error);
            }
            var result = expression.Evaluate(Variables);
            if (result.Error)
            {
                throw new XTMFRuntimeException(this, result.ErrorMessage);
            }
            if (result.IsVectorResult)
            {
                throw new XTMFRuntimeException(this, "The expression resulted in a vector instead of a scalar!");
            }
            if (result.IsOdResult)
            {
                throw new XTMFRuntimeException(this, "The expression resulted in a matrix instead of a scalar!");
            }
            return result.LiteralValue;
            
        }

        public override bool RuntimeValidation(ref string error)
        {
            foreach (var module in Variables)
            {
                if (!(module is IFunction<Matrix> || module is IFunction<Vector> || module is IFunction<float>
                    || module is IFunction<Categories>))
                {
                    error = $"Invalid variable module type {module.GetType().Name} from module {module.Name}!";
                    return false;
                }
            }
            return true;
        }
    }
}
