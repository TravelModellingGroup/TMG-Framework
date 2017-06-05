﻿/*
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

namespace TMG.Frameworks.Data.Processing.AST
{
    public static class Compiler
    {
        public static bool Compile(string expression,out Expression ex, ref string error)
        {
            var buffer = expression.ToCharArray();
            return Expression.Compile(buffer, 0, buffer.Length, out ex, ref error) && Expression.Optimize(ref ex, ref error);
        }
    }
}