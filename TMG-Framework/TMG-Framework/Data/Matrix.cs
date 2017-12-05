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
    /// <summary>
    /// A 2D representation with categories for rows and columns
    /// </summary>
    public sealed class Matrix
    {
        /// <summary>
        /// The categories for the rows.
        /// </summary>
        public Categories RowCategories { get; private set; }

        /// <summary>
        /// The categories for the columns.
        /// </summary>
        public Categories ColumnCategories { get; private set; }

        /// <summary>
        /// The backend storage for the matrix
        /// </summary>
        public float[] Data { get; private set; }

        private int _RowSpan;

        /// <summary>
        /// Create a new matrix with the given row and column categories.
        /// </summary>
        /// <param name="rowCategories">The categories for the rows.</param>
        /// <param name="columnCategories">The categories for the columns.</param>
        public Matrix(Categories rowCategories, Categories columnCategories)
        {
            RowCategories = rowCategories;
            ColumnCategories = columnCategories;
            _RowSpan = ColumnCategories.Count;
            Data = new float[RowCategories.Count * ColumnCategories.Count];
        }

        /// <summary>
        /// Create a new matrix using the dimensions from the given vector.
        /// </summary>
        /// <param name="vector">The vector to get the dimensions from.</param>
        public Matrix(Vector vector)
        {
            ColumnCategories = RowCategories = vector.Categories;
            _RowSpan = ColumnCategories.Count;
            Data = new float[RowCategories.Count * ColumnCategories.Count];
        }

        /// <summary>
        /// Create a new matrix with the dimensions from the provided
        /// matrix.
        /// </summary>
        /// <param name="matrix">The matrix to copy the dimensions from.</param>
        public Matrix(Matrix matrix)
        {
            RowCategories = matrix.RowCategories;
            ColumnCategories = matrix.ColumnCategories;
            _RowSpan = matrix._RowSpan;
            Data = new float[RowCategories.Count * ColumnCategories.Count];
        }

        /// <summary>
        /// Get the row and column for a given flat index into the backend data
        /// </summary>
        /// <param name="flatIndex">The flat index in the data to get the sparse row and column for.</param>
        /// <returns>The row and column in sparse space for this flat index.</returns>
        public (int Row, int Column) GetSparseIndex(int flatIndex)
        {
            return (RowCategories.GetSparseIndex(flatIndex / _RowSpan), RowCategories.GetSparseIndex(flatIndex % _RowSpan));
        }

        /// <summary>
        /// Get the index in data given the flat row and columns
        /// </summary>
        /// <param name="flatRow">The row to lookup</param>
        /// <param name="floatColumn">The column to lookup</param>
        /// <returns>The index in data for this data.</returns>
        public int GetFlatIndex(int flatRow, int floatColumn)
        {
            return _RowSpan * flatRow + floatColumn;
        }

        /// <summary>
        /// Get the index in the data for the starting point of a given row
        /// </summary>
        /// <param name="flatRow">The flat index of the row to get</param>
        /// <returns></returns>
        public int GetFlatRowIndex(int flatRow)
        {
            return _RowSpan * flatRow;
        }

        /// <summary>
        /// Get the index int he data for the starting point given a sparse row
        /// </summary>
        /// <param name="sparseRow">The sparse row index to lookup</param>
        /// <returns>The index in data for the start of this row</returns>
        public int GetSparseRowIndex(int sparseRow)
        {
            var index = RowCategories.GetFlatIndex(sparseRow);
            if(index < 0)
            {
                return -1;
            }
            return index * _RowSpan;
        }

        /// <summary>
        /// Get a reference to a row in the matrix.
        /// </summary>
        /// <param name="flatRowIndex">The 0 indexed row number to get access to.</param>
        /// <returns>A reference to the row.</returns>
        public Span<float> GetRow(int flatRowIndex)
        {
            if(flatRowIndex < 0 | flatRowIndex + _RowSpan > RowCategories.Count)
            {
                throw new IndexOutOfRangeException(nameof(flatRowIndex));
            }
            return new Span<float>(Data, GetFlatRowIndex(flatRowIndex), _RowSpan);
        }
    }
}
