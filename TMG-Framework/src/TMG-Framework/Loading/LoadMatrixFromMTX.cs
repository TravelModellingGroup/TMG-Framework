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
    [Module(Name = "Load SparseMatrix From MTX", Description = "Loads a matrix of data in the shape of the SparseMap from an EMME matrix file.",
    DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class LoadMatrixFromMTX : BaseFunction<ReadStream, Matrix>
    {
        [SubModule(Required = true, Name = "Map", Description = "The sparse map this vector will be shaped in.", Index = 0)]
        public IFunction<Categories> Categories;

        private const uint MagicNumber = 0xC4D4F1B2;

        private const int FloatType = 0x1;

        public override Matrix Invoke(ReadStream context)
        {
            var categories = Categories.Invoke();
            var matrix = new Matrix(categories, categories);
            using (var reader = new BinaryReader(context))
            {
                var magic = reader.ReadUInt32();
                if(magic != MagicNumber)
                {
                    throw new XTMFRuntimeException(this, "The file was not an EMME matrix!");
                }
                // version
                reader.ReadInt32();
                var type = reader.ReadInt32();
                if(type != FloatType)
                {
                    throw new XTMFRuntimeException(this, "The matrix was not using a float type!");
                }
                var numberOfIndexes = reader.ReadInt32();
                if(numberOfIndexes != 2)
                {
                    throw new XTMFRuntimeException(this, $"The matrix contained {numberOfIndexes} dimensions!");
                }
                int firstSize = reader.ReadInt32();
                int secondSize = reader.ReadInt32();
                if(categories.Count != firstSize)
                {
                    throw new XTMFRuntimeException(this, $"The matrix had the wrong number of elements in the first dimension!");
                }
                if (categories.Count != secondSize)
                {
                    throw new XTMFRuntimeException(this, $"The matrix had the wrong number of elements in the second dimension!");
                }
                ValidateIndexes(reader, categories);
                ValidateIndexes(reader, categories);
                var dataSize = categories.Count * categories.Count * sizeof(float);
                ConversionBuffer buffer = new ConversionBuffer()
                {
                    FloatData = matrix.Data
                };
                buffer.GetByteBuffer(dataSize);
                var soFar = 0;
                while (soFar < dataSize)
                {
                    var amount = reader.Read(buffer.ByteData, soFar, dataSize - soFar);
                    if(amount == 0)
                    {
                        throw new XTMFRuntimeException(this, $"The matrix expected {dataSize}bytes but we only could get {soFar}bytes!");
                    }
                    soFar += amount;
                }
                buffer.FinalizeAsFloatArray(dataSize / sizeof(float));
            }
            return matrix;
        }

        private void ValidateIndexes(BinaryReader reader, Categories categories)
        {
            var length = categories.Count;
            for (int i = 0; i < length; i++)
            {
                var index = reader.ReadInt32();
                if(index != categories.GetSparseIndex(i))
                {
                    throw new XTMFRuntimeException(this, $"The matrix file has an index of {index} where we were expecting an index of {categories.GetSparseIndex(i)}!");
                }
            }
        }
    }
}
