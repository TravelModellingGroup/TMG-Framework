/*
    Copyright 2014-2017 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TMG.Data.Testing
{
    /// <summary>
    ///This is a test class for RangeSetSeriesTest and is intended
    ///to contain all RangeSetSeriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RangeSetSeriesTest
    {
        /// <summary>
        ///A test for RangeSetSeries Constructor
        ///</summary>
        [TestMethod()]
        public void RangeSetSeriesConstructorTest()
        {
            List<RangeSet> tempRange = GenerateTempRange();
            RangeSetSet target = new RangeSetSet(tempRange);
            Assert.AreEqual(target.Count, 2);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            List<RangeSet> tempRange = GenerateTempRange();
            RangeSetSet target = new RangeSetSet(tempRange);
            string expected = "{1-2,4-5},{11-12,14-15}";
            var actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryParse
        ///</summary>
        [TestMethod()]
        public void TryParseTestFail()
        {
            string error = null;
            string rangeString = "{1-2,4-5},{11-12,14-15";
            bool actual = RangeSetSet.TryParse(ref error, rangeString, out RangeSetSet output);
            Assert.IsNotNull(error);
            Assert.AreEqual(null, output);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for TryParse
        ///</summary>
        [TestMethod()]
        public void TryParseTestNonError()
        {
            string rangeString = "{1-2,4-5},{11-12,14-15}";
            RangeSetSet outputExpected = new RangeSetSet(GenerateTempRange());
            var actual = RangeSetSet.TryParse(rangeString, out RangeSetSet output);
            Assert.AreEqual(outputExpected, output);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for TryParse
        ///</summary>
        [TestMethod()]
        public void TryParseTestSuccess()
        {
            string error = null;
            string rangeString = "{1-2,4-5},{11-12,14-15}";
            RangeSetSet outputExpected = new RangeSetSet(GenerateTempRange());
            var actual = RangeSetSet.TryParse(ref error, rangeString, out RangeSetSet output);
            Assert.AreEqual(null, error);
            Assert.AreEqual(outputExpected, output);
            Assert.AreEqual(true, actual);
        }

        private static List<RangeSet> GenerateTempRange()
        {
            List<RangeSet> tempRange = new List<RangeSet>()
            {
                new RangeSet(new List<Range>()
                {
                    new Range(1, 2),
                    new Range(4, 5)
                }),
                new RangeSet(new List<Range>()
                {
                    new Range(11, 12),
                    new Range(14, 15)
                })
            };
            return tempRange;
        }
    }
}