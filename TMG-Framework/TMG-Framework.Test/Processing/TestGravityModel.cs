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
using System.Text;
using System.Threading.Tasks;
using TMG.Processing;
using TMG.Test.Utilities;

namespace TMG.Test.Processing
{
    [TestClass]
    public class TestGravityModel
    {
        [TestMethod]
        public void Gravity2D()
        {
            var map = MapHelper.LoadMap(MapHelper.WriteCSV(2));
            var friction = new Matrix(map);
            friction.Data[0] = 0.25f;
            friction.Data[1] = 0.75f;
            friction.Data[2] = 2f;
            friction.Data[3] = 2f;
            var production = new Vector(map);
            production.Data[0] = 2;
            production.Data[1] = 2;
            var attraction = new Vector(map);
            attraction.Data[0] = 1.5f;
            attraction.Data[1] = 2.5f;
            var gm = new Evaluate2DGravityModel()
            {
                Attraction = Helper.CreateParameter(attraction),
                Production = Helper.CreateParameter(production),
                Friction = Helper.CreateParameter(friction),
                MaxError = Helper.CreateParameter(0.25f),
                MaxIterations = Helper.CreateParameter(100)
            };
            var ret = gm.Invoke();
            var result = ret.Data;
            Assert.AreSame(map, ret.Map);
            Assert.AreEqual(0.5f, result[0], 0.0001f);
            Assert.AreEqual(1.5f, result[1], 0.0001f);
            Assert.AreEqual(1f, result[2], 0.0001f);
            Assert.AreEqual(1f, result[3], 0.0001f);
        }
    }
}
