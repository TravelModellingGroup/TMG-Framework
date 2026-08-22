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

using TMG.Utilities;

namespace TMG.Frameworks.Data.Processing.AST;

public sealed class Subtract : BinaryExpression
{
    public Subtract(int start) : base(start)
    {

    }

    internal override bool OptimizeAst(ref Expression ex,
        [NotNullWhen(false)] ref string? error)
    {
        if (!base.OptimizeAst(ref ex, ref error))
        {
            return false;
        }
        var lhs = Lhs as Literal;
        var rhs = Rhs as Literal;
        if (lhs != null && rhs != null)
        {
            ex = new Literal(Start, lhs.Value - rhs.Value);
        }
        return true;
    }

    public override ComputationResult Evaluate(ComputationResult lhs, ComputationResult rhs)
    {
        // see if we have two values, in this case we can skip doing the matrix operation
        if (lhs.IsValue && rhs.IsValue)
        {
            return new ComputationResult(lhs.LiteralValue - rhs.LiteralValue);
        }
        // float / matrix
        if (lhs.IsValue)
        {
            if (rhs.IsVectorResult)
            {
                var retVector = rhs.Accumulator ? rhs.VectorData : new Vector(rhs.VectorData);
                var flat = retVector.Data;
                VectorHelper.Subtract(flat, lhs.LiteralValue, rhs.VectorData.Data);
                return new ComputationResult(retVector, true);
            }
            else
            {
                var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData);
                VectorHelper.Subtract(retMatrix.Data, lhs.LiteralValue, rhs.OdData.Data);
                return new ComputationResult(retMatrix, true);
            }
        }
        else if (rhs.IsValue)
        {
            if (lhs.IsVectorResult)
            {
                var retVector = lhs.Accumulator ? lhs.VectorData : new Vector(lhs.VectorData);
                var flat = retVector.Data;
                VectorHelper.Subtract(flat, lhs.VectorData.Data, rhs.LiteralValue);
                return new ComputationResult(retVector, true);
            }
            else
            {
                // matrix / float
                var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                VectorHelper.Subtract(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue);
                return new ComputationResult(retMatrix, true);
            }
        }
        else
        {
            if (lhs.IsVectorResult || rhs.IsVectorResult)
            {
                if (lhs.IsVectorResult && rhs.IsVectorResult)
                {
                    var retMatrix = lhs.Accumulator ? lhs.VectorData : (rhs.Accumulator ? rhs.VectorData : new Vector(lhs.VectorData));
                    VectorHelper.Subtract(retMatrix.Data, lhs.VectorData.Data, rhs.VectorData.Data);
                    return new ComputationResult(retMatrix, true, lhs.Direction);
                }
                else if (lhs.IsVectorResult)
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData);
                    var flatLhs = lhs.VectorData.Data;
                    var rowSize = flatLhs.Length;
                    if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        for (int i = 0; i < rowSize; i++)
                        {
                            var retRow = retMatrix.GetRow(i);
                            var rhsRow = rhs.OdData.GetRow(i);
                            VectorHelper.Subtract(retRow, flatLhs[i], rhsRow);
                        }
                    }
                    else if (lhs.Direction == ComputationResult.VectorDirection.Horizontal)
                    {
                        for (int i = 0; i < rowSize; i++)
                        {
                            var retRow = retMatrix.GetRow(i);
                            var rhsRow = rhs.OdData.GetRow(i);
                            VectorHelper.Subtract(retRow, flatLhs, rhsRow);
                        }
                    }
                    else
                    {
                        return new ComputationResult("Unable to subtract vector without directionality starting at position " + (Lhs?.Start ?? -1) + "!");
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else
                {
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                    var flatRhs = rhs.VectorData.Data;
                    var rowSize = retMatrix.RowCategories.Count;
                    if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        for (int i = 0; i < flatRhs.Length; i++)
                        {
                            var retRow = retMatrix.GetRow(i);
                            var lhsRow = lhs.OdData.GetRow(i);
                            VectorHelper.Subtract(retRow, lhsRow, flatRhs[i]);
                        }
                    }
                    else if (rhs.Direction == ComputationResult.VectorDirection.Horizontal)
                    {
                        for (int i = 0; i < flatRhs.Length; i++)
                        {
                            var retRow = retMatrix.GetRow(i);
                            var lhsRow = lhs.OdData.GetRow(i);
                            VectorHelper.Subtract(retRow, lhsRow, flatRhs);
                        }
                    }
                    else
                    {
                        return new ComputationResult("Unable to subtract vector without directionality starting at position " + (Lhs?.Start ?? -1) + "!");
                    }
                    return new ComputationResult(retMatrix, true);
                }
            }
            else
            {
                var retMatrix = lhs.Accumulator ? lhs.OdData : (rhs.Accumulator ? rhs.OdData : new Matrix(lhs.OdData));
                VectorHelper.Subtract(retMatrix.Data, lhs.OdData.Data, rhs.OdData.Data);
                return new ComputationResult(retMatrix, true);
            }
        }
    }
}
