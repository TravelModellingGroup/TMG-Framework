/*
    Copyright 2026 University of Toronto

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

using TMG.Processing;
using TMG.Test.Utilities;
using TMG.Utilities;
using XTMF2;
using TestHelper = TMG.Test.Utilities.Helper;

namespace TMG.Test.Processing;

[TestClass]
public class TestEvaluateMinMax
{
    [TestMethod]
    public void ReductionReturnsExtremaForScalarsVectorsAndMatrices()
    {
        var categories = MapHelper.LoadMap(2);
        using var vector = new Vector(categories);
        vector.Data[0] = -2f;
        vector.Data[1] = 5f;
        using var matrix = CreateMatrix(categories, -1f, 2f, 3f, -4f);

        Assert.AreEqual(3f, EvaluateScalar("Max(A)", [TestHelper.CreateParameter(matrix, "A")]));
        Assert.AreEqual(-4f, EvaluateScalar("Min(A)", [TestHelper.CreateParameter(matrix, "A")]));
        Assert.AreEqual(5f, EvaluateScalar("Max(V)", [TestHelper.CreateParameter(vector, "V")]));
        Assert.AreEqual(-2f, EvaluateScalar("Min(V)", [TestHelper.CreateParameter(vector, "V")]));
        Assert.AreEqual(3.14f, EvaluateScalar("Max(3.14)", Array.Empty<IModule>()));
        Assert.AreEqual(-3.14f, EvaluateScalar("Min(-3.14)", Array.Empty<IModule>()));
    }

    [TestMethod]
    public void MatrixVectorAndMatrixScalarOperationsAllowEitherArgumentOrder()
    {
        var categories = MapHelper.LoadMap(2);
        using var vector = new Vector(categories);
        vector.Data[0] = -1f;
        vector.Data[1] = 3f;
        using var matrix = CreateMatrix(categories, -1f, 2f, 3f, -4f);
        var variables = new IModule[]
        {
            TestHelper.CreateParameter(matrix, "A"),
            TestHelper.CreateParameter(vector, "V")
        };

        AssertMatrix(EvaluateMatrix("Max(A, AsVertical(V))", variables), -1f, 2f, 3f, 3f);
        AssertMatrix(EvaluateMatrix("Max(AsVertical(V), A)", variables), -1f, 2f, 3f, 3f);
        AssertMatrix(EvaluateMatrix("Min(A, AsVertical(V))", variables), -1f, -1f, 3f, -4f);
        AssertMatrix(EvaluateMatrix("Min(AsVertical(V), A)", variables), -1f, -1f, 3f, -4f);
        AssertMatrix(EvaluateMatrix("Max(A, 1)", variables), 1f, 2f, 3f, 1f);
        AssertMatrix(EvaluateMatrix("Max(1, A)", variables), 1f, 2f, 3f, 1f);
        AssertMatrix(EvaluateMatrix("Min(A, 1)", variables), -1f, 1f, 1f, -4f);
        AssertMatrix(EvaluateMatrix("Min(1, A)", variables), -1f, 1f, 1f, -4f);
    }

    [TestMethod]
    public void ScalarVectorOperationsAllowEitherArgumentOrder()
    {
        var categories = MapHelper.LoadMap(2);
        using var vector = new Vector(categories);
        vector.Data[0] = -2f;
        vector.Data[1] = 5f;
        var variables = new IModule[] { TestHelper.CreateParameter(vector, "V") };

        AssertVector(EvaluateVector("Max(V, 3)", variables), 3f, 5f);
        AssertVector(EvaluateVector("Max(3, V)", variables), 3f, 5f);
        AssertVector(EvaluateVector("Min(V, 3)", variables), -2f, 3f);
        AssertVector(EvaluateVector("Min(3, V)", variables), -2f, 3f);
    }

    [TestMethod]
    public void VectorVectorResultUsesTheFirstVectorsOrientation()
    {
        var categories = MapHelper.LoadMap(2);
        using var first = new Vector(categories);
        first.Data[0] = 1f;
        first.Data[1] = 4f;
        using var second = new Vector(categories);
        second.Data[0] = 3f;
        second.Data[1] = 2f;
        var variables = new IModule[]
        {
            TestHelper.CreateParameter(first, "A"),
            TestHelper.CreateParameter(second, "B")
        };

        AssertMatrix(EvaluateMatrix("Matrix(Max(AsVertical(A), AsHorizontal(B)))", variables), 3f, 3f, 4f, 4f);
        AssertMatrix(EvaluateMatrix("Matrix(Max(AsHorizontal(B), AsVertical(A)))", variables), 3f, 4f, 3f, 4f);
    }

    [TestMethod]
    public void InvalidOrientationAndArgumentCountsThrowRuntimeErrors()
    {
        var categories = MapHelper.LoadMap(2);
        using var vector = new Vector(categories);
        using var matrix = CreateMatrix(categories, -1f, 2f, 3f, -4f);
        var variables = new IModule[]
        {
            TestHelper.CreateParameter(matrix, "A"),
            TestHelper.CreateParameter(vector, "V")
        };

        TestHelper.ThrowsException<XTMFRuntimeException>(() => EvaluateMatrix("Max(A, V)", variables));
        TestHelper.ThrowsException<XTMFRuntimeException>(() => EvaluateScalar("Min(1, 2, 3)", Array.Empty<IModule>()));
    }

    private static Matrix CreateMatrix(Categories categories, params float[] values)
    {
        var matrix = new Matrix(categories, categories);
        values.AsSpan().CopyTo(matrix.Data);
        return matrix;
    }

    private static float EvaluateScalar(string expression, IModule[] variables)
    {
        return new EvaluateScalar
        {
            Expression = TestHelper.CreateParameter(expression),
            Variables = variables
        }.Invoke();
    }

    private static Vector EvaluateVector(string expression, IModule[] variables)
    {
        return new EvaluateVector
        {
            Expression = TestHelper.CreateParameter(expression),
            Variables = variables
        }.Invoke();
    }

    private static Matrix EvaluateMatrix(string expression, IModule[] variables)
    {
        return new EvaluateMatrix
        {
            Expression = TestHelper.CreateParameter(expression),
            Variables = variables
        }.Invoke();
    }

    private static void AssertMatrix(Matrix matrix, params float[] expected)
    {
        using (matrix)
        {
            CollectionAssert.AreEqual(expected, matrix.Data.ToArray());
        }
    }

    private static void AssertVector(Vector vector, params float[] expected)
    {
        using (vector)
        {
            CollectionAssert.AreEqual(expected, vector.Data.ToArray());
        }
    }
}
