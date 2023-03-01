using System;

namespace LighthouseDotnet.Objects
{
    internal class NpmPackageVersion
    {
        public NpmPackageVersion(string version)
        {
            var arr = version.Split('.');
            if (arr.Length == 0) throw new ArgumentException("Invalid version");
            FullVersion = version;
            MajorVersion = int.Parse(arr[0]);
            MinorVersion = int.Parse(arr[1]);
            PatchVersion = int.Parse(arr[2]);
        }
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int PatchVersion { get; }
        public string FullVersion { get; }
    }
}
