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
using System.Text;

namespace TMG.Data
{
    public sealed class SparseMatrix
    {
        public SparseMap Map { get; private set; }
        public float[] Data { get; private set; }

        private int RowLength;

        public SparseMatrix(SparseMap map)
        {
            Map = map;
            RowLength = Map.Count;
            Data = new float[Map.Count * Map.Count];
        }

        public (int Row, int Column) GetSparseIndex(int flatIndex)
        {
            return (flatIndex / RowLength, flatIndex % RowLength);
        }

        public int GetFlatIndex(int flatRow, int floatColumn)
        {
            return RowLength * flatRow + floatColumn;
        }
    }
}
