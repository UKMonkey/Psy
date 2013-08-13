using System;

namespace Psy.Core.Tasks.TaskTypes
{
    class DelayedTask : Task
    {
        internal DelayedTask(string name, TaskProcess process, int initialDelay)
            : base(name, process, initialDelay, 0)
        {
            TaskStatus = TaskStatus.DelayedWait;
        }

        protected override void SetPostExecuteTaskStatus()
        {
            TaskStatus = TaskStatus.Stopped;
        }
    }
}