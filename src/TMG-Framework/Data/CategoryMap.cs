/*
    Copyright 2018-2019 University of Toronto

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
using System.Linq;
using System.Text;
using static TMG.Utilities.ExceptionHelper;

namespace TMG
{
    /// <summary>
    /// Provides a mapping between categories and provides a method for aggregating
    /// data from the base categories to the destination categories.
    /// </summary>
    public sealed class CategoryMap
    {
        /// <summary>
        /// The categories to start with.
        /// </summary>
        public Categories Base { get; }

        /// <summary>
        /// The categories to converge the base into.
        /// </summary>
        public Categories Destination { get; }

        /// <summary>
        /// The mapping between the two category types.
        /// </summary>
        private readonly List<(int originFlatIndex, int destinationFlatIndex)> _baseToDestination;

        public static bool CreateCategoryMap(Categories baseCategories, Categories destinationCategories,
            List<(int originFlatIndex, int destinationFlatIndex)> baseToDestination, out CategoryMap map, ref string error)
        {
            if (baseCategories == null)
            {
                ThrowParameterNull(nameof(baseCategories));
            }
            if (destinationCategories == null)
            {
                ThrowParameterNull(nameof(destinationCategories));
            }
            if (baseToDestination == null)
            {
                ThrowParameterNull(nameof(baseToDestination));
            }
            if (ValidateMapping(baseCategories, destinationCategories, baseToDestination, ref error))
            {
                map = new CategoryMap(baseCategories, destinationCategories, baseToDestination);
                return true;
            }
            map = null;
            return false;
        }

        /// <summary>
        /// Given two categories create the mapping between the two.
        /// </summary>
        /// <param name="baseCategories">The larger set of categories.</param>
        /// <param name="destinationCategories">The different set of categories the base will be mapped to.</param>
        /// <param name="baseToDestination">The individual linking records from base to destination categories.</param>
        private CategoryMap(Categories baseCategories, Categories destinationCategories,
            List<(int originFlatIndex, int destinationFlatIndex)> baseToDestination)
        {
            if (baseCategories == null)
            {
                ThrowParameterNull(nameof(baseCategories));
            }
            if (destinationCategories == null)
            {
                ThrowParameterNull(nameof(destinationCategories));
            }
            if (baseToDestination == null)
            {
                ThrowParameterNull(nameof(baseToDestination));
            }
            Base = baseCategories;
            Destination = destinationCategories;
            _baseToDestination = baseToDestination;
        }

        private static bool FailWith(ref string error, string message)
        {
            error = message;
            return false;
        }

        /// <summary>
        /// Ensure that all of the indexes exist in the base and destination categories.
        /// </summary>
        /// <param name="baseToDestination"></param>
        private static bool ValidateMapping(Categories baseCategories, Categories destinationCategories,
            List<(int originFlatIndex, int destinationFlatIndex)> baseToDestination, ref string error)
        {
            if (baseCategories == null)
            {
                return FailWith(ref error, "baseCategories was null!");
            }
            if (destinationCategories == null)
            {
                return FailWith(ref error, "destinationCategories was null!");
            }
            if (baseToDestination == null)
            {
                return FailWith(ref error, "baseToDestination was null!");
            }
            foreach (var (originFlatIndex, destinationFlatIndex) in baseToDestination)
            {
                if (originFlatIndex < 0 || originFlatIndex >= baseCategories.Count)
                    return FailWith(ref error, $"The base categories does not contain a flat index of {originFlatIndex}!");
                if (destinationFlatIndex < 0 || destinationFlatIndex >= destinationCategories.Count)
                    return FailWith(ref error, $"The destination categories does not contain a flat index of {destinationFlatIndex}!");
            }
            return true;
        }


        /// <summary>
        /// Aggregate the baseVector data into the destination category system.
        /// </summary>
        /// <param name="baseVector">The vector to aggregate</param>
        /// <param name="ret"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool AggregateToDestination(Vector baseVector, out Vector ret, ref string error)
        {
            ret = null;
            if (baseVector == null)
            {
                return FailWith(ref error, "baseVector was null!");
            }
            if (baseVector.Categories != Base)
            {
                return FailWith(ref error, "Invalid Base Categories");
            }
            ret = new Vector(Destination);
            var b = baseVector.Data;
            var r = ret.Data;
            foreach (var (originFlatIndex, destinationFlatIndex) in _baseToDestination)
            {
                r[destinationFlatIndex] += b[originFlatIndex];
            }
            return true;
        }

        /// <summary>
        /// Creates a reverse index of destination category indexes relating them to the list of Base category indexes that they map.
        /// </summary>
        /// <returns>A reverse index dictionary mapping Destination elements to a list of base category indexes that represent them.</returns>
        public Dictionary<CategoryIndex, List<CategoryIndex>> CreateReverseIndex()
        {
            return _baseToDestination
                .GroupBy(record => record.destinationFlatIndex)
                .ToDictionary(group => (CategoryIndex)group.Key, group => group.Select(record => (CategoryIndex)record.originFlatIndex).ToList());
        }

        /// <summary>
        /// Creates an index of base category to destination category indexes.
        /// </summary>
        /// <returns>A dictionary containing the mapping between base CategoryIndex to destination CategoryIndex.</returns>
        public Dictionary<CategoryIndex, CategoryIndex> CreateIndex()
        {
            return _baseToDestination
                .ToDictionary(record => (CategoryIndex)record.originFlatIndex, record => (CategoryIndex)record.destinationFlatIndex);
        }
    }
}
