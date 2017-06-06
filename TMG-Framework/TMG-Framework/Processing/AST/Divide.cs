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

using XTMF2;
using TMG.Utilities;
using System;

namespace TMG.Frameworks.Data.Processing.AST
{
    public sealed class Divide : BinaryExpression
    {
        public Divide(int start) : base(start)
        {

        }

        internal override bool OptimizeAst(ref Expression ex, ref string error)
        {
            if (!base.OptimizeAst(ref ex, ref error))
            {
                return false;
            }
            var lhs = Lhs as Literal;
            var rhs = Rhs as Literal;
            if (lhs != null && rhs != null)
            {
                ex = new Literal(Start, lhs.Value / rhs.Value);
            }
            else if(rhs != null)
            {
                // if the RHS is a literal we can replace it with a multiply instead
                ex = new Multiply(Start)
                {
                    Lhs = Lhs,
                    Rhs = new Literal(rhs.Start, 1.0f / rhs.Value)
                };
            }
            return true;
        }

        public override ComputationResult Evaluate(ComputationResult lhs, ComputationResult rhs)
        {
            // see if we have two values, in this case we can skip doing the matrix operation
            if (lhs.IsValue && rhs.IsValue)
            {
                return new ComputationResult(lhs.LiteralValue / rhs.LiteralValue);
            }
            // float / matrix
            if (lhs.IsValue)
            {
                if (rhs.IsVectorResult)
                {
                    var retVector = rhs.Accumulator ? rhs.VectorData : new SparseVector(rhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.Divide(flat, lhs.LiteralValue, rhs.VectorData.Data);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData : new SparseMatrix(rhs.OdData);
                    VectorHelper.Divide(retMatrix.Data, lhs.LiteralValue, rhs.OdData.Data);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (rhs.IsValue)
            {
                if (lhs.IsVectorResult)
                {
                    var retVector = lhs.Accumulator ? lhs.VectorData : new SparseVector(lhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.Divide(flat, lhs.VectorData.Data, rhs.LiteralValue);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    // matrix / float
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData);
                    VectorHelper.Divide(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else
            {
                if (lhs.IsVectorResult || rhs.IsVectorResult)
                {
                    if (lhs.IsVectorResult && rhs.IsVectorResult)
                    {
                        var retMatrix = lhs.Accumulator ? lhs.VectorData : (rhs.Accumulator ? rhs.VectorData : new SparseVector(lhs.VectorData));
                        VectorHelper.Divide(retMatrix.Data, 0, lhs.VectorData.Data, 0, rhs.VectorData.Data, 0, retMatrix.Data.Length);
                        return new ComputationResult(retMatrix, true, lhs.Direction);
                    }
                    else if (lhs.IsVectorResult)
                    {
                        var retMatrix = rhs.Accumulator ? rhs.OdData : new SparseMatrix(rhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatRhs = rhs.OdData.Data;
                        var flatLhs = lhs.VectorData.Data;
                        var rowSize = flatLhs.Length;
                        if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                        { 
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Divide(retMatrix.Data, i * rowSize, flatLhs[i], flatRhs, i * rowSize, rowSize);
                            }
                        }
                        else if (lhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Divide(retMatrix.Data, i * rowSize, flatLhs, 0, flatRhs, i * rowSize, rowSize);
                            }
                        }
                        else
                        {
                            return new ComputationResult("Unable to add vector without directionality starting at position " + Lhs.Start + "!");
                        }
                        return new ComputationResult(retMatrix, true);
                    }
                    else
                    {
                        var retMatrix = lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatLhs = lhs.OdData.Data;
                        var flatRhs = rhs.VectorData.Data;
                        var rowSize = flatRhs.Length;
                        if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Divide(retMatrix.Data, i * rowSize, flatLhs, i * rowSize, flatRhs[i], rowSize);
                            }
                        }
                        else if (rhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Divide(retMatrix.Data, i * rowSize, flatLhs, i * rowSize, flatRhs, 0, rowSize);
                            }
                        }
                        else
                        {
                            return new ComputationResult("Unable to add vector without directionality starting at position " + Lhs.Start + "!");
                        }
                        return new ComputationResult(retMatrix, true);
                    }
                }
                else
                {
                    var retMatrix = lhs.Accumulator ? lhs.OdData : (rhs.Accumulator ? rhs.OdData : new SparseMatrix(lhs.OdData));
                    VectorHelper.Divide(retMatrix.Data, 0, lhs.OdData.Data, 0, rhs.OdData.Data, 0, lhs.OdData.Data.Length);
                    return new ComputationResult(retMatrix, true);
                }
            }
        }
    }
}
