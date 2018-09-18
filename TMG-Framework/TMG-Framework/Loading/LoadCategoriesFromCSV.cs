/*
    Copyright 2017-2018 University of Toronto

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
    [Module(Name = "Load Categories From CSV", Description = "Loads categories where each row has a different sparse index.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class LoadCategoriesFromCSV : BaseFunction<ReadStream, Categories>
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
            if(record.Count == 0)
            {
                throw new XTMFRuntimeException(this, "Unable to create Categories object with no contained categories.");
            }
            string error = null;
            var ret = Categories.CreateCategories(record, ref error);
            if(ret == null)
            {
                throw new XTMFRuntimeException(this, error);
            }
            return ret;
        }
    }
}
