/*
    Copyright 2017 University of Toronto

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TMG;
using TMG.Frameworks.Data.Processing.AST;
using TMG.Processing;
using TMG.Test.Utilities;

namespace TMG.Test.Processing
{
    [TestClass]
    public class TestEvaluateMatrix
    {
        [TestMethod]
        public void LargeMatrixMathFMA()
        {
            var map = MapHelper.LoadMap(MapHelper.WriteCSV(4000));
            var a = new Matrix(map);
            var b = new Matrix(map);
            var c = new Matrix(map);
            var eval = new EvaluateMatrix()
            {
                Expression = Helper.CreateParameter("A * B + (C * 2 + 3)"),
                Variables = new[]
                {
                    Helper.CreateParameter(a, "A"),
                    Helper.CreateParameter(b, "B"),
                    Helper.CreateParameter(c, "C"),
                }
            };
            string error = null;
            Assert.IsTrue(eval.RuntimeValidation(ref error), error);
            for (int i = 0; i < 200; i++)
            {
                var res = eval.Invoke();
                Assert.IsNotNull(res);
            }
        }
    }
}
