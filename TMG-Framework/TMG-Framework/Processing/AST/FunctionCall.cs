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
            Log
        }

        private FunctionType Type;

        private Expression[] Parameters;

        private FunctionCall(int start, FunctionType call, Expression[] parameters) : base(start)
        {
            Parameters = parameters;
            Type = call;
        }

        internal override bool OptimizeAst(ref Expression ex, ref string error)
        {
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (!Parameters[i].OptimizeAst(ref Parameters[i], ref error))
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
                default:
                    error = "The function '" + call + "' is undefined!";
                    return false;
            }
        }

        public override ComputationResult Evaluate(IModule[] dataSources)
        {
            // first evaluate the parameters
            var values = new ComputationResult[Parameters.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Parameters[i].Evaluate(dataSources);
                if (values[i].Error)
                {
                    return values[i];
                }
            }

            switch (Type)
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

            }
            return new ComputationResult("An undefined function was executed!");
        }

        private ComputationResult Log(ComputationResult[] values)
        {
            if (values[0].IsValue)
            {
                return new ComputationResult((float)Math.Log(values[0].LiteralValue));
            }
            else if (values[0].IsVectorResult)
            {
                var saveTo = values[0].Accumulator ? values[0].VectorData : new SparseVector(values[0].VectorData);
                var flat = saveTo.Data;
                VectorHelper.Log(flat, 0, flat, 0, flat.Length);
                return new ComputationResult(saveTo, true);
            }
            else
            {
                var saveTo = values[0].Accumulator ? values[0].OdData : new SparseMatrix(values[0].OdData);
                var flat = saveTo.Data;
                VectorHelper.Log(flat, 0, flat, 0, flat.Length);
                return new ComputationResult(saveTo, true);
            }
        }

        private ComputationResult IdentityMatrix(ComputationResult computationResult)
        {
            SparseMatrix ret;
            if (computationResult.IsVectorResult)
            {
                var vector = computationResult.VectorData;
                ret = new SparseMatrix(vector);
            }
            else
            {
                var matrix = computationResult.OdData;
                ret = new SparseMatrix(matrix);
            }
            var step = ret.Map.Count + 1;
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
            var newMatrix = new SparseMatrix(vectorData);
            var rowSize = newMatrix.Map.Count;
            var flatVector = vectorData.Data;
            var flatMatrix = newMatrix.Data;
            switch (computationResult.Direction)
            {
                case ComputationResult.VectorDirection.Unassigned:
                    return new ComputationResult("Matrix was executed with an unassigned orientation vector!");
                case ComputationResult.VectorDirection.Vertical:
                    // each row is the single value
                    for (int i = 0; i < flatVector.Length; i ++)
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
                return new ComputationResult(new SparseMatrix(values[0].VectorData), true);
            }
            else
            {
                return new ComputationResult(new SparseMatrix(values[0].OdData), true);
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
                var retVector = computationResult.Accumulator ? computationResult.VectorData : new SparseVector(computationResult.VectorData);
                var flat = retVector.Data;
                VectorHelper.Abs(flat, computationResult.VectorData.Data);
                return new ComputationResult(retVector, true);
            }
            else
            {
                var retMatrix = computationResult.Accumulator ? computationResult.OdData : new SparseMatrix(computationResult.OdData);
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
            var ret = computationResult.Accumulator ? computationResult.OdData : new SparseMatrix(computationResult.OdData);
            var flatRet = ret.Data;
            var flatOrigin = computationResult.OdData.Data;
            var rowLength = ret.Map.Count;
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
            var ret = new SparseVector(computationResult.OdData.Map);
            var flatRet = ret.Data;
            var flatData = computationResult.OdData.Data;
            var rowSize = ret.Map.Count;
            for (int i = 0; i < flatRet.Length; i++)
            {
                VectorHelper.Add(flatRet, 0, flatRet, 0, flatData, i * rowSize, rowSize);
            }
            return new ComputationResult(ret, true, ComputationResult.VectorDirection.Horizontal);
        }

        private ComputationResult SumRows(ComputationResult computationResult)
        {
            var ret = new SparseVector(computationResult.OdData.Map);
            var flatRet = ret.Data;
            var flatData = computationResult.OdData.Data;
            var rowSize = ret.Map.Count;
            for (int i = 0; i < flatRet.Length; i++)
            {
                flatRet[i] = VectorHelper.Sum(flatData, i * rowSize, rowSize);
            }
            return new ComputationResult(ret, true, ComputationResult.VectorDirection.Vertical);
        }

        private ComputationResult AvgColumns(ComputationResult computationResult)
        {
            var data = computationResult.OdData;
            var ret = new SparseVector(data.Map);
            var flatRet = ret.Data;
            var flatData = data.Data;
            var rowSize = ret.Map.Count;
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
            var ret = new SparseVector(data.Map);
            var flatRet = ret.Data;
            var flatData = data.Data;
            var rowSize = ret.Map.Count;
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
                var ret = new SparseVector(data.Map);
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
                var ret = new SparseVector(data.Map);
                var flatRet = ret.Data;
                var flatData = data.Data;
                VectorHelper.Set(flatData, flatRet.Length);
                return new ComputationResult(ret, true, ComputationResult.VectorDirection.Horizontal);
            }
            return new ComputationResult("An unknown data type was processed through LengthColumns!");
        }
    }
}
