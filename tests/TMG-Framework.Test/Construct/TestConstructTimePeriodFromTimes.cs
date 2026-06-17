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

namespace TMG.Test.Construct
{
    [TestClass]
    public class TestConstructTimePeriodFromTimes
    {
        [TestMethod]
        public void ConstructTimePeriod()
        {
            var time1 = Time.FromMinutes(60f);
            var time2 = Time.FromMinutes(120f);
            var constructor = new TMG.Construct.ConstructTimePeriodFromTimes()
            {
                Name = "Construct Time Period",
                StartTime = Helper.CreateParameter(time1, "StartTime"),
                EndTime = Helper.CreateParameter(time2, "EndTime"),
            };
            var interval = constructor.Invoke();
            Assert.AreEqual(time1, interval.Start);
            Assert.AreEqual(time2, interval.End);
        }
    }
}
