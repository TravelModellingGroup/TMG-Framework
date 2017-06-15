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
using System.IO;
using XTMF2;

namespace TMG.FileOperations
{
    [Module(Name = "Delete File", Description = "Delete a file (or directory).",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public sealed class DeleteFile : BaseAction
    {
        [Parameter(DefaultValue = "", Name = "To Delete", Index = 0, Description = "The path to delete.")]
        public IFunction<string> ToDelete;

        public override void Invoke()
        {
            var info = new FileInfo(ToDelete.Invoke());
            if(!info.Exists)
            {
                throw new XTMFRuntimeException(this, $"The file does not exist {info.FullName}!");
            }
            if (info.Attributes.HasFlag(FileAttributes.Directory))
            {
                new DirectoryInfo(info.FullName).Delete(true);
            }
            else
            {
                info.Delete();
            }
        }
    }
}
