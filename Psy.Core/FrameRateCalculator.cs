using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Psy.Core
{
    public class FrameRateCalculator
    {
        public long MillisecondsPerFrame { get; set; }
        
        public float FramesPerSecond
        {
            get { return 1000.0f/MillisecondsPerFrame; }
        }

        public string HudText
        {
            get
            {
                
                return string.Format("UpdateFPS: Current/Last10/FPSMAX : {0}/{1}/{2}",
                MillisecondsPerFrame, _lastTenAverage.ToString("#.##"), FramesPerSecond);
            }
        }

        private readonly Queue<long> _recentFrameDurations;
        private readonly Stopwatch _stopwatch;
        private double _lastTenAverage;

        public FrameRateCalculator()
        {
            _stopwatch = Stopwatch.StartNew();
            _recentFrameDurations = new Queue<long>(10);
        }

        public void FrameBegin()
        {
            _stopwatch.Restart();
        }

        public void FrameEnd()
        {
            _stopwatch.Stop();
            MillisecondsPerFrame = _stopwatch.ElapsedMilliseconds;
            _recentFrameDurations.Enqueue(MillisecondsPerFrame);
            if (_recentFrameDurations.Count > 10)
                _recentFrameDurations.Dequeue();

            if (_recentFrameDurations.Count > 0)
            {
                _lastTenAverage = _recentFrameDurations.Average();
            }
        }
    }
}