namespace Psy.Core
{
    public class TimeSample
    {
        public double Last { get; private set; }
        public double Average { get; private set; }

        private double _beginTime;

        public void Begin()
        {
            _beginTime = Timer.GetTime();
        }

        public void End()
        {
            var duration = Timer.GetTime() - _beginTime;
            Last = duration;
            Average += Last;
            Average /= 2;
        }

        public override string ToString()
        {
            return string.Format("Last:{0:0.000}, Average:{1:0.000}", Last, Average);
        }
    }
}