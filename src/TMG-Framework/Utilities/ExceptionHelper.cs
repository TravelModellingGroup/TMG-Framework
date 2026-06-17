/*
    Copyright 2019 University of Toronto

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

namespace TMG.Utilities
{
    /// <summary>
    /// This class provides wrappers around common exception types.  The
    /// reason to use this is to help the JIT inline methods.  If a method
    /// throws an exception it can not be inlined however, if you call one of the
    /// exception helpers it can still be inlined.
    /// </summary>
    internal static class ExceptionHelper
    {
        /// <summary>
        /// Invoke an out of range exception for a parameter with the given name.
        /// </summary>
        /// <param name="name">The name of the parameter that caused the out of range exception.</param>
        internal static void ThrowOutOfRangeException(string name)
        {
            throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Invoke an ArgumentNullException for a parameter with the given name.
        /// </summary>
        /// <param name="name">The name of the parameter that caused the ArgumentNullException.</param>
        internal static void ThrowParameterNull(string name)
        {
            throw new ArgumentNullException(name);
        }
    }
}
