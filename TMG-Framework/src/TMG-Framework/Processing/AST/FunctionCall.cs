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
using System;
using System.Linq;
using TMG.Utilities;
using XTMF2;

namespace TMG.Frameworks.Data.Processing.AST
{
    public sealed class FunctionCall : Value
    {

        public enum FunctionType
        {
            Undefined,
            Transpose,
            SumRows,
            SumColumns,
            AsHorizontal,
            AsVertical,
            Sum,
            Abs,
            Avg,
            AvgRows,
            AvgColumns,
            E,
            Pi,
            Length,
            LengthColumns,
            LengthRows,
            ZeroMatrix,
            Matrix,
            IdentityMatrix,
            Log,
            Sqrt,
            If,
            IfNaN,
            Normalize,
            NormalizeColumns,
            NormalizeRows
        }

        private readonly FunctionType _type;

        private readonly Expression[] _parameters;

        private FunctionCall(int start, FunctionType call, Expression[] parameters) : base(start)
        {
            _parameters = parameters;
            _type = call;
        }

        internal override bool OptimizeAst(ref Expression ex, ref string error)
        {
            for (int i = 0; i < _parameters.Length; i++)
            {
                if (!_parameters[i].OptimizeAst(ref _parameters[i], ref error))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool GetCall(int start, string call, Expression[] parameters, out FunctionCall ex, ref string error)
        {
            //decode the call to a type
            ex = null;
            if (!Decode(call, ref error, out FunctionType type))
            {
                return false;
            }
            ex = new FunctionCall(start, type, parameters);
            return true;
        }

        private static bool Decode(string call, ref string error, out FunctionType type)
        {
            type = FunctionType.Undefined;
            call = call.ToLowerInvariant();
            switch (call)
            {
                case "ashorizontal":
                    type = FunctionType.AsHorizontal;
                    return true;
                case "asvertical":
                    type = FunctionType.AsVertical;
                    return true;
                case "transpose":
                    type = FunctionType.Transpose;
                    return true;
                case "sumrows":
                    type = FunctionType.SumRows;
                    return true;
                case "sumcolumns":
                    type = FunctionType.SumColumns;
                    return true;
                case "sum":
                    type = FunctionType.Sum;
                    return true;
                case "abs":
                    type = FunctionType.Abs;
                    return true;
                case "avg":
                    type = FunctionType.Avg;
                    return true;
                case "avgrows":
                    type = FunctionType.AvgRows;
                    return true;
                case "avgcolumns":
                    type = FunctionType.AvgColumns;
                    return true;
                case "e":
                    type = FunctionType.E;
                    return true;
                case "pi":
                    type = FunctionType.Pi;
                    return true;
                case "length":
                    type = FunctionType.Length;
                    return true;
                case "lengthrows":
                    type = FunctionType.LengthRows;
                    return true;
                case "lengthcolumns":
                    type = FunctionType.LengthColumns;
                    return true;
                case "zeromatrix":
                    type = FunctionType.ZeroMatrix;
                    return true;
                case "matrix":
                    type = FunctionType.Matrix;
                    return true;
                case "identitymatrix":
                    type = FunctionType.IdentityMatrix;
                    return true;
                case "log":
                    type = FunctionType.Log;
                    return true;
                case "sqrt":
                    type = FunctionType.Sqrt;
                    return true;
                case "if":
                    type = FunctionType.If;
                    return true;
                case "ifnan":
                    type = FunctionType.IfNaN;
                    return true;
                case "normalize":
                    type = FunctionType.Normalize;
                    return true;
                case "normalizecolumns":
                    type = FunctionType.NormalizeColumns;
                    return true;
                case "normalizerows":
                    type = FunctionType.NormalizeRows;
                    return true;
                default:
                    error = "The function '" + call + "' is undefined!";
                    return false;
            }
        }

        public override ComputationResult Evaluate(IModule[] dataSources)
        {
            // first evaluate the parameters
            var values = new ComputationResult[_parameters.Length];
            switch (_parameters.Length)
            {
                case 0:
                    break;
                case 1:
                    {
                        values[0] = _parameters[0].Evaluate(dataSources);
                        if (values[0].Error)
                        {
                            return values[0];
                        }
                    }
                    break;
                default:
                    {
                        System.Threading.Tasks.Parallel.For(0, values.Length, (int i) =>
                        {
                            values[i] = _parameters[i].Evaluate(dataSources);
                        });
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i].Error)
                            {
                                return values[i];
                            }
                        }
                    }
                    break;
            }

