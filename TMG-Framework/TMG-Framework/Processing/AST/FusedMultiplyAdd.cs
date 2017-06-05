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
    public sealed class FusedMultiplyAdd : Expression
    {

        public Expression MulLhs;
        public Expression MulRhs;
        public Expression Add;

        public FusedMultiplyAdd(int start) : base(start)
        {

        }

        public override ComputationResult Evaluate(IModule[] dataSources)
        {
            var mulLhs = MulLhs.Evaluate(dataSources);
            var mulRhs = MulRhs.Evaluate(dataSources);
            var add = Add.Evaluate(dataSources);
            if (mulLhs.Error)
            {
                return mulLhs;
            }
            else if (mulRhs.Error)
            {
                return mulRhs;
            }
            else if (add.Error)
            {
                return add;
            }
            return Evaluate(mulLhs, mulRhs, add);
        }

        private ComputationResult Evaluate(ComputationResult mulLhs, ComputationResult mulRhs, ComputationResult add)
        {
            if (add.IsValue)
            {
                return EvaluateAddIsValue(mulLhs, mulRhs, add);
            }
            else if (add.IsVectorResult)
            {
                return EvaluateAddIsVector(mulLhs, mulRhs, add);
            }
            else
            {
                return EvaluateAddIsMatrix(mulLhs, mulRhs, add);
            }
        }

        private ComputationResult EvaluateAddIsValue(ComputationResult lhs, ComputationResult rhs, ComputationResult add)
        {
            if (add.IsValue && lhs.IsValue && rhs.IsValue)
            {
                return new ComputationResult(lhs.LiteralValue * rhs.LiteralValue + add.LiteralValue);
            }
            // float / matrix
            if (lhs.IsValue)
            {
                if (rhs.IsVectorResult)
                {
                    var retVector = rhs.Accumulator ? rhs.VectorData : new SparseVector(rhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.FusedMultiplyAdd(flat, rhs.VectorData.Data, lhs.LiteralValue, add.LiteralValue);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData : new SparseMatrix(rhs.OdData);
                    // inverted lhs, rhs since order does not matter
                    VectorHelper.FusedMultiplyAdd(retMatrix.Data, rhs.OdData.Data, lhs.LiteralValue, add.LiteralValue);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (rhs.IsValue)
            {
                if (lhs.IsVectorResult)
                {
                    var retVector = lhs.Accumulator ? lhs.VectorData : new SparseVector(lhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.FusedMultiplyAdd(flat, lhs.VectorData.Data, rhs.LiteralValue, add.LiteralValue);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    // matrix / float
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData);
                    VectorHelper.FusedMultiplyAdd(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue, add.LiteralValue);
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
                        VectorHelper.FusedMultiplyAdd(retMatrix.Data, lhs.VectorData.Data, rhs.VectorData.Data, add.LiteralValue);
                        return new ComputationResult(retMatrix, true, lhs.Direction);
                    }
                    else if (lhs.IsVectorResult)
                    {
                        var retMatrix = rhs.Accumulator ? rhs.OdData : new SparseMatrix(rhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatRhs = rhs.OdData.Data;
                        var flatLhs = lhs.VectorData.Data;
                        if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            throw new NotImplementedException();
                        }
                        else if (lhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            return new ComputationResult("Unable to add vector without directionality starting at position " + MulLhs.Start + "!");
                        }
                        return new ComputationResult(retMatrix, true);
                    }
                    else
                    {
                        var retMatrix = lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatLhs = lhs.OdData.Data;
                        var flatRhs = rhs.VectorData.Data;
                        if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            throw new NotImplementedException();
                        }
                        else if (rhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            return new ComputationResult("Unable to add vector without directionality starting at position " + MulLhs.Start + "!");
                        }
                        return new ComputationResult(retMatrix, true);
                    }
                }
                else
                {
                    var retMatrix = lhs.Accumulator ? lhs.OdData : (rhs.Accumulator ? rhs.OdData : new SparseMatrix(lhs.OdData));
                    VectorHelper.FusedMultiplyAdd(retMatrix.Data, lhs.OdData.Data, rhs.OdData.Data, add.LiteralValue);
                    return new ComputationResult(retMatrix, true);
                }
            }
        }

        private static void Swap<T>(ref T first, ref T second) where T : class
        {
            var temp = first;
            first = second;
            second = temp;
        }

        private ComputationResult EvaluateAddIsVector(ComputationResult lhs, ComputationResult rhs, ComputationResult add)
        {
            // Test the simple case of this really just being an add with a constant multiply
            if (lhs.IsValue && rhs.IsValue)
            {
                var retVector = add.Accumulator ? add.VectorData : new SparseVector(add.VectorData);
                VectorHelper.Add(retVector.Data, add.VectorData.Data, lhs.LiteralValue * rhs.LiteralValue);
                return new ComputationResult(retVector, true, add.Direction);
            }
            if (lhs.IsOdResult || rhs.IsOdResult)
            {
                if (lhs.IsVectorResult && lhs.Direction == ComputationResult.VectorDirection.Unassigned)
                {
                    return new ComputationResult("Unable to multiply vector without directionality starting at position " + MulLhs.Start + "!");
                }
                if (rhs.IsVectorResult && lhs.Direction == ComputationResult.VectorDirection.Unassigned)
                {
                    return new ComputationResult("Unable to multiply vector without directionality starting at position " + MulRhs.Start + "!");
                }
                if (add.Direction == ComputationResult.VectorDirection.Unassigned)
                {
                    return new ComputationResult("Unable to add vector without directionality starting at position " + Add.Start + "!");
                }
                // if the lhs is a value just swap the two around
                if (!lhs.IsOdResult)
                {
                    Swap(ref lhs, ref rhs);
                }
                //LHS is a matrix
                if (rhs.IsOdResult)
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData :
                        (lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData));
                    if (add.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        //VectorHelper.FusedMultiplyAddVerticalAdd(retMatrix.Data, lhs.OdData.Data, rhs.OdData.Data, add.VectorData.Data);
                        throw new NotImplementedException();
                    }
                    else
                    {
                        //VectorHelper.FusedMultiplyAddHorizontalAdd(retMatrix.Data, lhs.OdData.Data, rhs.OdData.Data, add.VectorData.Data);
                        throw new NotImplementedException();
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else if (rhs.IsVectorResult)
                {
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData);
                    if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        if (add.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            //VectorHelper.FusedMultiplyAddVerticalRhsVerticalAdd(retMatrix.Data, lhs.OdData.Data, rhs.VectorData.Data, add.VectorData.Data);
                            throw new NotImplementedException();
                        }
                        else
                        {
                            //VectorHelper.FusedMultiplyAddVerticalRhsHorizontalAdd(retMatrix.Data, lhs.OdData.Data, rhs.VectorData.Data, add.VectorData.Data);
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        if (add.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            //VectorHelper.FusedMultiplyAddHorizontalRhsVerticalAdd(retMatrix.Data, lhs.OdData.Data, rhs.VectorData.Data, add.VectorData.Data);
                            throw new NotImplementedException();
                        }
                        else
                        {
                            //VectorHelper.FusedMultiplyAddHorizontalRhsHorizontalAdd(retMatrix.Data, lhs.OdData.Data, rhs.VectorData.Data, add.VectorData.Data);
                            throw new NotImplementedException();
                        }
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else
                {
                    //RHS is a scalar
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new SparseMatrix(lhs.OdData);
                    if (add.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        //VectorHelper.FusedMultiplyAddVerticalAdd(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue, add.VectorData.Data);
                        throw new NotImplementedException();
                    }
                    else
                    {
                        //VectorHelper.FusedMultiplyAddHorizontalAdd(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue, add.VectorData.Data);
                        throw new NotImplementedException();
                    }
                    return new ComputationResult(retMatrix, true);
                }
            }
            // vector cases
            else
            {
                // if the lhs is a value just swap the two around
                if (lhs.IsValue)
                {
                    Swap(ref lhs, ref rhs);
                }
                // vector * vector + vector
                if (rhs.IsVectorResult)
                {
                    var retVector = add.Accumulator ? add.VectorData :
                        (rhs.Accumulator ? rhs.VectorData :
                        (lhs.Accumulator ? lhs.VectorData : new SparseVector(lhs.VectorData)));
                    VectorHelper.FusedMultiplyAdd(retVector.Data, lhs.VectorData.Data, rhs.VectorData.Data, add.VectorData.Data);
                    return new ComputationResult(retVector, true, add.Direction == lhs.Direction && add.Direction == rhs.Direction ? add.Direction : ComputationResult.VectorDirection.Unassigned);
                }
                // vector * lit + vector
                else
                {
                    var retVector = add.Accumulator ? add.VectorData :
                        (lhs.Accumulator ? lhs.VectorData : new SparseVector(lhs.VectorData));
                    VectorHelper.FusedMultiplyAdd(retVector.Data, lhs.VectorData.Data, rhs.LiteralValue, add.VectorData.Data);
                    return new ComputationResult(retVector, true, add.Direction == lhs.Direction && add.Direction == rhs.Direction ? add.Direction : ComputationResult.VectorDirection.Unassigned);
                }
            }
        }

        private ComputationResult EvaluateAddIsMatrix(ComputationResult lhs, ComputationResult rhs, ComputationResult add)
        {
            if (lhs.IsVectorResult && lhs.Direction == ComputationResult.VectorDirection.Unassigned)
            {
                return new ComputationResult("Unable to multiply vector without directionality starting at position " + MulLhs.Start + "!");
            }
            if (rhs.IsVectorResult && rhs.Direction == ComputationResult.VectorDirection.Unassigned)
            {
                return new ComputationResult("Unable to multiply vector without directionality starting at position " + MulRhs.Start + "!");
            }
            // Ensure that the LHS is a higher or equal order to the RHS (Matrix > Vector > Scalar)
            if (!lhs.IsOdResult)
            {
                Swap(ref lhs, ref rhs);
            }
            if (lhs.IsValue)
            {
                Swap(ref lhs, ref rhs);
            }
            // LHS is now a higher or equal to the order of RHS
            if (lhs.IsOdResult)
            {
                if (rhs.IsOdResult)
                {
                    var retMatrix = add.Accumulator ? add.OdData :
                        (lhs.Accumulator ? lhs.OdData :
                        (rhs.Accumulator ? rhs.OdData : new SparseMatrix(add.OdData)));
                    VectorHelper.FusedMultiplyAdd(retMatrix.Data, lhs.OdData.Data, rhs.OdData.Data, add.OdData.Data);
                    return new ComputationResult(retMatrix, true);
                }
                else if (rhs.IsVectorResult)
                {
                    var retMatrix = add.Accumulator ? add.OdData :
                        (lhs.Accumulator ? lhs.OdData : new SparseMatrix(add.OdData));
                    if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        //VectorHelper.FusedMultiplyAddVerticalRhs(retMatrix.Data, lhs.OdData.Data, rhs.VectorData.Data, add.OdData.Data);
                        throw new NotImplementedException();
                    }
                    else
                    {
                        //VectorHelper.FusedMultiplyAddHorizontalRhs(retMatrix.Data, lhs.OdData.Data, rhs.VectorData.Data, add.OdData.Data);
                        throw new NotImplementedException();
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else
                {
                    //RHS is scalar
                    var retMatrix = add.Accumulator ? add.OdData :
                        (lhs.Accumulator ? lhs.OdData : new SparseMatrix(add.OdData));
                    VectorHelper.FusedMultiplyAdd(retMatrix.Data, lhs.OdData.Data, rhs.LiteralValue, add.OdData.Data);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (lhs.IsVectorResult)
            {
                var retMatrix = add.Accumulator ? add.OdData : new SparseMatrix(add.OdData);
                var tempVector = lhs.Accumulator ? lhs.VectorData : (rhs.IsVectorResult && rhs.Accumulator ? rhs.VectorData : new SparseVector(lhs.VectorData));
                // compute the multiplication separately in this case for better performance (n multiplies instead of n^2)
                if (rhs.IsVectorResult)
                {
                    if (lhs.Direction != rhs.Direction)
                    {
                        // if the directions don't add up then the sum operation would be undefined!
                        return new ComputationResult("Unable to add vector without directionality starting at position " + MulLhs.Start + "!");
                    }
                    VectorHelper.Multiply(tempVector.Data, 0, lhs.VectorData.Data, 0, rhs.VectorData.Data, 0, tempVector.Data.Length);
                }
                else
                {
                    VectorHelper.Multiply(tempVector.Data, lhs.VectorData.Data, rhs.LiteralValue);
                }
                if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                {
                    //VectorHelper.AddVertical(retMatrix.Data, add.OdData.Data, tempVector.Data);
                    throw new NotImplementedException();
                }
                else
                {
                    //VectorHelper.AddHorizontal(retMatrix.Data, add.OdData.Data, tempVector.Data);
                    throw new NotImplementedException();
                }
                return new ComputationResult(retMatrix, true);
            }
            else
            {
                // in this case LHS is a scalar, and therefore RHS is also a scalar
                var retMatrix = add.Accumulator ? add.OdData : new SparseMatrix(add.OdData);
                VectorHelper.Add(retMatrix.Data, add.OdData.Data, lhs.LiteralValue * rhs.LiteralValue);
                return new ComputationResult(retMatrix, true);
            }
        }

        internal override bool OptimizeAst(ref Expression ex, ref string error)
        {
            return !(!MulLhs.OptimizeAst(ref MulLhs, ref error)
                    || !MulLhs.OptimizeAst(ref MulRhs, ref error)
                    || !MulLhs.OptimizeAst(ref Add, ref error));
        }
    }
}
