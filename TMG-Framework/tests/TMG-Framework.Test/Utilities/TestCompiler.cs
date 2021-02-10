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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TMG;
using TMG.Frameworks.Data.Processing.AST;
using XTMF2;

namespace XTMF.Testing.TMG.Data
{
    [TestClass]
    public class TestCompiler
    {
        readonly Categories matrixCategories;
        readonly Categories vectorCategories;

        public TestCompiler()
        {
            string error = null;
            matrixCategories = Categories.CreateCategories(new List<int>() { 1, 2 }, ref error);
            vectorCategories = Categories.CreateCategories(new List<int>() { 1, 2, 3, 4 }, ref error);
        }
        /// <summary>
        /// Create a new simple matrix for testing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <returns></returns>
        private IFunction<Matrix> CreateMatrixData(string name, float m11, float m12, float m21, float m22)
        {
            var matrix = new Matrix(matrixCategories, matrixCategories);
            matrix.Data[0] = m11;
            matrix.Data[1] = m12;
            matrix.Data[2] = m21;
            matrix.Data[3] = m22;
            return new MatrixSource(matrix) { Name = name };
        }

        /// <summary>
        /// Create a new simple matrix for testing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <returns></returns>
        private IFunction<Vector> CreateVectorData(string name, float m1, float m2, float m3, float m4)
        {
            var ret = new Vector(vectorCategories);
            ret.Data[0] = m1;
            ret.Data[1] = m2;
            ret.Data[2] = m3;
            ret.Data[3] = m4;
            return new VectorSource(ret) { Name = name };
        }

        /// <summary>
        /// Create a new simple matrix for testing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <returns></returns>
        private IFunction<float> CreateScalarData(string name, float m1)
        {
            return new ScalarSource(m1) { Name = name };
        }

        /// <summary>
        /// Create a new simple vector for testing.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private IFunction<Vector> CreateData(string name, float m1, float m2)
        {
            var vector = new Vector(matrixCategories);
            vector.Data[0] = m1;
            vector.Data[1] = m2;
            return new VectorSource(vector) { Name = name };
        }

