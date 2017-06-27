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
    [Module(Name = "Evaluate Matrix", Description = "Evaluates a matrix given the expression.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class EvaluateMatrix : BaseFunction<Matrix>
    {
        [Parameter(Name = "Expression", Index = 0, Description = "The expression to compute using the following variables.")]
        public IFunction<string> Expression;

        [SubModule(Name = "Variables", Description = "The variables to use in our expression", Index = 1)]
        public IModule[] Variables;

        private string PreviousExpressionString = null;
        private Expression PreviousExpression = null;

        public override Matrix Invoke()
        {
            string error = null;
            // compile and optimize the expression
            var expressionString = Expression.Invoke();
            if (PreviousExpression == null || expressionString != PreviousExpressionString)
            {
                PreviousExpressionString = expressionString;
                if (!TMG.Frameworks.Data.Processing.AST.Compiler.Compile(expressionString, out var expression, ref error))
                {
                    throw new XTMFRuntimeException(this, error);
                }
                PreviousExpression = expression;
            }
            var result = PreviousExpression.Evaluate(Variables);
            if(result.Error)
            {
                throw new XTMFRuntimeException(this, result.ErrorMessage);
            }
            // this easy case, the expression was of the correct type
            if(result.IsOdResult)
            {
                return result.OdData;
            }
            // if the result ended up being a
            if(result.IsVectorResult)
            {
                throw new XTMFRuntimeException(this, "The expression resulted in a vector instead of a matrix!");
            }
            throw new XTMFRuntimeException(this, "The expression resulted in a scalar instead of a matrix!");
        }

        public override bool RuntimeValidation(ref string error)
        {
            foreach(var module in Variables)
            {
                if(!(module is IFunction<Matrix> || module is IFunction<Vector> || module is IFunction<float>
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
