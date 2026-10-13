/*
    Copyright 2017-2026 University of Toronto

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

using System.IO.Compression;

namespace TMG.Saving;

[Module(Name = "Save Matrix To MTX", Description = "Saves a matrix to the given write stream.",
DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
public class SaveMatrixAsMTX : BaseAction
{
    private const uint MagicNumber = 0xC4D4F1B2;

    private const int FloatType = 0x1;

    [SubModule(Required = true, Description = "The matrix to be saved as MTX.", Name = "Matrix", Index = 0)]
    public IFunction<Matrix> Matrix = null!;

    [SubModule(Required = true, Description = "The write stream to which the MTX will be saved.", Name = "WriteTo", Index = 1)]
    public IFunction<WriteStream> WriteTo = null!;

    [Parameter(Name = "Compressed", DefaultValue = "False", Index = 2,
        Description = "Should the MTX file be compressed (mtx.gz)?")]
    public IFunction<bool> Compressed = null!;

    public override void Invoke()
    {
        var matrix = Matrix.Invoke();
        var stream = WriteTo.Invoke();
        Stream outputStream = Compressed.Invoke()
            ? new GZipStream(stream, CompressionMode.Compress)
            : stream;
        using var writer = new BinaryWriter(outputStream);
        var rowCategories = matrix.RowCategories;
        var columnCategories = matrix.ColumnCategories;
        var rowLength = rowCategories.Count;
        var columnLength = columnCategories.Count;
        writer.Write(MagicNumber);
        writer.Write((int)1);
        writer.Write(FloatType);
        writer.Write((int)2);
        // write out the lengths for each index
        writer.Write(rowLength);
        writer.Write(columnLength);
        writer.Flush();
        using (var indexWriter = new BinaryWriter(outputStream, Encoding.Unicode, true))
        {
            for (int i = 0; i < rowLength; i++)
            {
                indexWriter.Write(rowCategories.GetSparseIndex(i));
            }
            for (int i = 0; i < columnLength; i++)
            {
                indexWriter.Write(columnCategories.GetSparseIndex(i));
            }
        }
        // use span to avoid copying the data
        writer.Write(MemoryMarshal.Cast<float, byte>(matrix.Data));
    }
}
