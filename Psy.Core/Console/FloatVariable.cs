using System;

namespace Psy.Core.Console
{
    public class FloatVariable
    {
        public Func<float> GetValue;
        public Action<float> SetValue;
    }
}