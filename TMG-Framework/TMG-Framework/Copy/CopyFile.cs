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

namespace TMG.Copy
{
    [Module(Name = "Copy File", Description = "Copies a file (or directory) between two places with the option to delete after being copied.",
        DocumentationLink = "http://tmg.utoronto.ca/doc/2.0")]
    public class CopyFile : BaseAction
    {
        [Parameter(DefaultValue = "", Name = "Origin", Index = 0, Description = "The path to the file/directory to copy.")]
        public IFunction<string> Origin;

        [Parameter(DefaultValue = "", Name = "Destination", Index = 1, Description = "The path to the file/directory to copy into.")]
        public IFunction<string> Destination;

        [Parameter(DefaultValue = "False", Name = "Move", Index = 2, Description = "Should the origin be erased after the file is copied?")]
        public IFunction<bool> Move;

        public override void Invoke()
        {
            var oInfo = new FileInfo(Origin.Invoke());
            var dInfo = new FileInfo(Destination.Invoke());
            var move = Move.Invoke();
            if(!oInfo.Exists)
            {
                throw new XTMFRuntimeException(this, $"There is no file at the path {oInfo.FullName}!");
            }
            var originIsDir = oInfo.Attributes.HasFlag(FileAttributes.Directory);
            var destIsDir = dInfo.Attributes.HasFlag(FileAttributes.Directory);
            if (originIsDir)
            {
                if(!destIsDir)
                {
                    throw new XTMFRuntimeException(this, $"The path {dInfo.FullName} is not a directory where are the source {oInfo.FullName} is!");
                }
                // copy the directories
                DirectoryCopy(oInfo.FullName, dInfo.FullName);
                Directory.Delete(oInfo.FullName);
            }
            else
            {
                if(destIsDir)
                {
                    oInfo.CopyTo(dInfo.FullName, true);
                }
                else
                {
                    oInfo.CopyTo(dInfo.Directory.FullName, true);
                }
            }
        }

        private static void DirectoryCopy(string sourceDirectory, string destinationDirectory)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirectory);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirectory);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destinationDirectory, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}
