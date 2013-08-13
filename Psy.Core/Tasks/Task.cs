namespace Psy.Core.Tasks
{
    abstract class Task : ITask
    {
        public TaskProcess TaskProcess { get; private set; }
        public TaskStatus TaskStatus { get; protected set; }
        private string Name { get; set; }

        public int InitialDelay { get; private set; }
        public int RepetitionDelay { get; private set; }

        public string GetDebugString()
        {
            return string.Format("{0,-30} last:{1,-4:0.000}  avg:{2,-4:0.000}", Name, LastExecutionTime, AverageExecutionTime);
        }

        private double LastExecution { get; set; }
        private double NextExecution { get; set; }

        public double LastExecutionTime { get; set; }
        public double AverageExecutionTime { get; set; }

        protected abstract void SetPostExecuteTaskStatus();


        protected Task(string name, TaskProcess process, int initalDelay, int repetitionDelay)
        {
            TaskProcess = process;
            Name = name;
            
            InitialDelay = initalDelay;
            RepetitionDelay = repetitionDelay;
            LastExecution = Timer.GetTime();
            NextExecution = LastExecution + initalDelay;
        }

        private bool ShouldExecute()
        {
            if (TaskStatus == TaskStatus.Stopped)
                return false;

            var now = Timer.GetTime();

            return now >= NextExecution;
        }

        private double GetNextExecutionTime()
        {
            return NextExecution + RepetitionDelay;
        }

        public bool Execute()
        {
            if (ShouldExecute())
            {
                var taskStartTime = Timer.GetTime();
                TaskProcess();
                var taskEndTime = Timer.GetTime();
                SetPostExecuteTaskStatus();
                LastExecution = NextExecution;
                NextExecution = GetNextExecutionTime();

                LastExecutionTime = taskEndTime - taskStartTime;
                AverageExecutionTime += LastExecutionTime;
                AverageExecutionTime /= 2;

                return true;
            }
                
            return false;
        }
    }
}
