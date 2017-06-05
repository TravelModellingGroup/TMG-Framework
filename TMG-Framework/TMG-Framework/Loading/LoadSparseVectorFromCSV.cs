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
    [Module(Name = "Load SparseVector From CSV", Description = "Loads a map where each row has a different sparse index.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class LoadSparseVectorFromCSV : BaseFunction<ReadStream, SparseVector>
    {
        [SubModule(Required = true, Name = "Map", Description = "The sparse map this vector will be shaped in.", Index = 0)]
        public IFunction<SparseMap> SparseMap;

        [Parameter(DefaultValue = "0", Name = "Map Column", Index = 1, Description = "The 0 indexed column containing the sparse map index.")]
        public IFunction<int> MapColumn;

        [Parameter(DefaultValue = "1", Name = "Data Column", Index = 2, Description = "The 0 indexed column containing the data to load index.")]
        public IFunction<int> DataColumn;

        public override SparseVector Invoke(ReadStream stream)
        {
            var map = SparseMap.Invoke();
            var ret = new SparseVector(map);
            var data = ret.Data;
            var mapColumn = MapColumn.Invoke();
            var dataColumn = DataColumn.Invoke();
            if(mapColumn < 0 || dataColumn < 0)
            {
                throw new XTMFRuntimeException(this, "Column indexes must be greater than or equal to zero!");
            }
            var minColumnSize = Math.Max(mapColumn, dataColumn);
            using (var reader = new CsvReader(stream, true))
            {
                reader.LoadLine();
                while(reader.LoadLine(out var columns))
                {
                    // This is strictly greater because the column size is 0 indexed
                    if(columns > minColumnSize)
                    {
                        int flatIndex;
                        reader.Get(out int mapIndex, mapColumn);
                        reader.Get(out float dataValue, dataColumn);
                        if((flatIndex = map.GetFlatIndex(mapIndex)) >= 0)
                        {
                            // if we know where to put it
                            data[flatIndex] = dataValue;
                        }
                        else
                        {
                            throw new XTMFRuntimeException(this, $"An invalid sparse map index was specified {mapIndex}!");
                        }
                    }
                }
            }
            return ret;
        }
    }
}
