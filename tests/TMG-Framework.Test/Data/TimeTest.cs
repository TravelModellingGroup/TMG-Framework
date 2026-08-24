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



namespace TMG.Test.Data;

[TestClass]
public class TimeTest
{
    [TestMethod]
    public void IntersectionBool_ReturnsTrue_WhenIntervalsOverlap()
    {
        var start1 = At(8, 0);
        var end1 = At(9, 0);
        var start2 = At(8, 30);
        var end2 = At(10, 0);

        var hasIntersection = Time.Intersection(start1, end1, start2, end2);

        Assert.IsTrue(hasIntersection);
    }

    [TestMethod]
    public void IntersectionDuration_ReturnsExpectedDuration_WhenIntervalsOverlap()
    {
        var start1 = At(8, 0);
        var end1 = At(9, 0);
        var start2 = At(8, 30);
        var end2 = At(10, 0);

        var hasIntersection = Time.Intersection(start1, end1, start2, end2, out var duration);

        Assert.IsTrue(hasIntersection);
        Assert.AreEqual(At(0, 30), duration);
    }

    [TestMethod]
    public void IntersectionDuration_ReturnsZeroDuration_WhenIntervalsTouch()
    {
        var start1 = At(8, 0);
        var end1 = At(9, 0);
        var start2 = At(9, 0);
        var end2 = At(10, 0);

        var hasIntersection = Time.Intersection(start1, end1, start2, end2, out var duration);

        Assert.IsTrue(hasIntersection);
        Assert.AreEqual(Time.Zero, duration);
    }

    [TestMethod]
    public void IntersectionBounds_ReturnsExpectedStartAndEnd_WhenIntervalsOverlap()
    {
        var start1 = At(8, 0);
        var end1 = At(9, 0);
        var start2 = At(8, 30);
        var end2 = At(10, 0);

        var hasIntersection = Time.Intersection(start1, end1, start2, end2, out var intersectionStart, out var intersectionEnd);

        Assert.IsTrue(hasIntersection);
        Assert.AreEqual(At(8, 30), intersectionStart);
        Assert.AreEqual(At(9, 0), intersectionEnd);
    }

    [TestMethod]
    public void IntersectionOutputs_ReturnFalseAndZeroValues_WhenNoOverlap()
    {
        var start1 = At(8, 0);
        var end1 = At(8, 30);
        var start2 = At(9, 0);
        var end2 = At(10, 0);

        var hasDurationIntersection = Time.Intersection(start1, end1, start2, end2, out var duration);
        var hasBoundIntersection = Time.Intersection(start1, end1, start2, end2, out var intersectionStart, out var intersectionEnd);

        Assert.IsFalse(hasDurationIntersection);
        Assert.AreEqual(Time.Zero, duration);
        Assert.IsFalse(hasBoundIntersection);
        Assert.AreEqual(Time.Zero, intersectionStart);
        Assert.AreEqual(Time.Zero, intersectionEnd);
    }

    private static Time At(int hours, int minutes, int seconds = 0)
    {
        return new Time()
        {
            Hours = hours,
            Minutes = minutes,
            Seconds = seconds
        };
    }
}
