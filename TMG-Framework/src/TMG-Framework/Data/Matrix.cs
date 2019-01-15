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
        public Categories RowCategories => _rowCategories;
        private readonly Categories _rowCategories;

        /// <summary>
        /// The categories for the columns.
        /// </summary>
        public Categories ColumnCategories => _columnCategories;
        private readonly Categories _columnCategories;

        /// <summary>
        /// The backend storage for the matrix
        /// </summary>
        public float[] Data => _data;
        private readonly float[] _data;

        private int _rowSpan;

        /// <summary>
        /// Create a new matrix with the given row and column categories.
        /// </summary>
        /// <param name="rowCategories">The categories for the rows.</param>
        /// <param name="columnCategories">The categories for the columns.</param>
        public Matrix(Categories rowCategories, Categories columnCategories)
        {
            _rowCategories = rowCategories;
            _columnCategories = columnCategories;
            _rowSpan = ColumnCategories.Count;
            _data = new float[_rowCategories.Count * _columnCategories.Count];
        }

        /// <summary>
        /// Create a new matrix using the dimensions from the given vector.
        /// </summary>
        /// <param name="vector">The vector to get the dimensions from.</param>
        public Matrix(Vector vector)
        {
            _columnCategories = _rowCategories = vector.Categories;
            _rowSpan = ColumnCategories.Count;
            _data = new float[_rowCategories.Count * _columnCategories.Count];
        }

        /// <summary>
        /// Create a new matrix with the dimensions from the provided
        /// matrix.
        /// </summary>
        /// <param name="matrix">The matrix to copy the dimensions from.</param>
        public Matrix(Matrix matrix)
        {
            _rowCategories = matrix.RowCategories;
            _columnCategories = matrix.ColumnCategories;
            _rowSpan = matrix._rowSpan;
            _data = new float[_rowCategories.Count * _columnCategories.Count];
        }

        /// <summary>
        /// Get the row and column for a given flat index into the backend data
        /// </summary>
        /// <param name="flatIndex">The flat index in the data to get the sparse row and column for.</param>
        /// <returns>The row and column in sparse space for this flat index.</returns>
        public (CategoryIndex Row, CategoryIndex Column) GetSparseIndex(int flatIndex)
        {
            return (_rowCategories.GetSparseIndex(flatIndex / _rowSpan), _rowCategories.GetSparseIndex(flatIndex % _rowSpan));
        }

        /// <summary>
        /// Get the index in data given the flat row and columns
        /// </summary>
        /// <param name="flatRow">The row to lookup</param>
        /// <param name="floatColumn">The column to lookup</param>
        /// <returns>The index in data for this data.</returns>
        public int GetFlatIndex(int flatRow, int floatColumn)
        {
            return _rowSpan * flatRow + floatColumn;
        }

        /// <summary>
        /// Get the index in the data for the starting point of a given row
        /// </summary>
        /// <param name="flatRow">The flat index of the row to get</param>
        /// <returns></returns>
        public int GetFlatRowIndex(int flatRow)
        {
            return _rowSpan * flatRow;
        }

        /// <summary>
        /// Get the index int he data for the starting point given a sparse row
        /// </summary>
        /// <param name="sparseRow">The sparse row index to lookup</param>
        /// <returns>The index in data for the start of this row</returns>
        public int GetSparseRowIndex(CategoryIndex sparseRow)
        {
            var index = _rowCategories.GetFlatIndex(sparseRow);
            if(index < 0)
            {
                return -1;
            }
            return index * _rowSpan;
        }

        /// <summary>
        /// Get a reference to a row in the matrix.
        /// </summary>
        /// <param name="flatRowIndex">The 0 indexed row number to get access to.</param>
        /// <returns>A reference to the row.</returns>
        public Span<float> GetRow(int flatRowIndex)
        {
            if(flatRowIndex < 0 | flatRowIndex + _rowSpan > _rowCategories.Count)
            {
                throw new IndexOutOfRangeException(nameof(flatRowIndex));
            }
            return new Span<float>(_data, GetFlatRowIndex(flatRowIndex), _rowSpan);
        }
    }
}
