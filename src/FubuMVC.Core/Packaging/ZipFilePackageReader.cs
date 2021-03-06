﻿using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class ZipFilePackageReader : IPackageLoader
    {
        private readonly IPackageManifestReader _reader;
        private readonly IPackageExploder _exploder;

        public ZipFilePackageReader(IPackageManifestReader reader, IPackageExploder exploder)
        {
            _reader = reader;
            _exploder = exploder;
        }

        public IEnumerable<IPackageInfo> Load()
        {
            var applicationDirectory = FubuMvcPackageFacility.GetApplicationPath();
            return _exploder.ExplodeAllZipsAndReturnPackageDirectories(applicationDirectory).Select(dir => _reader.LoadFromFolder(dir));
        }

        public static string GetContentFolderForPackage(string packageFolder)
        {
            return FileSystem.Combine(packageFolder, FubuMvcPackages.WebContentFolder);
        }
    }
}