            switch (_type)
            {
                case FunctionType.AsHorizontal:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("AsHorizontal at position " + Start + " was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsVectorResult)
                    {
                        return new ComputationResult("AsHorizontal at position " + Start + " was executed with a parameter that was not a vector!");
                    }
                    return new ComputationResult(values[0], ComputationResult.VectorDirection.Horizontal);
                case FunctionType.AsVertical:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("AsVertical at position " + Start + " was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsVectorResult)
                    {
                        return new ComputationResult("AsVertical at position " + Start + " was executed with a parameter that was not a vector!");
                    }
                    return new ComputationResult(values[0], ComputationResult.VectorDirection.Vertical);
                case FunctionType.Transpose:
                    {
                        if (values.Length != 1)
                        {
                            return new ComputationResult("Transpose at position " + Start + " was executed with the wrong number of parameters!");
                        }
                        if (values[0].IsVectorResult)
                        {
                            switch (values[0].Direction)
                            {
                                case ComputationResult.VectorDirection.Horizontal:
                                    return new ComputationResult(values[0], ComputationResult.VectorDirection.Vertical);
                                case ComputationResult.VectorDirection.Vertical:
                                    return new ComputationResult(values[0], ComputationResult.VectorDirection.Horizontal);
                                case ComputationResult.VectorDirection.Unassigned:
                                    return new ComputationResult("Unable to transpose an vector that does not have a directionality!");
                            }
                        }
                        if (values[0].IsValue)
                        {
                            return new ComputationResult("The parameter to Transpose at position " + Start + " was executed against a scalar!");
                        }
                        if (values[0].IsOdResult)
                        {
                            return TransposeOd(values[0]);
                        }
                        return new ComputationResult("Unsupported data type for Transpose at position " + Start + ".");
                    }
                case FunctionType.SumRows:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("SumRows was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsOdResult)
                    {
                        return new ComputationResult("SumRows was executed with a parameter that was not a matrix!");
                    }
                    return SumRows(values[0]);
                case FunctionType.SumColumns:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("SumColumns was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsOdResult)
                    {
                        return new ComputationResult("SumColumns was executed with a parameter that was not a matrix!");
                    }
                    return SumColumns(values[0]);
                case FunctionType.AvgRows:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("AvgRows was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsOdResult)
                    {
                        return new ComputationResult("AvgRows was executed with a parameter that was not a matrix!");
                    }
                    return AvgRows(values[0]);
                case FunctionType.AvgColumns:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("AvgColumns was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsOdResult)
                    {
                        return new ComputationResult("AvgColumns was executed with a parameter that was not a matrix!");
                    }
                    return AvgColumns(values[0]);
                case FunctionType.Sum:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Sum was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsValue)
                    {
                        return new ComputationResult("Sum was executed with a parameter that was already a scalar value!");
                    }
                    return Sum(values[0]);
                case FunctionType.Abs:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Abs was executed with the wrong number of parameters!");
                    }
                    return Abs(values[0]);
                case FunctionType.Avg:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Avg was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsValue)
                    {
                        return new ComputationResult("Avg was executed with a parameter that was already a scalar value!");
                    }
                    return Avg(values[0]);
                case FunctionType.E:
                    return new ComputationResult((float)Math.E);
                case FunctionType.Pi:
                    return new ComputationResult((float)Math.PI);
                case FunctionType.Length:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Length was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsValue)
                    {
                        return new ComputationResult("Length can not be applied to a scalar!");
                    }
                    return Length(values[0]);
                case FunctionType.LengthColumns:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("LengthColumns was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsOdResult)
                    {
                        return new ComputationResult("LengthColumns must be applied to a Matrix!");
                    }
                    return LengthColumns(values[0]);
                case FunctionType.LengthRows:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("LengthRows was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsOdResult)
                    {
                        return new ComputationResult("LengthRows must be applied to a Matrix!");
                    }
                    return LengthRows(values[0]);
                case FunctionType.ZeroMatrix:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("ZeroMatrix was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsValue)
                    {
                        return new ComputationResult("ZeroMatrix must be applied to a vector, or a matrix!");
                    }
                    return ZeroMatrix(values);
                case FunctionType.Matrix:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Matrix was executed with the wrong number of parameters!");
                    }
                    if (!values[0].IsVectorResult)
                    {
                        return new ComputationResult("Matrix must be applied to a vector!");
                    }
                    return Matrix(values[0]);
                case FunctionType.IdentityMatrix:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("IdentityMatrix was executed with the wrong number of parameters!");
                    }
                    if (values[0].IsValue)
                    {
                        return new ComputationResult("IdentityMatrix must be applied to a vector, or a matrix!");
                    }
                    return IdentityMatrix(values[0]);
                case FunctionType.Log:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Log must be executed with one parameter!");
                    }
                    return Log(values);
                case FunctionType.Sqrt:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Sqrt must be executed with one parameter!");
                    }
                    return Sqrt(values);
                case FunctionType.If:
                    if (values.Length != 3)
                    {
                        return new ComputationResult("If requires at 3 parameters (condition, valueIfTrue, valueIfFalse)!");
                    }
                    return ComputeIf(values);
                case FunctionType.IfNaN:
                    if (values.Length != 2)
                    {
                        return new ComputationResult("IfNaN requires 2 parameters (original,replacement)!");
                    }
                    return ComputeIfNaN(values);
                case FunctionType.Normalize:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("Normalize requires 1 parameter, a matrix to be normalized.");
                    }
                    return ComputeNormalize(values);
                case FunctionType.NormalizeColumns:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("NormalizeColumns requires 1 parameter, a matrix to be normalized.");
                    }
                    return ComputeNormalizeColumns(values);
                case FunctionType.NormalizeRows:
                    if (values.Length != 1)
                    {
                        return new ComputationResult("NormalizeRows requires 1 parameter, a matrix to be normalized.");
                    }
                    return ComputeNormalizeRows(values);

            }
            return new ComputationResult("An undefined function was executed!");
        }

        private ComputationResult ComputeNormalizeColumns(ComputationResult[] values)
        {
            var toNormalize = values[0];
            if (toNormalize.IsValue)
            {
                return new ComputationResult($"{Start + 1}:Normalize requires its parameter to be of type Matrix, not a scalar.");
            }
            if (toNormalize.IsVectorResult)
            {
                return new ComputationResult($"{Start + 1}:Normalize requires its parameter to be of type Matrix, not a vector.");
            }
            var writeTo = toNormalize.Accumulator ? toNormalize.OdData : new Matrix(toNormalize.OdData);
            var readFrom = toNormalize.OdData;
            var rowLength = readFrom.RowCategories.Count;
            // This could be executed in parallel if proved to be more efficient
            var columnTotals = new float[readFrom.ColumnCategories.Count];
            var columnSpan = columnTotals.AsSpan();
            for (int i = 0; i < rowLength; i++)
            {
                var readRow = readFrom.GetRow(i);
                VectorHelper.Add(columnSpan, 0, columnSpan, 0, readRow, 0, readRow.Length);
            }
            System.Threading.Tasks.Parallel.For(0, rowLength, (int i) =>
            {
                var writeRow = writeTo.GetRow(i);
                var readRow = readFrom.GetRow(i);
                for (int j = 0; j < writeRow.Length; j++)
                {
                    writeRow[j] = columnTotals[j] != 0f ? readRow[j] / columnTotals[j] : 0f;
                }
            });
            return new ComputationResult(writeTo, true);
        }

        private ComputationResult ComputeNormalizeRows(ComputationResult[] values)
        {
            var toNormalize = values[0];
            if (toNormalize.IsValue)
            {
                return new ComputationResult($"{Start + 1}:Normalize requires its parameter to be of type Matrix, not a scalar.");
            }
            if (toNormalize.IsVectorResult)
            {
                return new ComputationResult($"{Start + 1}:Normalize requires its parameter to be of type Matrix, not a vector.");
            }
            var rowLength = toNormalize.OdData.RowCategories.Count;
            var writeTo = toNormalize.Accumulator ? toNormalize.OdData : new Matrix(toNormalize.OdData);
            var readFrom = toNormalize.OdData;
            System.Threading.Tasks.Parallel.For(0, rowLength, (int i) =>
            {
                var flatWrite = writeTo.GetRow(i);
                var flatRead = readFrom.GetRow(i);
                var denominator = VectorHelper.Sum(flatRead);
                if (denominator != 0f)
                {
                    VectorHelper.Divide(flatWrite, flatRead, denominator);
                }
                else if (flatRead == flatWrite)
                {
                    // we only need to accumulate if we are going to return a previously accumulated matrix.
                    VectorHelper.Memset(flatWrite, 0f);
                }
            });
            return new ComputationResult(writeTo, true);
        }

        private ComputationResult ComputeNormalize(ComputationResult[] values)
        {
            var toNormalize = values[0];
            if (toNormalize.IsValue)
            {
                return new ComputationResult($"{Start + 1}:Normalize requires its parameter to be of type Matrix, not a scalar.");
            }
            if (toNormalize.IsVectorResult)
            {
                return new ComputationResult($"{Start + 1}:Normalize requires its parameter to be of type Matrix, not a vector.");
            }
            var writeTo = toNormalize.Accumulator ? toNormalize.OdData : new Matrix(toNormalize.OdData);
            var flatWrite = writeTo.Data;
            var flatRead = toNormalize.OdData.Data;
            // sum the whole matrix in parallel using SIMD for each array
            var denominator = VectorHelper.Sum(flatRead, 0, flatRead.Length);
            if (denominator == 0f)
            {
                // only clear the write array if it was an accumulator
                if (flatRead == flatWrite)
                {
                    Array.Clear(flatWrite, 0, flatRead.Length);
                }
            }
            else
            {
                VectorHelper.Divide(flatWrite, flatRead, denominator);
            }
            return new ComputationResult(writeTo, true);
        }

        private ComputationResult ComputeIfNaN(ComputationResult[] values)
        {
            var condition = values[0];
            var replacement = values[1];
            // both must be the same size
            if (condition.IsValue && replacement.IsValue)
            {
                return new ComputationResult(!float.IsNaN(condition.LiteralValue) ? condition.LiteralValue : replacement.LiteralValue);
            }
            else if (condition.IsVectorResult && replacement.IsVectorResult)
            {
                var saveTo = values[0].Accumulator ? values[0].VectorData : new Vector(values[0].VectorData);
                VectorHelper.ReplaceIfNaN(saveTo.Data, condition.VectorData.Data, replacement.VectorData.Data, 0, replacement.VectorData.Data.Length);
                return new ComputationResult(saveTo, true, condition.Direction);
            }
            else if (condition.IsOdResult && replacement.IsOdResult)
            {
                var saveTo = values[0].Accumulator ? values[0].OdData : new Matrix(values[0].OdData);
                var flatSave = saveTo.Data;
                var flatCond = condition.OdData.Data;
                var flatRep = replacement.OdData.Data;
                int rows = values[0].OdData.RowCategories.Count;
                int columns = values[0].OdData.ColumnCategories.Count;
                System.Threading.Tasks.Parallel.For(0, rows, (int i) =>
                {
                    VectorHelper.ReplaceIfNaN(flatSave, flatCond, flatRep, i * columns, columns);
                });
                return new ComputationResult(saveTo, true);
            }
            return new ComputationResult($"{Start + 1}:The Condition and Replacement case of an IfNaN expression must be of the same dimensionality.");
        }

        private ComputationResult ComputeIf(ComputationResult[] values)
        {
            var condition = values[0];
            var ifTrue = values[1];
            var ifFalse = values[2];
            if ((ifTrue.IsValue & !ifFalse.IsValue)
                || (ifTrue.IsVectorResult & !ifFalse.IsVectorResult)
                || (ifTrue.IsOdResult & !ifFalse.IsOdResult))
            {
                return new ComputationResult($"{Start + 1}:The True and False case of an if expression must be of the same dimensionality.");
            }
            if (condition.IsValue)
            {
                // in all cases we can just move the result to the next level
                return condition.LiteralValue > 0f ? ifTrue : ifFalse;
            }
            else if (condition.IsVectorResult)
            {
                if (ifTrue.IsValue)
                {
                    var saveTo = values[0].Accumulator ? values[0].VectorData : new Vector(values[0].VectorData);
                    var result = saveTo.Data;
                    var cond = condition.VectorData.Data;
                    var t = ifTrue.LiteralValue;
                    var f = ifFalse.LiteralValue;
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = cond[i] > 0f ? t : f;
                    }
                    return new ComputationResult(saveTo, true, condition.Direction);
                }
                else if (ifTrue.IsVectorResult)
                {
                    var saveTo = values[0].Accumulator ? values[0].VectorData : new Vector(values[0].VectorData);
                    var result = saveTo.Data;
                    var cond = condition.VectorData.Data;
                    var t = ifTrue.VectorData.Data;
                    var f = ifFalse.VectorData.Data;
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = cond[i] > 0f ? t[i] : f[i];
                    }
                    return new ComputationResult(saveTo, true, condition.Direction);
                }
                else
                {
                    switch (condition.Direction)
                    {
                        case ComputationResult.VectorDirection.Unassigned:
                            return new ComputationResult($"{Start + 1}:The directionality of the condition vector is required when working with a matrix values.");
                        case ComputationResult.VectorDirection.Vertical:
                            {
                                var saveTo = values[1].Accumulator ? values[1].OdData : new Matrix(values[1].OdData);
                                var result = saveTo.Data;
                                var cond = condition.VectorData.Data;
                                var t = ifTrue.OdData.Data;
                                var f = ifFalse.OdData.Data;
                                for (int i = 0; i < cond.Length; i++)
                                {
                                    var toAssign = cond[i] > 0 ? t : f;
                                    var rowOffset = i * cond.Length;
                                    Array.Copy(toAssign, rowOffset, result, rowOffset, cond.Length);
                                }
                                return new ComputationResult(saveTo, true);
                            }
                        case ComputationResult.VectorDirection.Horizontal:
                            {
                                var saveTo = values[1].Accumulator ? values[1].OdData : new Matrix(values[1].OdData);
                                var result = saveTo.Data;
                                var cond = condition.VectorData.Data;
                                var t = ifTrue.OdData.Data;
                                var f = ifFalse.OdData.Data;
                                for (int i = 0; i < cond.Length; i++)
                                {
                                    int rowOffset = i * cond.Length;
                                    for (int j = 0; j < cond.Length; j++)
                                    {
                                        result[rowOffset + j] = cond[j] > 0 ? t[rowOffset + j] : f[rowOffset + j];
                                    }
                                }
                                return new ComputationResult(saveTo, true);
                            }
                    }
                }
            }
            if (condition.IsOdResult)
            {
                if (!ifTrue.IsOdResult)
                {
                    return new ComputationResult($"{Start + 1}:The True and False cases must be a Matrix when the condition is a matrix.");
                }
                var saveTo = values[0].Accumulator ? values[0].OdData : new Matrix(values[0].OdData);
                var cond = condition.OdData.Data;
                var tr = ifTrue.OdData.Data;
                var fa = ifFalse.OdData.Data;
                var sa = saveTo.Data;
                var columnSize = condition.OdData.GetFlatRowIndex(1);
                // this will never have a remainder
                var rowSize = cond.Length / columnSize;
                System.Threading.Tasks.Parallel.For(0, rowSize, (int row) =>
                {
                    var start = columnSize * row;
                    var end = columnSize * (row + 1);
                    for (int i = start; i < end; i++)
                    {
                        sa[i] = cond[i] > 0f ? tr[i] : fa[i];
                    }
                });

                return new ComputationResult(saveTo, true);
            }
            return new ComputationResult($"{Start + 1}:This combination of parameter types has not been implemented for if!");
        }

        private ComputationResult Log(ComputationResult[] values)
        {
            if (values[0].IsValue)
            {
                return new ComputationResult((float)Math.Log(values[0].LiteralValue));
            }
            else if (values[0].IsVectorResult)
            {
                var saveTo = values[0].Accumulator ? values[0].VectorData : new Vector(values[0].VectorData);
                var flat = saveTo.Data;
                var source = values[0].VectorData.Data;
                VectorHelper.Log(flat, 0, source, 0, source.Length);
                return new ComputationResult(saveTo, true);
            }
            else
            {
                var saveTo = values[0].Accumulator ? values[0].OdData : new Matrix(values[0].OdData);
                var flat = saveTo.Data;
                var source = values[0].OdData.Data;
                VectorHelper.Log(flat, 0, source, 0, source.Length);
                return new ComputationResult(saveTo, true);
            }
        }

        private ComputationResult Sqrt(ComputationResult[] values)
        {
            if (values[0].IsValue)
            {
                return new ComputationResult((float)Math.Sqrt(values[0].LiteralValue));
            }
            else if (values[0].IsVectorResult)
            {
                var saveTo = values[0].Accumulator ? values[0].VectorData : new Vector(values[0].VectorData);
                var source = values[0].VectorData.Data;
                var flat = saveTo.Data;
                // x^0.5 is sqrt
                VectorHelper.Pow(flat, source, 0.5f);
                return new ComputationResult(saveTo, true);
            }
            else
            {
                var saveTo = values[0].Accumulator ? values[0].OdData : new Matrix(values[0].OdData);
                var source = values[0].OdData.Data;
                var flat = saveTo.Data;
                // x^0.5 is sqrt
                VectorHelper.Pow(flat, source, 0.5f);
                return new ComputationResult(saveTo, true);
            }
        }

        private ComputationResult IdentityMatrix(ComputationResult computationResult)
        {
            Matrix ret;
            if (computationResult.IsVectorResult)
            {
                var vector = computationResult.VectorData;
                ret = new Matrix(vector);
            }
            else
            {
                var matrix = computationResult.OdData;
                ret = new Matrix(matrix);
            }
            var step = ret.RowCategories.Count + 1;
            var flatRet = ret.Data;
            for (int i = 0; i < flatRet.Length; i += step)
            {
                flatRet[i] = 1.0f;
            }
            return new ComputationResult(ret, true);
        }

        private ComputationResult Matrix(ComputationResult computationResult)
        {
            var vectorData = computationResult.VectorData;
            var newMatrix = new Matrix(vectorData);
            var rowSize = newMatrix.RowCategories.Count;
            var flatVector = vectorData.Data;
            var flatMatrix = newMatrix.Data;
            switch (computationResult.Direction)
            {
                case ComputationResult.VectorDirection.Unassigned:
                    return new ComputationResult("Matrix was executed with an unassigned orientation vector!");
                case ComputationResult.VectorDirection.Vertical:
                    // each row is the single value
                    for (int i = 0; i < flatVector.Length; i++)
                    {
                        VectorHelper.Set(flatMatrix, i * rowSize, flatVector[i], rowSize);
                    }
                    break;
                case ComputationResult.VectorDirection.Horizontal:
                    // each column is the single value
                    for (int i = 0; i < flatMatrix.Length; i += flatVector.Length)
                    {
                        Array.Copy(flatVector, 0, flatMatrix, i, flatVector.Length);
                    }
                    break;
            }
            return new ComputationResult(newMatrix, true);
        }

        private ComputationResult ZeroMatrix(ComputationResult[] values)
        {
            if (values[0].VectorData != null)
            {
                return new ComputationResult(new Matrix(values[0].VectorData), true);
            }
            else
            {
                return new ComputationResult(new Matrix(values[0].OdData), true);
            }
        }

        private ComputationResult Avg(ComputationResult computationResult)
        {
            if (computationResult.IsVectorResult)
            {
                var flat = computationResult.VectorData.Data;
                return new ComputationResult(VectorHelper.Sum(flat, 0, flat.Length) / flat.Length);
            }
            else
            {
                var flat = computationResult.OdData.Data;
                return new ComputationResult(VectorHelper.Sum(flat, 0, flat.Length) / flat.Length);
            }
        }

        private ComputationResult Abs(ComputationResult computationResult)
        {
            if (computationResult.IsValue)
            {
                return new ComputationResult(Math.Abs(computationResult.LiteralValue));
            }
            else if (computationResult.IsVectorResult)
            {
                var retVector = computationResult.Accumulator ? computationResult.VectorData : new Vector(computationResult.VectorData);
                var flat = retVector.Data;
                VectorHelper.Abs(flat, computationResult.VectorData.Data);
                return new ComputationResult(retVector, true);
            }
            else
            {
                var retMatrix = computationResult.Accumulator ? computationResult.OdData : new Matrix(computationResult.OdData);
                var flat = retMatrix.Data;
                VectorHelper.Abs(flat, computationResult.OdData.Data);
                return new ComputationResult(retMatrix, true);
            }
        }

        private ComputationResult Sum(ComputationResult computationResult)
        {
            if (computationResult.IsVectorResult)
            {
                return new ComputationResult(VectorHelper.Sum(computationResult.VectorData.Data, 0, computationResult.VectorData.Data.Length));
            }
            else if (computationResult.IsOdResult)
            {
                var data = computationResult.OdData.Data;
                var total = VectorHelper.Sum(data, 0, data.Length);
                return new ComputationResult(total);
            }
            return new ComputationResult("Unknown data type to sum!");
        }

        private ComputationResult TransposeOd(ComputationResult computationResult)
        {
            var ret = computationResult.Accumulator ? computationResult.OdData : new Matrix(computationResult.OdData);
            var flatRet = ret.Data;
            var flatOrigin = computationResult.OdData.Data;
            var rowLength = ret.RowCategories.Count;
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = i + 1; j < rowLength; j++)
                {
                    var temp = flatOrigin[i * rowLength + j];
                    flatRet[i * rowLength + j] = flatOrigin[j * rowLength + i];
                    flatRet[j * rowLength + i] = temp;
                }
            }
            // if this is a new matrix copy the diagonal
            if (!computationResult.Accumulator)
            {
                for (int i = 0; i < rowLength; i++)
                {
                    flatRet[i * rowLength] = flatOrigin[i * rowLength];
                }
            }
            return new ComputationResult(ret, true);
        }

        private ComputationResult SumColumns(ComputationResult computationResult)
        {
            var ret = new Vector(computationResult.OdData.RowCategories);
            var flatRet = ret.Data;
            var flatData = computationResult.OdData.Data;
            var rowSize = ret.Categories.Count;
            for (int i = 0; i < flatRet.Length; i++)
            {
                VectorHelper.Add(flatRet, 0, flatRet, 0, flatData, i * rowSize, rowSize);
            }
            return new ComputationResult(ret, true, ComputationResult.VectorDirection.Horizontal);
        }

        private ComputationResult SumRows(ComputationResult computationResult)
        {
            var ret = new Vector(computationResult.OdData.RowCategories);
            var flatRet = ret.Data;
            var flatData = computationResult.OdData.Data;
            var rowSize = ret.Categories.Count;
            for (int i = 0; i < flatRet.Length; i++)
            {
                flatRet[i] = VectorHelper.Sum(flatData, i * rowSize, rowSize);
            }
            return new ComputationResult(ret, true, ComputationResult.VectorDirection.Vertical);
        }

        private ComputationResult AvgColumns(ComputationResult computationResult)
        {
            var data = computationResult.OdData;
            var ret = new Vector(data.RowCategories);
            var flatRet = ret.Data;
            var flatData = data.Data;
            var rowSize = ret.Categories.Count;
            for (int i = 0; i < flatRet.Length; i++)
            {
                VectorHelper.Add(flatRet, 0, flatRet, 0, flatData, i * rowSize, rowSize);
            }
            VectorHelper.Multiply(flatRet, flatRet, 1.0f / flatRet.Length);
            return new ComputationResult(ret, true, ComputationResult.VectorDirection.Horizontal);
        }

        private ComputationResult AvgRows(ComputationResult computationResult)
        {
            var data = computationResult.OdData;
            var ret = new Vector(data.RowCategories);
            var flatRet = ret.Data;
            var flatData = data.Data;
            var rowSize = ret.Categories.Count;
            for (int i = 0; i < flatRet.Length; i++)
            {
                flatRet[i] = VectorHelper.Sum(flatData, i * rowSize, rowSize);
            }
            VectorHelper.Multiply(flatRet, flatRet, 1.0f / flatRet.Length);
            return new ComputationResult(ret, true, ComputationResult.VectorDirection.Vertical);
        }

        private ComputationResult Length(ComputationResult computationResult)
        {
            if (computationResult.IsOdResult)
            {
                return new ComputationResult(computationResult.OdData.Data.Length);
            }
            if (computationResult.IsVectorResult)
            {
                return new ComputationResult(computationResult.VectorData.Data.Length);
            }
            return new ComputationResult("An unknown data type was processed through Length!");
        }

        private ComputationResult LengthRows(ComputationResult computationResult)
        {
            if (computationResult.IsOdResult)
            {
                var data = computationResult.OdData;
                var ret = new Vector(data.RowCategories);
                var flatRet = ret.Data;
                var flatData = data.Data;
                VectorHelper.Set(flatData, flatRet.Length);
                return new ComputationResult(ret, true, ComputationResult.VectorDirection.Vertical);
            }
            return new ComputationResult("An unknown data type was processed through LengthRows!");
        }

        private ComputationResult LengthColumns(ComputationResult computationResult)
        {
            if (computationResult.IsOdResult)
            {
                var data = computationResult.OdData;
                var ret = new Vector(data.RowCategories);
                var flatRet = ret.Data;
                var flatData = data.Data;
                VectorHelper.Set(flatData, flatRet.Length);
                return new ComputationResult(ret, true, ComputationResult.VectorDirection.Horizontal);
            }
            return new ComputationResult("An unknown data type was processed through LengthColumns!");
        }
    }
}
