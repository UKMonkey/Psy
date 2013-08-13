namespace Psy.Core.Tasks.TaskTypes
{
    class ImmediateRunOnceTask : Task
    {
        internal ImmediateRunOnceTask(string name, TaskProcess process) : base(name, process, 0, 0)
        {
            TaskStatus = TaskStatus.Running;
        }

        protected override void SetPostExecuteTaskStatus()
        {
            TaskStatus = TaskStatus.Stopped;
        }
    }
}