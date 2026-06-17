/*
    Copyright 2021 University of Toronto

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
using System.Linq;
using System.Text;
using TMG.Utilities;
using XTMF2;

namespace TMG.Loading
{
    [Module(Name = "Load Category Map From Third Normalized CSV",
        Description = "Loads a category map (such as planning districts)",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class LoadCategoryMapFromThirdNormalizedCSV : BaseFunction<CategoryMap>
    {
        [SubModule(Index = 0, Required = true, Name = "Base Categories", Description = "The categories that this map from to the Destination Categories.")]
        public IFunction<Categories> BaseCategories;

        [SubModule(Index = 1, Required = false, Name = "Destination Categories", Description = "The categories that this map to from the Base Categories.  If not linked the CSV will create a new one based on the destination values found.")]
        public IFunction<Categories> DestinationCategories;

        [SubModule(Index = 2, Required = true, Name = "CSV Stream", Description = "The stream that we will read from.")]
        public IFunction<ReadStream> CSVStream;

        [Parameter(DefaultValue = "0", Name = "Base Column", Index = 3, Description = "The 0 indexed column containing the sparse map index for the base category index.")]
        public IFunction<int> BaseColumn;

        [Parameter(DefaultValue = "1", Name = "Destination Column", Index = 4, Description = "The 0 indexed column containing the sparse map index for the destination category index.")]
        public IFunction<int> DestinationColumn;

        public override CategoryMap Invoke()
        {
            var destinationCategories = DestinationCategories?.Invoke();
            if (destinationCategories is null)
            {
                return LoadWithCustomDestinations();
            }
            else
            {
                return LoadWithStrictDestinations(destinationCategories);
            }
        }

        /// <summary>
        /// Load the category map where we are generating the destinations on the fly.
        /// </summary>
        /// <returns>Returns the mapping of categories from the base to the destination.</returns>
        private CategoryMap LoadWithCustomDestinations()
        {
            var records = LoadMapRecords();
            Categories destinationCategories = GetDestinationCategoriesFromRecords(records);
            return Load(records, destinationCategories);
        }

        /// <summary>
        /// Load the category map where there are pre-defined destination categories.
        /// </summary>
        /// <param name="destinationCategories">The destination categories to map to.</param>
        /// <returns>Returns the mapping of categories from the base to the destination.</returns>
        private CategoryMap LoadWithStrictDestinations(Categories destinationCategories)
        {
            return Load(LoadMapRecords(), destinationCategories);
        }

        /// <summary>
        /// Generates a Categories object using the destinations specified in the loaded mapping records.
        /// </summary>
        /// <param name="records">The records of the map.</param>
        /// <returns>A Categories object that represents the destinations from the records organized in ascending order.</returns>
        /// <exception cref="XTMFRuntimeException">This throws when we are unable to create the categories object.</exception>
        private Categories GetDestinationCategoriesFromRecords(List<(int baseSparseIndex, int destinationSparseIndex)> records)
        {
            string error = null;
            var ret = Categories.CreateCategories(records
                .Select(r => r.destinationSparseIndex)
                .Distinct()
                .OrderBy(r => r)
                .ToList(), ref error);
            if(ret is null)
            {
                throw new XTMFRuntimeException(this, error);
            }
            return ret;
        }

        /// <summary>
        /// Creates the CategoryMap using the sparse records and destination categories.
        /// </summary>
        /// <param name="records">The records in sparse-space for the map.</param>
        /// <param name="destinationCategories">The destination categories that we will check for.</param>
        /// <returns>Returns a category map between the Base categories and the Destination categories.</returns>
        /// <exception cref="XTMFRuntimeException">This is thrown if there is a sparse-record that is not defined in their respective category.</exception>
        private CategoryMap Load(List<(int baseSparseIndex, int destinationSparseIndex)> records, Categories destinationCategories)
        {
            var baseCategories = BaseCategories!.Invoke();
            var flatRecords = records
                .Select(record =>
                {
                    var ret = (BaseIndex: baseCategories.GetFlatIndex(record.baseSparseIndex),
                    DestinationIndex: destinationCategories.GetFlatIndex(record.destinationSparseIndex));
                    if (ret.BaseIndex < 0)
                    {
                        ThrowBadBaseIndex(ret.BaseIndex);
                    }
                    if (ret.DestinationIndex < 0)
                    {
                        ThrowBadDestinationIndex(ret.DestinationIndex);
                    }
                    return ret;
                }).ToList();
            string error = null;
            if (!CategoryMap.CreateCategoryMap(baseCategories, destinationCategories, flatRecords, out var map, ref error))
            {
                throw new XTMFRuntimeException(this, error);
            }
            return map;
        }

        /// <summary>
        /// A helper function to throw if we load a sparse index that is not contained in the base categories.
        /// </summary>
        /// <param name="baseSparseIndex">The sparse index that was not found.</param>
        /// <exception cref="XTMFRuntimeException">This always throws.</exception>
        private void ThrowBadBaseIndex(int baseSparseIndex)
        {
            throw new XTMFRuntimeException(this, $"Found an invalid base category sparse index {baseSparseIndex} while loading in the category map!");
        }

        /// <summary>
        /// A helper function to throw if we load a sparse index that is not contained in the destination categories.
        /// </summary>
        /// <param name="baseSparseIndex">The sparse index that was not found.</param>
        /// <exception cref="XTMFRuntimeException">This always throws.</exception>
        private void ThrowBadDestinationIndex(int destinationSparseIndex)
        {
            throw new XTMFRuntimeException(this, $"Found an invalid destination category sparse index {destinationSparseIndex} while loading in the category map!");
        }

        /// <summary>
        /// Loads the sparse-index mapping records from the stream.
        /// </summary>
        /// <returns>The sparse-indexed records.</returns>
        /// <exception cref="XTMFRuntimeException">This is thrown if we are unable to properly parse a line.</exception>
        private List<(int baseSparseIndex, int destinationSparseIndex)> LoadMapRecords()
        {
            var baseColumn = BaseColumn!.Invoke();
            var destinationColumn = DestinationColumn!.Invoke();
            int requiredColumns = Math.Max(baseColumn, destinationColumn) + 1;
            var streamReader = new CsvReader(CSVStream!.Invoke(), true);
            var records = new List<(int baseSparseIndex, int destinationSparseIndex)>();
            try
            {
                while (streamReader.LoadLine(out int columns))
                {
                    if (columns >= requiredColumns)
                    {
                        streamReader.Get(out int baseSparseIndex, baseColumn);
                        streamReader.Get(out int destinationSparseIndex, destinationColumn);
                        records.Add((baseSparseIndex, destinationSparseIndex));
                    }
                }
            }
            catch
            {
                throw new XTMFRuntimeException(this, $"Unable to read category map from {streamReader.FileName} on line {streamReader.LineNumber}!");
            }

            return records;
        }
    }
}
