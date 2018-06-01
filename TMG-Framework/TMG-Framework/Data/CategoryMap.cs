/*
    Copyright 2018 University of Toronto

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

namespace TMG
{
    /// <summary>
    /// Provides a mapping between categories and provides a method for aggregating
    /// data from the base categories to the destination categories.
    /// </summary>
    public sealed class CategoryMap
    {
        /// <summary>
        /// 
        /// </summary>
        public Categories Base => _baseCategories;
        private readonly Categories _baseCategories;

        /// <summary>
        /// 
        /// </summary>
        public Categories Destination => _destination;
        private readonly Categories _destination;

        private readonly List<(int originFlatIndex, int destinationFlatIndex)> _baseToDestination;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseCategories"></param>
        /// <param name="destinationCategories"></param>
        /// <param name="baseToDestination"></param>
        public CategoryMap(Categories baseCategories, Categories destinationCategories, 
            List<(int originFlatIndex, int destinationFlatIndex)> baseToDestination)
        {
            _baseCategories = baseCategories ?? throw new ArgumentNullException(nameof(baseCategories));
            _destination = destinationCategories ?? throw new ArgumentNullException(nameof(destinationCategories));
            _baseToDestination = baseToDestination;
            ValidateMapping(baseToDestination);
        }

        /// <summary>
        /// Ensure that all of the indexes exist in the base and destination categories.
        /// </summary>
        /// <param name="baseToDestination"></param>
        private void ValidateMapping(List<(int originFlatIndex, int destinationFlatIndex)> baseToDestination)
        {
            foreach(var (originFlatIndex, destinationFlatIndex) in baseToDestination)
            {
                if (originFlatIndex < 0 || originFlatIndex >= _baseCategories.Count)
                    throw new IndexOutOfRangeException($"The base categories does not contain a flat index of {originFlatIndex}!");
                if (destinationFlatIndex < 0 || destinationFlatIndex >= _destination.Count)
                    throw new IndexOutOfRangeException($"The destination categories does not contain a flat index of {destinationFlatIndex}!");
            }
        }

        /// <summary>
        /// Aggregate the baseVector data into the destination category system.
        /// </summary>
        /// <param name="baseVector">The vector to aggregate</param>
        /// <returns>A new vector with the mapped results from the base into the destination categories.</returns>
        public Vector AggregateToDestination(Vector baseVector)
        {
            if (baseVector == null)
            {
                throw new ArgumentNullException(nameof(baseVector));
            }
            if(baseVector.Categories != _baseCategories)
            {
                throw new ArgumentException("Invalid Base Categories", nameof(baseVector));
            }
            var ret = new Vector(Destination);
            var b = baseVector.Data;
            var r = ret.Data;
            foreach(var (originFlatIndex, destinationFlatIndex) in _baseToDestination)
            {
                r[destinationFlatIndex] += b[originFlatIndex];
            }
            return ret;
        }
    }
}
