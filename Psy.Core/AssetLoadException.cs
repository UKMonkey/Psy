using System;

namespace Psy.Core
{
    public class AssetLoadException : Exception
    {
        public AssetLoadException(string message) : base(message) { }
    }
}
