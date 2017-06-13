/*
    Copyright 2016-2017 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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

using System;
using TMG.Utilities;

namespace TMG.Frameworks.Data.Processing.AST
{
    public class Add : BinaryExpression
    {
        public Add(int start) : base(start)
        {

        }

        internal override bool OptimizeAst(ref Expression ex, ref string error)
        {
            if(!base.OptimizeAst(ref ex, ref error))
            {
                return false;
            }
            if(!OptimizeFusedMultiplyAdd(ref ex)
                || !OptimizeLiterals(ref ex))
            {
                return false;
            }
            return true;
        }

        private bool OptimizeLiterals(ref Expression ex)
        {
            var lhs = Lhs as Literal;
            var rhs = Rhs as Literal;
            if(lhs != null && rhs != null)
            {
                ex = new Literal(Start, lhs.Value + rhs.Value);
            }
            return true;
        }

        private bool OptimizeFusedMultiplyAdd(ref Expression ex)
        {
            var lhsMul = Lhs as Multiply;
            var rhsMul = Rhs as Multiply;
            if (lhsMul != null)
            {
                ex = new FusedMultiplyAdd(Start)
                {
                    MulLhs = lhsMul.Lhs,
                    MulRhs = lhsMul.Rhs,
                    Add = Rhs
                };
            }
            else if (rhsMul != null)
            {
                ex = new FusedMultiplyAdd(Start)
                {
                    MulLhs = rhsMul.Lhs,
                    MulRhs = rhsMul.Rhs,
                    Add = Lhs
                };
            }
            return true;
        }

        public override ComputationResult Evaluate(ComputationResult lhs, ComputationResult rhs)
        {
            // see if we have two values, in this case we can skip doing the matrix operation
            if (lhs.IsValue && rhs.IsValue)
            {
                return new ComputationResult(lhs.LiteralValue + rhs.LiteralValue);
            }
            // float / matrix
            if (lhs.IsValue)
            {
                if (rhs.IsVectorResult)
                {
                    var retVector = rhs.Accumulator ? rhs.VectorData : new Vector(rhs.VectorData.Map);
                    var flat = retVector.Data;
                    VectorHelper.Add(flat, rhs.VectorData.Data, lhs.LiteralValue);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData.Map);
                    VectorHelper.Add(retMatrix.Data, rhs.OdData.Data, lhs.LiteralValue);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (rhs.IsValue)
            {
                if (lhs.IsVectorResult)
                {
                    var retVector = lhs.Accumulator ? lhs.VectorData : new Vector(lhs.VectorData.Map);
                    var flat = retVector.Data;
                    VectorHelper.Add(flat, lhs.VectorData.Data, rhs.LiteralValue);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    // matrix / float
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData.Map);
                    VectorHelper.Add(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue);
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
                        VectorHelper.Add(retMatrix.Data, 0, lhs.VectorData.Data, 0, rhs.VectorData.Data, 0, retMatrix.Data.Length);
                        return new ComputationResult(retMatrix, true, lhs.Direction);
                    }
                    else if (lhs.IsVectorResult)
                    {
                        var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData.Map);
                        var flatRet = retMatrix.Data;
                        var flatRhs = rhs.OdData.Data;
                        var flatLhs = lhs.VectorData.Data;
                        var rowSize = flatLhs.Length;
                        if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Add(retMatrix.Data, i * rowSize, flatRhs, i * rowSize, flatLhs[i], rowSize);
                            }
                        }
                        else if (lhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Add(retMatrix.Data, i * rowSize, flatLhs, 0, flatRhs, i * rowSize, rowSize);
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
                        var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData.Map);
                        var flatRet = retMatrix.Data;
                        var flatLhs = lhs.OdData.Data;
                        var flatRhs = rhs.VectorData.Data;
                        var rowSize = flatRhs.Length;
                        if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Add(retMatrix.Data, i * rowSize, flatLhs, i * rowSize, flatRhs[i], rowSize);
                            }
                        }
                        else if (rhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.Add(retMatrix.Data, i * rowSize, flatRhs, 0, flatLhs, i * rowSize, rowSize);
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
                    var retMatrix = lhs.Accumulator ? lhs.OdData : (rhs.Accumulator ? rhs.OdData : new Matrix(lhs.OdData));
                    VectorHelper.Add(retMatrix.Data, lhs.OdData.Data, rhs.OdData.Data);
                    return new ComputationResult(retMatrix, true);
                }
            }
        }
    }
}
