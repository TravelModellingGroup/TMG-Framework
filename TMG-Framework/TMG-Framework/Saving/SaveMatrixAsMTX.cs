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
using System.IO;
using XTMF2;
using TMG.Utilities;

namespace TMG.Saving
{
    [Module(Name = "Save Matrix To MTX", Description = "Saves a matrix to the given write stream.",
    DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class SaveMatrixAsMTX : BaseAction<(Matrix Matrix, WriteStream Stream)>
    {
        private const uint MagicNumber = 0xC4D4F1B2;

        private const int FloatType = 0x1;

        public override void Invoke((Matrix Matrix, WriteStream Stream) context)
        {
            using (var writer = new BinaryWriter(context.Stream))
            {
                var matrix = context.Matrix;
                var map = matrix.RowCategories;
                var length = map.Count;
                writer.Write(MagicNumber);
                writer.Write((int)1);
                writer.Write(FloatType);
                writer.Write((int)2);
                // write out the lengths for each index
                writer.Write(length);
                writer.Write(length);
                writer.Flush();
                using (var indexBuffer = new MemoryStream())
                using (var indexWriter = new BinaryWriter(indexBuffer))
                {
                    for (int i = 0; i < length; i++)
                    {
                        indexWriter.Write(map.GetSparseIndex(i));
                    }
                    indexWriter.Flush();
                    // write out the indexes twice
                    indexBuffer.WriteTo(context.Stream);
                    indexBuffer.WriteTo(context.Stream);
                }
                // use conversion buffer to avoid copying the data
                var buffer = new ConversionBuffer()
                {
                    FloatData = matrix.Data
                };
                writer.Write(buffer.GetByteBuffer(length * length * sizeof(float)));
                buffer.FinalizeAsFloatArray(length * length);
            }
        }
    }
}
