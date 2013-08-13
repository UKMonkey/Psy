using System.Diagnostics;
namespace Psy.Core
{
    public static class Timer
    {
        // ticks / (second * 1000)   =  ticks / ms
        private static readonly Stopwatch Counter = Stopwatch.StartNew();

        private static readonly double TicksPerMillisecond = Stopwatch.Frequency/1000.0d;

        /// <summary>
        /// Returns elapsed time in fractional milliseconds.
        /// </summary>
        /// <returns></returns>
        public static double GetTime()
        {
            return Counter.ElapsedTicks / TicksPerMillisecond;
        }
    }
}
