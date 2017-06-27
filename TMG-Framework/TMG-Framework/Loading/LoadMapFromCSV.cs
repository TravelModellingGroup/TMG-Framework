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
using TMG.Utilities;
using XTMF2;

namespace TMG.Loading
{
    [Module(Name = "Load Map From CSV", Description = "Loads a map where each row has a different sparse index.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class LoadMapFromCSV : BaseFunction<ReadStream, Categories>
    {
        public override Categories Invoke(ReadStream stream)
        {
            var record = new List<int>();
            using(var reader = new CsvReader(stream, false))
            {
                reader.LoadLine();
                while(reader.LoadLine(out var columns))
                {
                    if(columns > 0)
                    {
                        reader.Get(out int point, 0);
                        record.Add(point);
                    }
                }
            }
            return new Categories(record);
        }
    }
}
