using System;

namespace Psy.Core
{
    public enum PlatformType
    {
        Windows,
        Linux,
        MacOs
    }

    public static class Platform
    {
        public static PlatformType GetExecutingPlatform()
        {
            switch ((int)Environment.OSVersion.Platform)
            {
                case 4:
                    return PlatformType.Linux;
                case 6:
                    return PlatformType.MacOs;
                case 128:
                    return PlatformType.Linux;
                default:
                    return PlatformType.Windows;
            }
        }
    }
}