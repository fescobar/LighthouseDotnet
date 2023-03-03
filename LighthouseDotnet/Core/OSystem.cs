using System;
namespace LighthouseDotnet.Core
{
    public class OSystem
    {
        public static bool IsLinux
        {
            get
            {
                int platform = (int)Environment.OSVersion.Platform;
                return platform == 4 || platform == 6 || platform == 128;
            }
        }
    }
}
