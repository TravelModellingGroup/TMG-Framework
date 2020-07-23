/*
    Copyright 2017-2019 University of Toronto

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
using System.Runtime.CompilerServices;
using System.Text;
using static TMG.Utilities.ExceptionHelper;

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
        public Categories RowCategories { get; }

        /// <summary>
        /// The categories for the columns.
        /// </summary>
        public Categories ColumnCategories { get; }

        /// <summary>
        /// The backend storage for the matrix
        /// </summary>
        public float[] Data { get; }

        /// <summary>
        /// Used as a quick lookup for the number of columns per row.
        /// </summary>
        private readonly int _rowSpan;

        /// <summary>
        /// Create a new matrix with the given row and column categories.
        /// </summary>
        /// <param name="rowCategories">The categories for the rows.</param>
        /// <param name="columnCategories">The categories for the columns.</param>
        public Matrix(Categories rowCategories, Categories columnCategories)
        {
            RowCategories = rowCategories ?? throw new ArgumentNullException(nameof(rowCategories));
            ColumnCategories = columnCategories ?? throw new ArgumentNullException(nameof(columnCategories));
            _rowSpan = ColumnCategories.Count;
            Data = new float[RowCategories.Count * ColumnCategories.Count];
        }

        /// <summary>
        /// Create a new matrix using the dimensions from the given vector.
        /// </summary>
        /// <param name="vector">The vector to get the dimensions from.</param>
        public Matrix(Vector vector)
        {
            if (vector == null)
            {
                ThrowParameterNull(nameof(vector));
            }
            ColumnCategories = RowCategories = vector.Categories;
            _rowSpan = ColumnCategories.Count;
            Data = new float[RowCategories.Count * ColumnCategories.Count];
        }

        /// <summary>
        /// Create a new matrix with the dimensions from the provided
        /// matrix.
        /// </summary>
        /// <param name="matrix">The matrix to copy the dimensions from.</param>
        public Matrix(Matrix matrix)
        {
            if (matrix == null)
            {
                ThrowParameterNull(nameof(matrix));
            }
            RowCategories = matrix.RowCategories;
            ColumnCategories = matrix.ColumnCategories;
            _rowSpan = matrix._rowSpan;
            Data = new float[RowCategories.Count * ColumnCategories.Count];
        }

        /// <summary>
        /// Get the row and column for a given flat index into the backend data
        /// </summary>
        /// <param name="flatIndex">The flat index in the data to get the sparse row and column for.</param>
        /// <returns>The row and column in sparse space for this flat index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public (CategoryIndex Row, CategoryIndex Column) GetSparseIndex(int flatIndex)
        {
            return (RowCategories.GetSparseIndex(flatIndex / _rowSpan), RowCategories.GetSparseIndex(flatIndex % _rowSpan));
        }

        /// <summary>
        /// Get the index in data given the flat row and columns
        /// </summary>
        /// <param name="flatRow">The row to lookup</param>
        /// <param name="flatColumn">The column to lookup</param>
        /// <returns>The index in data for this data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int GetFlatIndex(int flatRow, int flatColumn)
        {
            return _rowSpan * flatRow + flatColumn;
        }

        /// <summary>
        /// Get the index in the data for the starting point of a given row
        /// </summary>
        /// <param name="flatRow">The flat index of the row to get</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int GetFlatRowIndex(int flatRow)
        {
            return _rowSpan * flatRow;
        }

        /// <summary>
        /// Get the index int he data for the starting point given a sparse row
        /// </summary>
        /// <param name="sparseRow">The sparse row index to lookup</param>
        /// <returns>The index in data for the start of this row</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int GetSparseRowIndex(CategoryIndex sparseRow)
        {
            var index = RowCategories.GetFlatIndex(sparseRow);
            return index >= 0 ? index * _rowSpan : -1;
        }

        /// <summary>
        /// Get a reference to a row in the matrix.
        /// </summary>
        /// <param name="flatRowIndex">The 0 indexed row number to get access to.</param>
        /// <returns>A reference to the row.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Span<float> GetRow(int flatRowIndex)
        {
            if ((flatRowIndex < 0) | (flatRowIndex > RowCategories.Count))
            {
                ThrowOutOfRangeException(nameof(flatRowIndex));
            }
            flatRowIndex = GetFlatRowIndex(flatRowIndex);
            return new Span<float>(Data, flatRowIndex, _rowSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ref float GetFromSparseIndexes(int rowIndex, int columnIndex)
        {
            var o = RowCategories.GetFlatIndex(rowIndex);
            var d = ColumnCategories.GetFlatIndex(columnIndex);
            if(o < 0)
            {
                InvalidRow(rowIndex);
            }
            if(d < 0)
            {
                InvalidColumns(columnIndex);
            }
            return ref Data[GetFlatIndex(o,d)];
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InvalidColumns(int columnIndex)
        {
            throw new ArgumentOutOfRangeException($"Invalid column index {columnIndex}!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InvalidRow(int rowIndex)
        {
            throw new ArgumentOutOfRangeException($"Invalid row index {rowIndex}!");
        }
    }
}
