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
    [Module(Name = "Load Matrix From CSV", Description = "Loads a matrix of data in the shape of the SparseMap from a CSV in third normalized form.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class LoadMatrixFromCSVThirdNormalized : BaseFunction<ReadStream, Matrix>
    {
        [SubModule(Required = true, Name = "Map", Description = "The sparse map this vector will be shaped in.", Index = 0)]
        public IFunction<Map> Map;

        [Parameter(DefaultValue = "0", Name = "Origin Column", Index = 1, Description = "The 0 indexed column containing the sparse map index for the origin.")]
        public IFunction<int> OriginColumn;

        [Parameter(DefaultValue = "1", Name = "Destination Column", Index = 2, Description = "The 0 indexed column containing the sparse map index for the destination.")]
        public IFunction<int> DestinationColumn;

        [Parameter(DefaultValue = "2", Name = "Data Column", Index = 3, Description = "The 0 indexed column containing the data to load index.")]
        public IFunction<int> DataColumn;

        public override Matrix Invoke(ReadStream stream)
        {
            var map = Map.Invoke();
            var rowSize = map.Count;
            var ret = new Matrix(map);
            var data = ret.Data;
            var originColumn = OriginColumn.Invoke();
            var destinationColumn = DestinationColumn.Invoke();
            var dataColumn = DataColumn.Invoke();
            if (originColumn < 0 || destinationColumn < 0 || dataColumn < 0)
            {
                throw new XTMFRuntimeException(this, "Column indexes must be greater than or equal to zero!");
            }
            var minColumnSize = Math.Max(originColumn, dataColumn);
            using (var reader = new CsvReader(stream, true))
            {
                reader.LoadLine();
                while (reader.LoadLine(out var columns))
                {
                    // This is strictly greater because the column size is 0 indexed
                    if (columns > minColumnSize)
                    {
                        int flatOrigin, flatDestination;
                        reader.Get(out int originIndex, originColumn);
                        reader.Get(out int destinationIndex, originColumn);
                        reader.Get(out float dataValue, dataColumn);
                        if ((flatOrigin = map.GetFlatIndex(originIndex)) >= 0 && (flatDestination = map.GetFlatIndex(destinationIndex)) >= 0)
                        {
                            // if we know where to put it
                            data[flatOrigin * rowSize + flatDestination] = dataValue;
                        }
                        else
                        {
                            if (flatOrigin < 0)
                            {
                                throw new XTMFRuntimeException(this, $"An invalid origin was specified {originIndex}!");
                            }
                            else
                            {
                                throw new XTMFRuntimeException(this, $"An invalid destination was specified {destinationIndex}!");
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }
}
