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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMG;
using TMG.Processing;

namespace PerformanceTest
{
    class Program
    {
        static void PerfTest(string runName, Action toRun, int times)
        {
            Console.WriteLine($"Evaluating {runName}");
            List<long> runtime = new List<long>();
            Stopwatch watch = new Stopwatch();
            for (int i = 0; i < times; i++)
            {
                watch.Restart();
                toRun();
                watch.Stop();
                runtime.Add(watch.ElapsedTicks);
            }
            Console.WriteLine($"Total Time: {runtime.Sum() / (float)TimeSpan.TicksPerMillisecond} ms");
            Console.WriteLine($"Avg Time: {runtime.Average() / (float)TimeSpan.TicksPerMillisecond} ms");
        }

        static void Main(string[] args)
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
            eval.RuntimeValidation(ref error);
            PerfTest("Matrix Computation", () =>
           {
               var res = eval.Invoke();
           }, 200);
        }
    }
}