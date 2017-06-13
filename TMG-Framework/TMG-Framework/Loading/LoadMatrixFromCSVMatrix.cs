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
using TMG.Utilities;
using XTMF2;

namespace TMG.Loading
{
    [Module(Name = "Load Matrix From CSV", Description = "Loads a matrix of data in the shape of the SparseMap from a CSV in third normalized form.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class LoadMatrixFromCSVMatrix : BaseFunction<ReadStream, Matrix>
    {
        [SubModule(Required = true, Name = "Map", Description = "The sparse map this vector will be shaped in.", Index = 0)]
        public IFunction<Map> Map;

        public override Matrix Invoke(ReadStream stream)
        {
            var map = Map.Invoke();
            var ret = new Matrix(map);
            var flatData = ret.Data;
            var rowSize = map.Count;
            using (var reader = new CsvReader(stream, true))
            {
                int columns = reader.LoadLine();
                // read in the destination indexes
                int[] destinationFlatIndex = new int[columns - 1];
                for (int i = 1; i < columns; i++)
                {
                    reader.Get(out int sparseIndex, i);
                    if((destinationFlatIndex[i - 1] = map.GetFlatIndex(sparseIndex)) < 0)
                    {
                        throw new XTMFRuntimeException(this, $"Invalid sparse column index {sparseIndex}!");
                    }
                }
                while(reader.LoadLine(out columns))
                {
                    if(columns >= destinationFlatIndex.Length + 1)
                    {
                        reader.Get(out int sparseIndex, 0);
                        var originOffset = map.GetFlatIndex(sparseIndex) * rowSize;
                        if(originOffset < 0)
                        {
                            throw new XTMFRuntimeException(this, $"Invalid sparse row index {sparseIndex}!");
                        }
                        for (int i = 0; i < destinationFlatIndex.Length; i++)
                        {
                            reader.Get(out flatData[originOffset + destinationFlatIndex[i]], i + 1);
                        }
                    }
                }
            }
            return ret;
        }
    }
}
