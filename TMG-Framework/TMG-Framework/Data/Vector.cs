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

namespace TMG
{
    public sealed class Vector
    {
        public Categories Categories => _categories;
        private readonly Categories _categories;

        public float[] Data => _data;
        private readonly float[] _data;

        public Vector(Categories categories)
        {
            _categories = categories;
            _data = new float[Categories.Count];
        }

        public Vector(Vector vector)
        {
            _categories = vector.Categories;
            _data = new float[_categories.Count];
        }

        public float this[CategoryIndex sparseIndex]
        {
            get
            {
                var index = _categories.GetFlatIndex(sparseIndex);
                if(index >= 0)
                {
                    return _data[index];
                }
                return 0.0f;
            }

            set
            {
                var index = _categories.GetFlatIndex(sparseIndex);
                if (index >= 0)
                {
                    _data[index] = value;
                }
                throw new ArgumentOutOfRangeException(nameof(sparseIndex));
            }
        }
    }
}
