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

namespace TMG
{
    public sealed class Matrix
    {
        public Map Map { get; private set; }
        public float[] Data { get; private set; }

        private int RowLength;

        public Matrix(Map map)
        {
            Map = map;
            RowLength = Map.Count;
            Data = new float[Map.Count * Map.Count];
        }

        public Matrix(Vector vector)
        {
            Map = vector.Map;
            RowLength = Map.Count;
            Data = new float[Map.Count * Map.Count];
        }

        public Matrix(Matrix matrix)
        {
            Map = matrix.Map;
            RowLength = matrix.RowLength;
            Data = new float[Map.Count * Map.Count];
        }

        public (int Row, int Column) GetSparseIndex(int flatIndex)
        {
            return (flatIndex / RowLength, flatIndex % RowLength);
        }

        /// <summary>
        /// Get the index in data given the flat row and columns
        /// </summary>
        /// <param name="flatRow">The row to lookup</param>
        /// <param name="floatColumn">The column to lookup</param>
        /// <returns>The index in data for this data.</returns>
        public int GetFlatIndex(int flatRow, int floatColumn)
        {
            return RowLength * flatRow + floatColumn;
        }

        /// <summary>
        /// Get the index in the data for the starting point of a given row
        /// </summary>
        /// <param name="flatRow">The flat index of the row to get</param>
        /// <returns></returns>
        public int GetFlatRowIndex(int flatRow)
        {
            return RowLength * flatRow;
        }

        /// <summary>
        /// Get the index int he data for the starting point given a sparse row
        /// </summary>
        /// <param name="sparseRow">The sparse row index to lookup</param>
        /// <returns>The index in data for the start of this row</returns>
        public int GetSparseRowIndex(int sparseRow)
        {
            var index = Map.GetFlatIndex(sparseRow);
            if(index < 0)
            {
                return -1;
            }
            return index * RowLength;
        }
    }
}
