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

namespace TMG.Test.Utilities
{
    internal static class Helper
    {
        /// <summary>
        /// Generate a new basic parameter with the given value
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="value">The value to be returned</param>
        /// <param name="moduleName">The name of the module to create.</param>
        /// <returns></returns>
        internal static IFunction<T> CreateParameter<T>(T value, string moduleName = null)
        {
            return new BasicParameter<T>()
            {
                Name = moduleName,
                Value = value
            };
        }

        private class CustomizableModule<T, K> : BaseFunction<T, K>
        {
            private Func<T, K> _inner;

            public CustomizableModule(Func<T,K> inner)
            {
                _inner = inner;
            }

            public override K Invoke(T context)
            {
                return _inner(context);
            }
        }

        /// <summary>
        /// Generate a simple module
        /// </summary>
        /// <typeparam name="T">InputType</typeparam>
        /// <typeparam name="K">OutputType</typeparam>
        /// <param name="innerFunction">Processing logic</param>
        /// <returns>A module that executes the function passed in.</returns>
        internal static IFunction<T,K> CreateModule<T,K>(Func<T,K> innerFunction, string moduleName = null)
        {
            return new CustomizableModule<T, K>(innerFunction)
            {
                Name = moduleName
            };
        }
    }
}
