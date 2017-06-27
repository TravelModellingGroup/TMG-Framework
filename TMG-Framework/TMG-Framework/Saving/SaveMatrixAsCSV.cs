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
using System.IO;
using System.Text;
using XTMF2;

namespace TMG.Saving
{
    [Module(Name = "Save Matrix To CSV", Description = "Saves a matrix to the given write stream.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class SaveMatrixAsCSV : BaseAction<(Matrix Matrix, WriteStream Stream)>
    {
        [Parameter(Name = "Third Normalized", DefaultValue = "True", Index = 0,
            Description = "Should the data be saved in third normalized form?  If not it will be saved as a CSV Matrix")]
        public IFunction<bool> ThirdNormalized;

        [Parameter(Name = "First Index Header", DefaultValue = "Origin", Index = 1,
            Description = "The name of the first index.")]
        public IFunction<string> FirstIndexHeader;

        [Parameter(Name = "Second Index Header", DefaultValue = "Destination", Index = 2,
            Description = "The name of the second index.")]
        public IFunction<string> SecondIndexHeader;

        [Parameter(Name = "Data Index Header", DefaultValue = "Data", Index = 3,
            Description = "The name of the Data index.")]
        public IFunction<string> DataIndexHeader;

        public override void Invoke((Matrix Matrix, WriteStream Stream) context)
        {
            if(ThirdNormalized.Invoke())
            {
                WriteThirdNormalized(context);
            }
            else
            {
                WriteCSVMatrix(context);
            }
        }

        private void WriteCSVMatrix((Matrix Matrix, WriteStream Stream) context)
        {
            using (StreamWriter writer = new StreamWriter(context.Stream))
            {
                var matrix = context.Matrix;
                var map = matrix.RowCategories;
                var length = map.Count;
                writer.Write('"');
                writer.Write(FirstIndexHeader.Invoke().Replace('"', '\''));
                writer.Write('\\');
                writer.Write(SecondIndexHeader.Invoke().Replace('"', '\''));
                writer.Write('"');
                for (int i = 0; i < length; i++)
                {
                    writer.Write(',');
                    writer.Write(map.GetSparseIndex(i));
                }
                writer.WriteLine();
                var data = matrix.Data;
                int pos = 0;
                for (int i = 0; i < length; i++)
                {
                    writer.Write(map.GetSparseIndex(i));
                    for (int j = 0; j < length; j++)
                    {
                        writer.Write(',');
                        writer.Write(data[pos]);
                    }
                    writer.WriteLine();
                }
            }
        }

        private static void WriteWithQuotes(StreamWriter writer, string name)
        {
            writer.Write('"');
            writer.Write(name.Replace('"', '\''));
            writer.Write('"');
        }

        private void WriteThirdNormalized((Matrix Matrix, WriteStream Stream) context)
        {
            using (StreamWriter writer = new StreamWriter(context.Stream))
            {
                var matrix = context.Matrix;
                var map = matrix.RowCategories;
                var length = map.Count;
                var data = matrix.Data;
                WriteWithQuotes(writer, FirstIndexHeader.Invoke());
                writer.Write(',');
                WriteWithQuotes(writer, SecondIndexHeader.Invoke());
                writer.Write(',');
                WriteWithQuotes(writer, DataIndexHeader.Invoke());
                writer.WriteLine();
                for (int i = 0; i < data.Length; i++)
                {
                    //ignore zeroed out data to save disk space
                    if(data[i] != 0.0)
                    {
                        var pos = matrix.GetSparseIndex(i);
                        writer.Write(pos.Row);
                        writer.Write(',');
                        writer.Write(pos.Column);
                        writer.Write(',');
                        writer.WriteLine(data[i]);
                    }
                }
            }
        }
    }
}
