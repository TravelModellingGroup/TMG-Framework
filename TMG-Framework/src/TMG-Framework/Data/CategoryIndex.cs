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

namespace TMG
{
    /// <summary>
    /// Represents an index into an object of Categories.
    /// </summary>
    public readonly struct CategoryIndex
    {
        private readonly int _Value;

        public CategoryIndex(int value)
        {
            _Value = value;
        }

        /// <summary>
        /// Test if the value is a valid reference to the category
        /// </summary>
        public bool Exists => _Value >= 0;

        public static implicit operator int(CategoryIndex index) => index._Value;

        public static implicit operator CategoryIndex(int index) => new CategoryIndex(index);

        public static bool TryParse(string s, out CategoryIndex result)
        {
            if (!int.TryParse(s, out int temp))
            {
                result = temp;
                return true;
            }
            result = -1;
            return false;
        }
    }
}
