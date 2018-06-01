/*
    Copyright 2017-2018 University of Toronto

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
using System.Linq;

namespace TMG
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Categories
    {
        /// <summary>
        /// Get the number of elements in the categories.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// TODO: Update the representation later on to something more efficient
        /// </summary>
        private readonly List<int> _elements;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Categories CreateCategories(List<int> elements, ref string error)
        {
            elements = elements?.ToList() ?? throw new ArgumentNullException(nameof(elements));
            elements.Sort();
            for (int i = 1; i < elements.Count; i++)
            {
                if(elements[i - 1] == elements[i])
                {
                    error = $"Found a duplicate category {elements[i]}!";
                    return null;
                }
            }
            return new Categories(elements);
        }

        /// <summary>
        /// Create a SparseMap from a list of elements
        /// </summary>
        /// <param name="elements">The sorted elements to use</param>
        private Categories(List<int> elements)
        {
            _elements = elements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sparseIndex"></param>
        /// <returns></returns>
        public int GetFlatIndex(CategoryIndex sparseIndex)
        {
            return _elements.BinarySearch(sparseIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flatIndex"></param>
        /// <returns></returns>
        public CategoryIndex GetSparseIndex(int flatIndex)
        {
            if(flatIndex < 0 || flatIndex >= _elements.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(flatIndex));
            }
            return _elements[flatIndex];
        }
    }
}
