﻿/*
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
    public sealed class CompareGreaterThanOrEqual : BinaryExpression
    {
        public CompareGreaterThanOrEqual(int start) : base(start)
        {

        }

        public override ComputationResult Evaluate(ComputationResult lhs, ComputationResult rhs)
        {
            // see if we have two values, in this case we can skip doing the matrix operation
            if (lhs.IsValue && rhs.IsValue)
            {
                return new ComputationResult(lhs.LiteralValue >= rhs.LiteralValue ? 1 : 0);
            }
            // float / matrix
            if (lhs.IsValue)
            {
                if (rhs.IsVectorResult)
                {
                    var retVector = rhs.Accumulator ? rhs.VectorData : new Vector(rhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.FlagIfGreaterThanOrEqual(flat, 0, lhs.LiteralValue, rhs.VectorData.Data, 0, flat.Length);
                    return new ComputationResult(retVector, true, rhs.Direction);
                }
                else
                {
                    var retMatrix = rhs.Accumulator ? rhs.OdData : new Matrix(rhs.OdData);
                    VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, 0, lhs.LiteralValue, rhs.OdData.Data, 0, rhs.OdData.Data.Length);
                    return new ComputationResult(retMatrix, true);
                }
            }
            else if (rhs.IsValue)
            {
                if (lhs.IsVectorResult)
                {
                    var retVector = lhs.Accumulator ? lhs.VectorData : new Vector(lhs.VectorData);
                    var flat = retVector.Data;
                    VectorHelper.FlagIfGreaterThanOrEqual(flat, 0, lhs.VectorData.Data, 0, rhs.LiteralValue, lhs.VectorData.Data.Length);
                    return new ComputationResult(retVector, true, lhs.Direction);
                }
                else
                {
                    // matrix / float
                    var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                    VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, 0, lhs.OdData.Data, 0, rhs.LiteralValue, lhs.OdData.Data.Length);
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
                        VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, 0, lhs.VectorData.Data, 0, rhs.VectorData.Data, 0, retMatrix.Data.Length);
                        return new ComputationResult(retMatrix, true, lhs.Direction);
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
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, i * rowSize, flatLhs[i], flatRhs, i * rowSize, rowSize);
                            }
                        }
                        else if (lhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, i * rowSize, flatLhs, 0, flatRhs, i * rowSize, rowSize);
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
                        var retMatrix = lhs.Accumulator ? lhs.OdData : new Matrix(lhs.OdData);
                        var flatRet = retMatrix.Data;
                        var flatLhs = lhs.OdData.Data;
                        var flatRhs = rhs.VectorData.Data;
                        var rowSize = flatRhs.Length;
                        if (rhs.Direction == ComputationResult.VectorDirection.Vertical)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, i * rowSize, flatLhs, i * rowSize, flatRhs[i], rowSize);
                            }
                        }
                        else if (rhs.Direction == ComputationResult.VectorDirection.Horizontal)
                        {
                            for (int i = 0; i < rowSize; i++)
                            {
                                VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, i * rowSize, flatLhs, i * rowSize, flatRhs, 0, rowSize);
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
                    VectorHelper.FlagIfGreaterThanOrEqual(retMatrix.Data, 0, lhs.OdData.Data, 0, rhs.OdData.Data, 0, retMatrix.Data.Length);
                    return new ComputationResult(retMatrix, true);
                }
            }
        }
    }
}
