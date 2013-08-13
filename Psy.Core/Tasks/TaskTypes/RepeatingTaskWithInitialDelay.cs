using System;

namespace Psy.Core.Tasks.TaskTypes
{
    class RepeatingTaskWithInitialDelay : Task
    {
        internal RepeatingTaskWithInitialDelay(string name, TaskProcess process, int initialDelay, int repetitionDelay)
            : base(name, process, initialDelay, repetitionDelay)
        {
            TaskStatus = TaskStatus.DelayedWait;
        }

        protected override void SetPostExecuteTaskStatus()
        {
            TaskStatus = TaskStatus.RepeatWait;
        }
    }
}