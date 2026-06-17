/*
    Copyright 2020 University of Toronto

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
using System.IO;
using System.Text;
using TMG.Utilities;
using XTMF2;

namespace TMG.Processing
{
    [Module(
        Name = "Append Matrix Value To CSV",
        Description = "Reading in a CSV it will append a new column at the end containing the result of looking up an row and column value from a matrix.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class AppendMatrixValueToCSV : BaseAction
    {

        [Parameter(Name = "Column Name", DefaultValue = "Value", Description = "The name to use for the column", Index = 0)]
        public IFunction<string> ColumnName;

        [Parameter(Name = "Row Index", DefaultValue = "1", Description = "The 0 indexed column containing the sparse map index for the row.", Index = 1)]
        public IFunction<int> RowIndex;

        [Parameter(Name = "Column Index", DefaultValue = "2", Description = "The 0 indexed column containing the sparse map index for the column.", Index = 2)]
        public IFunction<int> ColumnIndex;

        [SubModule(Name = "Matrix", Index = 3, Description = "The matrix to assign.", Required = true)]
        public IFunction<Matrix> Matrix;

        [SubModule(Name = "Input Stream", Index = 4, Description = "The stream contianing the CSV file.", Required = true)]
        public IFunction<ReadStream> InputStream;

        [SubModule(Name = "Output Stream", Index = 5, Description = "The stream to store the results.", Required = true)]
        public IFunction<WriteStream> OutputStream;

        public override void Invoke()
        {
            var matrix = Matrix!.Invoke();
            var columnName = ColumnName?.Invoke();
            // name sure we don't just have a blank header
            columnName = String.IsNullOrEmpty(columnName) ? "Value" : columnName;
            using var reader = new CsvReader(InputStream!.Invoke(), false);
            using var writer = new StreamWriter(OutputStream!.Invoke());
            int rowIndex = RowIndex!.Invoke();
            int columnIndex = ColumnIndex!.Invoke();
            int minimumRowSize = Math.Max(rowIndex, columnIndex) + 1;
            // Process header
            var headers = reader.Headers;
            for (int i = 0; i < headers.Length; i++)
            {
                writer.Write(headers[i]);
                // it is safe to just have a comma after since we are adding a new column
                writer.Write(',');
            }
            writer.Write(columnName);
            writer.WriteLine();
            // Process main body
            while(reader.LoadLine(out var columns))
            {
                if (columns >= minimumRowSize)
                {
                    // copy all of the old values
                    for (int i = 0; i < columns; i++)
                    {
                        reader.Get(out string value, i);
                        writer.Write(value);
                        writer.Write(',');
                    }
                    reader.Get(out int o, rowIndex);
                    reader.Get(out int d, columnIndex);
                    writer.WriteLine(matrix.GetFromSparseIndexes(o, d));
                }
            }
        }
    }
}
