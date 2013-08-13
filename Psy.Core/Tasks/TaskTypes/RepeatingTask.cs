using System;

namespace Psy.Core.Tasks.TaskTypes
{
    class RepeatingTask : Task
    {
        internal RepeatingTask(string name, TaskProcess process, int repetitionDelay)
            : base(name, process, 0, repetitionDelay)
        {
            TaskStatus = TaskStatus.Running;
        }

        protected override void SetPostExecuteTaskStatus()
        {
            TaskStatus = TaskStatus.RepeatWait;
        }
    }
}