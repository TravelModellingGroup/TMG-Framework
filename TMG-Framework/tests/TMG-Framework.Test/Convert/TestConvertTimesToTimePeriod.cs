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
using TMG.Convert;
using TMG.Frameworks.Data.Processing.AST;
using TMG.Processing;
using TMG.Select;
using TMG.Test.Utilities;

namespace TMG.Test.Convert
{
    [TestClass]
    public class TestConstructTimePeriodFromTimes
    {
        [TestMethod]
        public void ConverTimesToTimePeriod()
        {
            var time1 = Time.FromMinutes(60f);
            var time2 = Time.FromMinutes(120f);
            var time3 = Time.FromMinutes(1500f);
            var converter = new ConvertTimesToTimePeriod()
            {
                Name = "Converter"
            };
            var interval1 = converter.Invoke((time1, time2));
            var interval2 = converter.Invoke((time2, time3));
            Assert.AreEqual(time1, interval1.Start);
            Assert.AreEqual(time2, interval2.Start);
            Assert.AreEqual(time2, interval1.End);
            Assert.AreEqual(time3, interval2.End);
        }
    }
}
