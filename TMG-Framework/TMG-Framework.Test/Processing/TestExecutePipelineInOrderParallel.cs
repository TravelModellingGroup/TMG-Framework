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
using System.Linq;
using TMG;
using TMG.Frameworks.Data.Processing.AST;
using TMG.Processing;
using TMG.Test.Utilities;
using System.Numerics;


namespace TMG.Test.Processing
{
    [TestClass]
    public class TestExecutePipelineInOrderParallel
    {
        [TestMethod]
        public void EnsureOrder()
        {
            ExecutePipelineInOrderParallel<int> module = new ExecutePipelineInOrderParallel<int>()
            {
                Name = "ModuleToTest",
                ToExecuteInParallel = Helper.CreateModule((int input) => input * 2),
                ToExecuteNotInParallel = new[]
                {
                    Helper.CreateModule((int j) => j + 1)
                }
            };
            var data = Enumerable.Range(0, 100).ToArray();
            var result = module.Invoke(data).ToArray();
            int i;
            for (i = 0; i < data.Length - Vector<float>.Count; i += Vector<float>.Count)
            {
                if (!System.Numerics.Vector.EqualsAll(new Vector<int>(data, i), new Vector<int>(result, i)))
                {
                    for (int j = 0; j < Vector<float>.Count; j++)
                    {
                        Assert.AreEqual((i + j) * 2 + 1, result[i + j]);
                    }
                }
            }
            for (; i < data.Length; i++)
            {
                Assert.AreEqual(i * 2 + 1, result[i]);
            }
        }
    }
}