        [TestMethod]
        public void TestMatrixAdd()
        {
            CompareMatrix("A + B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 6.0f, 9.0f, 12.0f);
        }

        [TestMethod]
        public void TestAddLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) + B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 6.0f, 7.0f, 10.0f);
        }

        [TestMethod]
        public void TestAddLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) + B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 5.0f, 8.0f, 10.0f);
        }

        [TestMethod]
        public void TestAddLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B + AsHorizontal(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 6.0f, 7.0f, 10.0f);
        }

        [TestMethod]
        public void TestAddLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B + AsVertical(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 5.0f, 8.0f, 10.0f);
        }

        [TestMethod]
        public void TestMultiplyLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) * B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 8.0f, 6.0f, 16.0f);
        }

        [TestMethod]
        public void TestMultiplyLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) * B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 4.0f, 12.0f, 16.0f);
        }

        [TestMethod]
        public void TestMultiplyLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B * AsHorizontal(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 8.0f, 6.0f, 16.0f);
        }

        [TestMethod]
        public void TestMultiplyLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B * AsVertical(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 4.0f, 12.0f, 16.0f);
        }

        [TestMethod]
        public void TestDivideLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) / B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 0.5f, 0.5f, 1.0f / 6.0f, 0.25f);
        }

        [TestMethod]
        public void TestDivideLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) / B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 0.5f, 0.25f, 2.0f / 6.0f, 0.25f);
        }

        [TestMethod]
        public void TestDivideLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B / AsHorizontal(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 2.0f, 6.0f, 4.0f);
        }

        [TestMethod]
        public void TestDivideLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B / AsVertical(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 4.0f, 3.0f, 4.0f);
        }

        [TestMethod]
        public void TestSubtractLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) - B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, -1.0f, -2.0f, -5.0f, -6.0f);
        }

        [TestMethod]
        public void TestSubtractLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) - B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, -1.0f, -3.0f, -4.0f, -6.0f);
        }

        [TestMethod]
        public void TestSubtractLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B - AsHorizontal(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 1.0f, 2.0f, 5.0f, 6.0f);
        }

        [TestMethod]
        public void TestSubtractLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B - AsVertical(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 1.0f, 3.0f, 4.0f, 6.0f);
        }

        [TestMethod]
        public void TestAndLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) == 1 & B == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 1.0f, 0.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestAndLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) == 1 & B == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 1.0f, 0.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestAndLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B == 1 & AsHorizontal(A) == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 1.0f, 0.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestAndLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B == 1 & AsVertical(A) == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 1.0f, 0.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestOrLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) == 1 | B == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 0.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestOrLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) == 1 | B == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestOrLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B == 1 | AsHorizontal(A) == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 0.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestOrLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B == 1 | AsVertical(A) == 1", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestLessThanLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("(AsHorizontal(A) == 1) < (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestLessThanLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("(AsVertical(A) == 1) < (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestLessThanLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("(B == 1) < (AsHorizontal(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 0.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestLessThanLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("(B == 1) < (AsVertical(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 1.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestLessThanOrEqualLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("(AsHorizontal(A) == 1) <= (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestLessThanOrEqualLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("(AsVertical(A) == 1) <= (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 0.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestLessThanOrEqualLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("(B == 1) <= (AsHorizontal(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestLessThanOrEqualLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("(B == 1) <= (AsVertical(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestEqualLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("(AsHorizontal(A) == 1) == (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestEqualLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("(AsVertical(A) == 1) == (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 0.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestEqualLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("(B == 1) == (AsHorizontal(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 1.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestEqualLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("(B == 1) == (AsVertical(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 1.0f, 0.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestNotEqualLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("(AsHorizontal(A) == 1) != (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 0.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestNotEqualLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("(AsVertical(A) == 1) != (B == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 1.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestNotEqualLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("(B == 1) != (AsHorizontal(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 0.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestNotEqualLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("(B == 1) != (AsVertical(A) == 1)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 0, 1)
            }, 0.0f, 1.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestPowLHSVectorRHSMatrixHorizontal()
        {
            CompareMatrix("AsHorizontal(A) ^ B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 1.0f, 16.0f, 1.0f, 256.0f);
        }

        [TestMethod]
        public void TestPowLHSVectorRHSMatrixVertical()
        {
            CompareMatrix("AsVertical(A) ^ B", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 1.0f, 1.0f, 64.0f, 256.0f);
        }

        [TestMethod]
        public void TestPowLHSMatrixRHSVectorHorizontal()
        {
            CompareMatrix("B ^ AsHorizontal(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 16.0f, 6.0f, 64.0f);
        }

        [TestMethod]
        public void TestPowLHSMatrixRHSVectorVertical()
        {
            CompareMatrix("B ^ AsVertical(A)", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 4.0f, 36.0f, 64.0f);
        }

        [TestMethod]
        public void TestMatrixSubtract()
        {
            CompareMatrix("A - B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, -1.0f, -2.0f, -3.0f, -4.0f);
        }
        [TestMethod]
        public void TestMatrixVectorSubtract()
        {
            CompareMatrix("A - SumColumns(B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, -7.0f, -10.0f, -5.0f, -8.0f);
        }

        [TestMethod]
        public void TestMatrixVectorSubtract2()
        {
            CompareMatrix("A - SumRows(B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, -5.0f, -4.0f, -11.0f, -10.0f);
        }

        [TestMethod]
        public void TestMatrixAddWithBrackets()
        {
            CompareMatrix("(A) + (B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 6.0f, 9.0f, 12.0f);
        }

        [TestMethod]
        public void TestMatrixAddWithDoubleBrackets()
        {
            CompareMatrix("((A)) + ((B))", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 6.0f, 9.0f, 12.0f);
        }

        [TestMethod]
        public void TestMatrixAddWithTripleBrackets()
        {
            CompareMatrix("(((A)) + ((B)))", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 6.0f, 9.0f, 12.0f);
        }

        [TestMethod]
        public void TestMatrixSumRows()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("SumRows(A + B)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsVectorResult);
            Assert.IsTrue(result.Direction == ComputationResult.VectorDirection.Vertical);
            var flat = result.VectorData.Data;
            Assert.AreEqual(9.0f, flat[0], 0.00001f);
            Assert.AreEqual(21.0f, flat[1], 0.00001f);
        }

        [TestMethod]
        public void TestMatrixSumColumns()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("SumColumns(A + B)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsVectorResult);
            Assert.IsTrue(result.Direction == ComputationResult.VectorDirection.Horizontal);
            var flat = result.VectorData.Data;
            Assert.AreEqual(12.0f, flat[0], 0.00001f);
            Assert.AreEqual(18.0f, flat[1], 0.00001f);
        }

        [TestMethod]
        public void TestMatrixAsHorizontal()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("AsHorizontal(SumRows(A + B))", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsVectorResult);
            Assert.IsTrue(result.Direction == ComputationResult.VectorDirection.Horizontal);
            var flat = result.VectorData.Data;
            Assert.AreEqual(9.0f, flat[0], 0.00001f);
            Assert.AreEqual(21.0f, flat[1], 0.00001f);
        }

        [TestMethod]
        public void TestMatrixSum()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Sum(A + B)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(30.0f, result.LiteralValue, 0.00001f);
        }

        [TestMethod]
        public void TestVectorSum()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Sum(SumRows(A + B))", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(30.0f, result.LiteralValue, 0.00001f);
        }

        [TestMethod]
        public void TestMatrixTranspose()
        {
            CompareMatrix("Transpose(A + B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 3.0f, 9.0f, 6.0f, 12.0f);
        }

        [TestMethod]
        public void TestMatrixAbs()
        {
            CompareMatrix("Abs(A - B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 1.0f, 2.0f, 3.0f, 4.0f);
        }

        [TestMethod]
        public void TestVectorAbs()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Abs(SumRows(A) - SumRows(B))", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsVectorResult);
            var flat = result.VectorData.Data;
            Assert.AreEqual(3.0f, flat[0], 0.00001f);
            Assert.AreEqual(7.0f, flat[1], 0.00001f);
        }

        [TestMethod]
        public void TestScalarAbs()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Abs(Sum(A) - Sum(B))", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(10.0f, result.LiteralValue, 0.00001f);
        }

        [TestMethod]
        public void TestMatrixAvg()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Avg(A - B)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(-2.5f, result.LiteralValue, 0.00001f);
        }

        [TestMethod]
        public void TestVectorAvg()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Avg(SumRows(A) - SumRows(B))", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(-5.0f, result.LiteralValue, 0.00001f);
        }

        [TestMethod]
        public void TestMatrixAvgRows()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("AvgRows(A)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsVectorResult);
            Assert.IsTrue(result.Direction == ComputationResult.VectorDirection.Vertical);
            var flat = result.VectorData.Data;
            Assert.AreEqual(1.5f, flat[0], 0.00001f);
            Assert.AreEqual(3.5f, flat[1], 0.00001f);
        }

        [TestMethod]
        public void TestMatrixAvgColumns()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("AvgColumns(A)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsVectorResult);
            Assert.IsTrue(result.Direction == ComputationResult.VectorDirection.Horizontal);
            var flat = result.VectorData.Data;
            Assert.AreEqual(2.0f, flat[0], 0.00001f);
            Assert.AreEqual(3.0f, flat[1], 0.00001f);
        }

        [TestMethod]
        public void TestPI()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("PI( )", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[0]);
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual((float)Math.PI, result.LiteralValue, 0.000001f);
        }

        [TestMethod]
        public void TestE()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("E( )", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[0]);
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual((float)Math.E, result.LiteralValue, 0.000001f);
        }

        [TestMethod]
        public void TestMatrixExponent()
        {
            CompareMatrix("A ^ B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 1.0f, 16.0f, 729.0f, 65536.0f);
        }

        [TestMethod]
        public void TestMatrixExponent2()
        {
            CompareMatrix("(A + 1) ^ (B - 1)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2.0f, 27.0f, 1024.0f, 78125.0f);
        }

        [TestMethod]
        public void TestMatrixEqual()
        {
            CompareMatrix("A == B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 1.0f, 1.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestMatrixNotEqual()
        {
            CompareMatrix("A != B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 0.0f, 0.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestMatrixLessThan()
        {
            CompareMatrix("A < B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 0.0f, 0.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestMatrixGreaterThan()
        {
            CompareMatrix("A > B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestMatrixLessThanOrEqual()
        {
            CompareMatrix("A <= B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 1.0f, 1.0f, 1.0f, 0.0f);
        }

        [TestMethod]
        public void TestMatrixGreaterThanOrEqual()
        {
            CompareMatrix("A >= B", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 1.0f, 1.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestMatrixCompareAdvanced()
        {
            CompareMatrix("(A == B) + (A != B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 1.0f, 1.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestAnd()
        {
            CompareMatrix("(A == B) & (A != B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 0.0f, 0.0f, 0.0f, 0.0f);
        }

        [TestMethod]
        public void TestOr()
        {
            CompareMatrix("(A == B) | (A != B)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 1.0f, 1.0f, 1.0f, 1.0f);
        }

        [TestMethod]
        public void TestMatrixLength()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("Length(A + 1) + Length(B - 1)", out Expression ex, ref error));
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 2, 4, 6, 8)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(8.0f, result.LiteralValue, 0.00001f);
        }

        [TestMethod]
        public void TestMatrix()
        {
            CompareMatrix("Matrix(asHorizontal(E)) + Matrix(asVertical(E))", new IModule[]
            {
                CreateData("E", 9, 10)
            }, 18.0f, 19.0f, 19.0f, 20.0f);
        }

        [TestMethod]
        public void TestIdentity()
        {
            var data = new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateData("E", 9, 10)
            };
            CompareMatrix("identityMatrix(E)", data, 1.0f, 0.0f, 0.0f, 1.0f);
            CompareMatrix("identityMatrix(A)", data, 1.0f, 0.0f, 0.0f, 1.0f);
        }

        [TestMethod]
        public void TestOptimizeFusedMultiplyAddIsOptimizedIn()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("A * B + A", out Expression ex, ref error), $"Unable to compile 'A * B + A'\r\n{error}");
            Assert.IsInstanceOfType(ex, typeof(FusedMultiplyAdd));
            Assert.IsTrue(Compiler.Compile("A + B * A", out ex, ref error), $"Unable to compile 'A * B + A'\r\n{error}");
            Assert.IsInstanceOfType(ex, typeof(FusedMultiplyAdd));
            Assert.IsTrue(Compiler.Compile("A * B + 4.0 * A + B * 1.2", out ex, ref error), $"Unable to compile 'A * B + A'\r\n{error}");
            Assert.IsInstanceOfType(ex, typeof(FusedMultiplyAdd));
            Assert.IsTrue(Compiler.Compile("A * B + (C * 2 + 3)", out ex, ref error), $"Unable to compile 'A * B + (C * 2 + 3)'\r\n{error}");
            Assert.IsInstanceOfType(ex, typeof(FusedMultiplyAdd));
            Assert.IsInstanceOfType(((FusedMultiplyAdd)ex).Add, typeof(FusedMultiplyAdd));
        }

        [TestMethod]
        public void TestOptimizeFusedMultiplyAdd()
        {
            CompareMatrix("A * B + A", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            }, 2.0f, 6.0f, 15.0f, 16.0f);
        }

        [TestMethod]
        public void TestOptimizeAddLiterals()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("1 + 2", out Expression ex, ref error), $"Unable to compile '1 + 2'\r\n{error}");
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(3.0f, result.LiteralValue);
            Assert.IsInstanceOfType(ex, typeof(Literal));
            Assert.AreEqual(3.0f, ((Literal)ex).Value);
        }

        [TestMethod]
        public void TestOptimizeSubtractLiterals()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("1 - 2", out Expression ex, ref error), $"Unable to compile '1 - 2'\r\n{error}");
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(-1.0f, result.LiteralValue);
            Assert.IsInstanceOfType(ex, typeof(Literal));
            Assert.AreEqual(-1.0f, ((Literal)ex).Value);
        }

        [TestMethod]
        public void TestOptimizeMultiplyLiterals()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("1 * 2", out Expression ex, ref error), $"Unable to compile '1 * 2'\r\n{error}");
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(2.0f, result.LiteralValue);
            Assert.IsInstanceOfType(ex, typeof(Literal));
            Assert.AreEqual(2.0f, ((Literal)ex).Value);
        }

        [TestMethod]
        public void TestOptimizeDivideLiterals()
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile("1 / 2", out Expression ex, ref error), $"Unable to compile '1 / 2'\r\n{error}");
            var result = ex.Evaluate(new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            });
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(0.5f, result.LiteralValue);
            Assert.IsInstanceOfType(ex, typeof(Literal));
            Assert.AreEqual(0.5f, ((Literal)ex).Value);
        }

        [TestMethod]
        public void TestOptimizeDivideToMultiply()
        {
            var data = new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("B", 1, 2, 4, 3)
            };
            Assert.IsInstanceOfType(CompareMatrix("A / 2", data, 0.5f, 1.0f, 1.5f, 2.0f), typeof(Multiply));
        }

        [TestMethod]
        public void TestIfValueValue()
        {
            CompareMatrix("ZeroMatrix(B) + if(1, 2, 3)", new[]
            {
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 2.0f, 2.0f, 2.0f, 2.0f);
            CompareMatrix("ZeroMatrix(B) + if(0, 2, 3)", new[]
            {
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 3.0f, 3.0f, 3.0f, 3.0f);
        }

        [TestMethod]
        public void TestIfValueVector()
        {
            CompareMatrix("ZeroMatrix(B) + asHorizontal(if(1, A, C))", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 1, 0, 1, 1),
                CreateData("C", -1, -2)
            }, 1.0f, 2.0f, 1.0f, 2.0f);
            CompareMatrix("ZeroMatrix(B) + AsHorizontal(if(0, A, C))", new IModule[]
            {
                CreateData("A", 1, 2),
                CreateMatrixData("B", 1, 0, 1, 1),
                CreateData("C", -1, -2)
            }, -1.0f, -2.0f, -1.0f, -2.0f);
        }

        [TestMethod]
        public void TestIfValueMatrix()
        {
            CompareMatrix("if(1, A, C)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("C", -1, -2, -3, -4)
            }, 1.0f, 2.0f, 3.0f, 4.0f);
            CompareMatrix("if(0, A, C)", new IModule[]
            {
                CreateMatrixData("A", 1, 2, 3, 4),
                CreateMatrixData("C", -1, -2, -3, -4)
            }, -1.0f, -2.0f, -3.0f, -4.0f);
        }

        [TestMethod]
        public void TestIfVectorValue()
        {
            CompareMatrix("ZeroMatrix(B) + asHorizontal(if(A, 4, 5))", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 1, 1),
            }, 4.0f, 5.0f, 4.0f, 5.0f);
        }

        [TestMethod]
        public void TestIfVectorVector()
        {
            CompareMatrix("ZeroMatrix(B) + asHorizontal(if(A, C, D))", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("B", 1, 0, 1, 1),
                CreateData("C", 4, 5),
                CreateData("D", -4, -5),
            }, 4.0f, -5.0f, 4.0f, -5.0f);
        }

        [TestMethod]
        public void TestIfVectorMatrixHorizontal()
        {
            CompareMatrix("if(asHorizontal(A), C, D)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("C", 4, 5, 6, 7),
                CreateMatrixData("D", -4, -5, -6, -7),
            }, 4.0f, -5.0f, 6.0f, -7.0f);
        }

        [TestMethod]
        public void TestIfVectorMatrixVertical()
        {
            CompareMatrix("if(asVertical(A), C, D)", new IModule[]
            {
                CreateData("A", 1, 0),
                CreateMatrixData("C", 4, 5, 6, 7),
                CreateMatrixData("D", -4, -5, -6, -7),
            }, 4.0f, 5.0f, -6.0f, -7.0f);
        }

        [TestMethod]
        public void TestIfMatrixMatrix()
        {
            CompareMatrix("if(B, ZeroMatrix(B)+2, ZeroMatrix(B) + (0-2))", new IModule[]
            {
                CreateMatrixData("B", 1, 0, 1, 1)
            }, 2.0f, -2.0f, 2.0f, 2.0f);
        }

        [TestMethod]
        public void TestLog()
        {
            CompareMatrix("Log(B)", new IModule[]
            {
                CreateMatrixData("B", 1, 0.5f, 0.25f, 0.125f)
            }, (float)Math.Log(1), (float)Math.Log(0.5f), (float)Math.Log(0.25f), (float)Math.Log(0.125f));
        }

        [TestMethod]
        public void TestSqrtMatrix()
        {
            CompareMatrix("sqrt(B)", new IModule[]
            {
                CreateMatrixData("B", 1, 0.5f, 0.25f, 0.125f)
            }, (float)Math.Sqrt(1), (float)Math.Sqrt(0.5f), (float)Math.Sqrt(0.25f), (float)Math.Sqrt(0.125f));
        }

        [TestMethod]
        public void TestSqrtVector()
        {
            CompareVector("sqrt(B)", new IModule[]
            {
                CreateVectorData("B", 1, 0.5f, 0.25f, 0.125f)
            }, (float)Math.Sqrt(1), (float)Math.Sqrt(0.5f), (float)Math.Sqrt(0.25f), (float)Math.Sqrt(0.125f));
        }

        [TestMethod]
        public void TestSqrtScalar()
        {
            CompareScalar("sqrt(B)", new IModule[]
            {
                CreateScalarData("B", 0.5f)
            }, (float)Math.Sqrt(0.5f));
        }

        [TestMethod]
        public void TestIfNaN()
        {
            CompareMatrix("IfNaN(B, C)", new IModule[]
            {
                CreateMatrixData("B", 1, float.NaN, float.NaN, 0.125f),
                CreateMatrixData("C", 1, 2, 3, 4)
            }, 1, 2, 3, 0.125f);
        }

        [TestMethod]
        public void TestNegate()
        {
            CompareMatrix("-B", new IModule[]
            {
                CreateMatrixData("B", 1, 0.5f, 0.25f, 0.125f)
            }, -1, -0.5f, -0.25f, -0.125f);
        }

        [TestMethod]
        public void TestNegateWithAdd()
        {
            CompareMatrix("-10 + B", new IModule[]
            {
                CreateMatrixData("B", 1, 2, 3, 4)
            }, -9, -8, -7, -6);
        }

        [TestMethod]
        public void TestNegateWithExp()
        {
            CompareMatrix("-B^-1", new IModule[]
            {
                CreateMatrixData("B", 1, 2, 4, 8)
            }, -1, -0.5f, -0.25f, -0.125f);
        }

        [TestMethod]
        public void TestNegateWithMul()
        {
            CompareMatrix("B*-1", new IModule[]
            {
                CreateMatrixData("B", 1, 2, 4, 8)
            }, -1, -2, -4, -8);
        }

        [TestMethod]
        public void TestNormalize()
        {
            CompareMatrix("normalize(B)", new IModule[]
            {
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 0.1f, 0.2f, 0.3f, 0.4f);
        }

        [TestMethod]
        public void TestNormalizeColumns()
        {
            CompareMatrix("normalizeColumns(B)", new IModule[]
            {
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2f / 8f, 4f / 12f, 6f / 8f, 8f / 12f);
        }

        [TestMethod]
        public void TestNormalizeRows()
        {
            CompareMatrix("normalizeRows(B)", new IModule[]
            {
                CreateMatrixData("B", 2, 4, 6, 8)
            }, 2f / 6f, 4f / 6f, 6f / 14f, 8f / 14f);
        }

        [TestMethod]
        public void TestSubtractionOrder()
        {
            string error = null;
            var equation = "2 + 3 - 1 + 2";
            Assert.IsTrue(Compiler.Compile(equation, out Expression ex, ref error), $"Unable to compile '{equation}'\r\n{error}");
            var result = ex.Evaluate(new IModule[0]);
            Assert.IsTrue(result.IsValue, "The result was not a scalar!");
            Assert.AreEqual(6.0f, result.LiteralValue, 0.000001f);
        }

        /// <summary>
        /// Assert results
        /// </summary>
        private static Expression CompareMatrix(string equation, IModule[] data, float m11, float m12, float m21, float m22)
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile(equation, out Expression ex, ref error), $"Unable to compile '{equation}'\r\n{error}");
            var result = ex.Evaluate(data);
            Assert.IsTrue(result.IsOdResult, result.ErrorMessage);
            var flat = result.OdData.Data;
            Assert.AreEqual(m11, flat[0], 0.00001f);
            Assert.AreEqual(m12, flat[1], 0.00001f);
            Assert.AreEqual(m21, flat[2], 0.00001f);
            Assert.AreEqual(m22, flat[3], 0.00001f);
            return ex;
        }

        /// <summary>
        /// Assert results
        /// </summary>
        private static Expression CompareVector(string equation, IModule[] data, float m11, float m12, float m21, float m22)
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile(equation, out Expression ex, ref error), $"Unable to compile '{equation}'\r\n{error}");
            var result = ex.Evaluate(data);
            if (result.Error)
            {
                Assert.Fail(result.ErrorMessage);
            }
            Assert.IsTrue(result.IsVectorResult);
            var flat = result.VectorData.Data;
            Assert.AreEqual(m11, flat[0], 0.00001f);
            Assert.AreEqual(m12, flat[1], 0.00001f);
            Assert.AreEqual(m21, flat[2], 0.00001f);
            Assert.AreEqual(m22, flat[3], 0.00001f);
            return ex;
        }

        /// <summary>
        /// Assert results
        /// </summary>
        private static Expression CompareScalar(string equation, IModule[] data, float m11)
        {
            string error = null;
            Assert.IsTrue(Compiler.Compile(equation, out Expression ex, ref error), $"Unable to compile '{equation}'\r\n{error}");
            var result = ex.Evaluate(data);
            if (result.Error)
            {
                Assert.Fail(result.ErrorMessage);
            }
            Assert.IsTrue(result.IsValue);
            Assert.AreEqual(m11, result.LiteralValue, 0.00001f);
            return ex;
        }

        class ScalarSource : BaseFunction<float>
        {
            readonly float _data;

            public ScalarSource(float data)
            {
                _data = data;
            }

            public override float Invoke()
            {
                return _data;
            }
        }

        class VectorSource : BaseFunction<Vector>
        {
            readonly Vector Data;

            public VectorSource(Vector vector)
            {
                Data = vector;
            }

            public override Vector Invoke()
            {
                return Data;
            }
        }

        class MatrixSource : BaseFunction<Matrix>
        {

            readonly Matrix Data;

            public MatrixSource(Matrix matrix)
            {
                Data = matrix;
            }

            public override Matrix Invoke()
            {
                return Data;
            }
        }
    }
}
