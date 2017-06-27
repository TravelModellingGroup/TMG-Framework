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
using System.Threading.Tasks;

namespace TMG.Frameworks.Data.Processing.AST
{
    public sealed class FusedMultiplyAdd : Expression
    {

        public Expression MulLhs;
        public Expression MulRhs;
        public Expression Add;
        private int AddStart;

        public FusedMultiplyAdd(int mulStart, int addStart) : base(mulStart)
        {
            AddStart = addStart;
        }

        public override ComputationResult Evaluate(IModule[] dataSources)
        {
            ComputationResult mulLhs = null;
            ComputationResult mulRhs = null;
            ComputationResult add = null;
            Parallel.Invoke(
                () => mulLhs = MulLhs.Evaluate(dataSources),
                () => mulRhs = MulRhs.Evaluate(dataSources),
                () => add = Add.Evaluate(dataSources));

            // mulLhs = MulLhs.Evaluate(dataSources);
            // mulRhs = MulRhs.Evaluate(dataSources);
            // add = Add.Evaluate(dataSources);

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
            if(!ValidateSizes(mulLhs, mulRhs, Start, out var error))
            {
                return error;
            }
            if (!ValidateSizes(mulRhs, add, AddStart, out var error2))
            {
                return error2;
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
                    var retVector = rhs.Accumulator ? rhs.VectorData : new Vector(rhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.FusedMultiplyAdd(flat, 0, rhs.VectorData.Data, 0, lhs.LiteralValue, add.LiteralValue, flat.Length);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData);
                    // inverted lhs, rhs since order does not matter
                    var flatRet = retMatrix.Data;
                    var flatLhs = lhs.LiteralValue;
                    var flatRhs = rhs.OdData.Data;
                    var flatAdd = add.LiteralValue;
                    var rowSize = retMatrix.RowCategories.Count;
                    Parallel.For(0, rowSize, (int i) =>
                    {
                        VectorHelper.FusedMultiplyAdd(flatRet, rowSize * i, flatRhs, rowSize * i,
                            flatLhs, flatAdd, rowSize);
                    });
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (rhs.IsValue)
            {
                if (lhs.IsVectorResult)
                {
                    var retVector = lhs.Accumulator ? lhs.VectorData : new Vector(lhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.FusedMultiplyAdd(flat, 0, lhs.VectorData.Data, 0, rhs.LiteralValue, add.LiteralValue, flat.Length);
                    return new ComputationResult(retVector, true);
                }
                else
                {
                    // matrix / float
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                    VectorHelper.FusedMultiplyAdd(retMatrix.Data, 0, lhs.OdData.Data, 0, rhs.LiteralValue, add.LiteralValue, retMatrix.Data.Length);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else
            {
                if (lhs.IsVectorResult || rhs.IsVectorResult)
                {
                    if (lhs.IsVectorResult && rhs.IsVectorResult)
                    {
                        var retVector = lhs.Accumulator ? lhs.VectorData : (rhs.Accumulator ? rhs.VectorData : new Vector(lhs.VectorData));
                        VectorHelper.FusedMultiplyAdd(retVector.Data, 0, lhs.VectorData.Data, 0, rhs.VectorData.Data, 0, add.LiteralValue, retVector.Data.Length);
                        return new ComputationResult(retVector, true, lhs.Direction);
                    }
                    else if (lhs.IsVectorResult)
                    {
                        var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatRhs = rhs.OdData.Data;
                        var flatLhs = lhs.VectorData.Data;
                        var rowSize = flatLhs.Length;
                        if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(flatRet, i * rowSize, flatRhs, i * rowSize, flatLhs[i], add.LiteralValue, rowSize);
                            });
                        }
                        else if (lhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(flatRet, i * rowSize, flatRhs, i * rowSize, flatLhs, 0, add.LiteralValue, rowSize);
                            });
                        }
                        else
                        {
                            return new ComputationResult("Unable to add vector without directionality starting at position " + MulLhs.Start + "!");
                        }
                        return new ComputationResult(retMatrix, true);
                    }
                    else
                    {
                        var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatLhs = lhs.OdData.Data;
                        var flatRhs = rhs.VectorData.Data;
                        var rowSize = flatRhs.Length;
                        if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(flatRet, i * rowSize, flatLhs, i * rowSize, flatRhs[i], add.LiteralValue, rowSize);
                            });
                        }
                        else if (rhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(flatRet, i * rowSize, flatLhs, i * rowSize, flatRhs, 0, add.LiteralValue, rowSize);
                            });
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
                    var retMatrix = lhs.Accumulator ? lhs.OdData : (rhs.Accumulator ? rhs.OdData : new Matrix(lhs.OdData));
                    var flatRet = retMatrix.Data;
                    var flatLhs = lhs.OdData.Data;
                    var flatRhs = rhs.OdData.Data;
                    var flatAdd = add.LiteralValue;
                    var rowSize = retMatrix.RowCategories.Count;
                    Parallel.For(0, rowSize, (int i) =>
                    {
                        VectorHelper.FusedMultiplyAdd(flatRet, rowSize * i, flatLhs, rowSize * i,
                            flatRhs, rowSize * i, flatAdd, rowSize);
                    });
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
                var retVector = add.Accumulator ? add.VectorData : new Vector(add.VectorData);
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
                        (lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData));
                    var rowSize = add.VectorData.Data.Length;
                    if (add.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        Parallel.For(0, rowSize, (int i) =>
                        {
                            VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.OdData.Data, i * rowSize, add.VectorData[i], rowSize);
                        });
                    }
                    else
                    {
                        Parallel.For(0, rowSize, (int i) =>
                        {
                            VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.OdData.Data, i * rowSize, add.VectorData.Data, 0, rowSize);
                        });
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else if (rhs.IsVectorResult)
                {
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                    var rowSize = add.VectorData.Data.Length;
                    if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        if (add.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.VectorData.Data[i], add.VectorData.Data[i], rowSize);
                            });
                        }
                        else
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.VectorData.Data[i], add.VectorData.Data, 0, rowSize);
                            });
                        }
                    }
                    else
                    {
                        if (add.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.VectorData.Data, 0, add.VectorData.Data[i], rowSize);
                            });
                        }
                        else
                        {
                            Parallel.For(0, rowSize, (int i) =>
                            {
                                VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.VectorData.Data, 0, add.VectorData.Data, 0, rowSize);
                            });
                        }
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else
                {
                    //RHS is a scalar
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                    var rowSize = add.VectorData.Data.Length;
                    if (add.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        Parallel.For(0, rowSize, (int i) =>
                        {
                            VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.LiteralValue, add.VectorData.Data[i], rowSize);
                        });
                    }
                    else
                    {
                        Parallel.For(0, rowSize, (int i) =>
                        {
                            VectorHelper.FusedMultiplyAdd(retMatrix.Data, i * rowSize, lhs.OdData.Data, i * rowSize, rhs.LiteralValue, add.VectorData.Data, 0, rowSize);
                        });
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
                        (lhs.Accumulator ? lhs.VectorData : new Vector(lhs.VectorData)));
                    VectorHelper.FusedMultiplyAdd(retVector.Data, 0, lhs.VectorData.Data, 0, rhs.VectorData.Data, 0, add.VectorData.Data, 0, retVector.Data.Length);
                    return new ComputationResult(retVector, true, add.Direction == lhs.Direction && add.Direction == rhs.Direction ? add.Direction : ComputationResult.VectorDirection.Unassigned);
                }
                // vector * lit + vector
                else
                {
                    var retVector = add.Accumulator ? add.VectorData :
                        (lhs.Accumulator ? lhs.VectorData : new Vector(lhs.VectorData));
                    VectorHelper.FusedMultiplyAdd(retVector.Data, 0, lhs.VectorData.Data, 0, rhs.LiteralValue,
                        add.VectorData.Data, 0, add.VectorData.Data.Length);
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
                        (rhs.Accumulator ? rhs.OdData : new Matrix(add.OdData)));
                    var flatRet = retMatrix.Data;
                    var flatLhs = lhs.OdData.Data;
                    var flatRhs = rhs.OdData.Data;
                    var flatAdd = add.OdData.Data;
                    var rowSize = retMatrix.RowCategories.Count;
                    Parallel.For(0, rowSize, (int i) =>
                    {
                        VectorHelper.FusedMultiplyAdd(flatRet, flatLhs, flatRhs,
                                                flatAdd, rowSize * i, rowSize);
                    });
                    return new ComputationResult(retMatrix, true);
                }
                else if (rhs.IsVectorResult)
                {
                    var retMatrix = add.Accumulator ? add.OdData :
                        (lhs.Accumulator ? lhs.OdData : new Matrix(add.OdData));
                    var flatRet = retMatrix.Data;
                    var flatLhs = lhs.OdData.Data;
                    var flatRhs = rhs.VectorData.Data;
                    var flatAdd = add.OdData.Data;
                    var rowSize = retMatrix.RowCategories.Count;
                    if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                    {
                        Parallel.For(0, rowSize, (int i) =>
                        {
                            VectorHelper.FusedMultiplyAdd(flatRet, i * rowSize, flatLhs, i * rowSize, flatRhs[i], flatAdd, i * rowSize, rowSize);
                        });
                    }
                    else
                    {
                        Parallel.For(0, rowSize, (int i) =>
                        {
                            VectorHelper.FusedMultiplyAdd(flatRet, i * rowSize, flatLhs, i * rowSize, flatRhs, 0, flatAdd, i * rowSize, rowSize);
                        });
                    }
                    return new ComputationResult(retMatrix, true);
                }
                else
                {
                    //RHS is scalar
                    var retMatrix = add.Accumulator ? add.OdData :
                        (lhs.Accumulator ? lhs.OdData : new Matrix(add.OdData));
                    var rowSize = retMatrix.RowCategories.Count;
                    Parallel.For(0, rowSize, (int i) =>
                    {
                        VectorHelper.FusedMultiplyAdd(retMatrix.Data, rowSize * i, lhs.OdData.Data, rowSize * i, rhs.LiteralValue,
                            add.OdData.Data, rowSize * i, rowSize);
                    });
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (lhs.IsVectorResult)
            {
                var retMatrix = add.Accumulator ? add.OdData : new Matrix(add.OdData);
                var tempVector = lhs.Accumulator ? lhs.VectorData : (rhs.IsVectorResult && rhs.Accumulator ? rhs.VectorData : new Vector(lhs.VectorData));
                var flatRet = retMatrix.Data;
                var flatAdd = add.OdData.Data;
                var rowSize = tempVector.Data.Length;
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
                var flatTemp = tempVector.Data;
                if (lhs.Direction == ComputationResult.VectorDirection.Vertical)
                {
                    Parallel.For(0, rowSize, (int i) =>
                    {
                        VectorHelper.Add(flatRet, i * rowSize, flatAdd, i * rowSize, flatTemp[i], rowSize);
                    });
                }
                else
                {
                    Parallel.For(0, rowSize, (int i) =>
                    {
                        VectorHelper.Add(flatRet, i * rowSize, flatAdd, i * rowSize, flatTemp, 0, rowSize);
                    });
                }
                return new ComputationResult(retMatrix, true);
            }
            else
            {
                // in this case LHS is a scalar, and therefore RHS is also a scalar
                var retMatrix = add.Accumulator ? add.OdData : new Matrix(add.OdData);
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
