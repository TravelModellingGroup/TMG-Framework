/*
    Copyright 2026 University of Toronto

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
using System.Globalization;
using System.IO.Compression;

namespace TMG.Loading;

[Module(Name = "Load Categories From NWP", Description = "Loads categories from the centroid nodes in an EMME network package.",
    DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
public sealed class LoadCategoriesFromNWP : BaseFunction<Categories>
{
    [SubModule(Index = 0, Required = true, Name = "NWP Stream", Description = "The NWP archive to load categories from.")]
    public IFunction<ReadStream> Input = null!;

    public override Categories Invoke()
    {
        List<int> categories = [];
        using var stream = Input.Invoke();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            var baseFile = archive.Entries.FirstOrDefault(entry =>
                entry.Name.Equals("base.211", StringComparison.InvariantCultureIgnoreCase));
            if (baseFile is null)
            {
                throw new XTMFRuntimeException(this, "The network package does not contain a base.211 file.");
            }

            using var reader = new StreamReader(baseFile.Open());
            string? line;
            const string nodesMarker = "t nodes";
            while ((line = reader.ReadLine()) is not null && line != nodesMarker)
            {
            }
            if (line != nodesMarker)
            {
                throw new XTMFRuntimeException(this, "The base.211 file does not contain the start of the nodes section.");
            }

            reader.ReadLine();
            var separators = new[] { ' ', '\t' };
            while ((line = reader.ReadLine()) is not null && (line.Length == 0 || line[0] != 't'))
            {
                if (line.Length < 2 || line[0] != 'a' || line[1] != '*')
                {
                    continue;
                }

                var columns = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (columns.Length < 2)
                {
                    throw new XTMFRuntimeException(this, $"Failed to load the line '{line}' when reading NWP categories. Expected a node ID.");
                }
                if (!int.TryParse(columns[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int category))
                {
                    throw new XTMFRuntimeException(this, $"Failed to load the line '{line}' when reading NWP categories. Unable to parse the node ID.");
                }
                categories.Add(category);
            }
        }

        if (categories.Count == 0)
        {
            throw new XTMFRuntimeException(this, "Unable to create Categories object with no contained categories.");
        }

        string? error = null;
        if (!Categories.CreateCategories(categories, out var result, ref error))
        {
            throw new XTMFRuntimeException(this, error);
        }
        return result;
    }
}
