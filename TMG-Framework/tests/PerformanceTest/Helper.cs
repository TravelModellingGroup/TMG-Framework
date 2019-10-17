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
using XTMF2;
using XTMF2.RuntimeModules;

namespace PerformanceTest
{
    internal static class Helper
    {
        internal static IFunction<T> CreateParameter<T>(T value, string moduleName = null)
        {
            return new BasicParameter<T>()
            {
                Name = moduleName,
                Value = value
            };
        }

        /// <summary>
        /// Create a ReadStream from the given file path.
        /// </summary>
        /// <param name="fileName">The path to the file to read.</param>
        /// <returns>A ReadStream for the given file.</returns>
        internal static ReadStream CreateReadStreamFromFile(string fileName)
        {
            return (new OpenReadStreamFromFile()
            {
                FilePath = CreateParameter(fileName)
            }).Invoke();
        }

        /// <summary>
        /// Create a WriteStream to the given file path.
        /// </summary>
        /// <param name="fileName">The path to the file to write.</param>
        /// <returns>A WriteStream for the given file.</returns>
        internal static WriteStream CreateWriteStreamFromFile(string fileName)
        {
            return (new OpenWriteStreamFromFile()
            {
                FilePath = CreateParameter(fileName)
            }).Invoke();
        }
    }
}
