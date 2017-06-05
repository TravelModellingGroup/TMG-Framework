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
    [Module(Name = "Save SparseVector To CSV", Description = "Saves a sparse vector to the given write stream.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class SaveSparseVectorToCSV : BaseAction<(WriteStream Stream, SparseVector Data)>
    {
        public override void Invoke((WriteStream Stream, SparseVector Data) context)
        {
            using (var writer = new StreamWriter(context.Stream))
            {
                var data = context.Data;
                var map = data.Map;
                WriteHeaders(writer);
                var length = map.Count;
                for (int i = 0; i < length; i++)
                {
                    writer.Write(map.GetSparseIndex(i));
                    writer.Write(',');
                    writer.WriteLine(data[i]);
                }
            }
        }

        [Parameter(Name = "Map Column Name", DefaultValue = "Zone", Description = "The header to give to the map indexes", Index = 0)]
        public IFunction<string> MapColumnName;

        [Parameter(Name = "Data Column Name", DefaultValue = "Data", Description = "The header to give to the data", Index = 1)]
        public IFunction<string> DataColumnName;

        private void WriteHeaders(StreamWriter writer)
        {
            writer.Write('\"');
            writer.Write(MapColumnName.Invoke().Replace('\"', '\''));
            writer.Write('\"');
            writer.Write(',');
            writer.Write('\"');
            writer.Write(DataColumnName.Invoke());
            writer.WriteLine('\"');
        }
    }
}
