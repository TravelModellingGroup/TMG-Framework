/*
    Copyright 2018 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF2.

    XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/

using TMG.Utilities;

namespace TMG.Frameworks.Data.Processing.AST;

public sealed class Negate : MonoExpression
{
    public Negate(int start) : base(start)
    {
    }

    internal override bool OptimizeAst(ref Expression ex,
        [NotNullWhen(false)] ref string? error)
    {
        if (InnerExpression is null)
        {
            error = "Unable to optimize Negate with null operand starting at position " + Start + "!";
            return false;
        }

        // Optimize our children first
        if (!InnerExpression.OptimizeAst(ref InnerExpression, ref error))
        {
            return false;
        }
        // optimize the case that we are a negative literal
        if (ex is Literal l)
        {
            ex = new Literal(Start, -l.Value);
        }
        return true;
    }

    public override ComputationResult Evaluate(IModule[] dataSources)
    {
        if (InnerExpression is null)
        {
            return new ComputationResult("Unable to evaluate Negate with null operand starting at position " + Start + "!");
        }

        var inner = InnerExpression.Evaluate(dataSources);
        if (inner.IsValue)
        {
            return new ComputationResult(-inner.LiteralValue);
        }
        else if (inner.IsVectorResult)
        {
            var ret = inner.Accumulator ? inner.VectorData : new Vector(inner.VectorData);
            VectorHelper.Negate(ret.Data, inner.VectorData.Data);
            return new ComputationResult(ret, true, inner.Direction);
        }
        else
        {
            var ret = inner.Accumulator ? inner.OdData : new Matrix(inner.OdData);
            var flatRet = ret.Data;
            var flatInner = inner.OdData.Data;
            VectorHelper.Negate(flatRet, flatInner);
            return new ComputationResult(ret, true);
        }
    }
}
