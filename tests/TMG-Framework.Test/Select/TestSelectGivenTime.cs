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
using TMG.Select;
using TMG.Test.Utilities;

namespace TMG.Test.Select
{
    [TestClass]
    public class TestSelectGivenTime
    {
        [TestMethod]
        public void SelectDataGivenTime()
        {
            var selector = new SelectGivenTime<int>()
            {
                Name = "Selection Module",
                DefaultTimePeriod = Helper.CreateParameter(0, "Default Value"),
                TimePeriodData = new []
                {
                    Helper.CreateParameter((new TimePeriod(Time.FromMinutes(60), Time.FromMinutes(120)), 1), "First"),
                    Helper.CreateParameter((new TimePeriod(Time.FromMinutes(120), Time.FromMinutes(240)), 2), "Second"),
                    Helper.CreateParameter((new TimePeriod(Time.FromMinutes(250), Time.FromMinutes(360)), 3), "Third")
                }
            };
            Assert.AreEqual(1, selector.Invoke(Time.FromMinutes(60.0f)));
            Assert.AreEqual(2, selector.Invoke(Time.FromMinutes(120.0f)));
            Assert.AreEqual(1, selector.Invoke(Time.FromMinutes(240.0f)));
            Assert.AreEqual(1, selector.Invoke(Time.FromMinutes(360.0f)));
            Assert.AreEqual(3, selector.Invoke(Time.FromMinutes(250.0f)));
            Assert.AreEqual(3, selector.Invoke(Time.FromMinutes(270.0f)));
        }
    }
}
