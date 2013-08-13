using System.Collections.Generic;
using Psy.Core.Console;

namespace Psy.Core.Tasks
{
    public class TaskQueue
    {
        private readonly List<ITask> _taskList;

        private double _lastTaskExecutionTime;
        private double _averageTaskExecutionTime;

        public TaskQueue()
        {
            _taskList = new List<ITask>(10);
        }

        public void HandleTasksCommand(params string[] parameters)
        {
            var console = StaticConsole.Console;

            foreach (var task in _taskList)
            {
                var taskString = task.GetDebugString();
                console.AddLine(taskString, Colours.Yellow);
            }
            console.AddLine("------------------------", Colours.Yellow);
            console.AddLine(
                string.Format("last:{0,-4:0.000}, avg:{1,-4:0.000}", 
                    _lastTaskExecutionTime, _averageTaskExecutionTime));
        }

        public ITask CreateOneOffTask(string name, TaskProcess process)
        {
            var newTask = new TaskTypes.ImmediateRunOnceTask(name, process);
            _taskList.Add(newTask);
            return newTask;
        }

        public ITask CreateOneOffTaskWithDelay(string name, TaskProcess process, int initialDelay)
        {
            var newTask = new TaskTypes.DelayedTask(name, process, initialDelay);
            _taskList.Add(newTask);
            return newTask;
        }

        public ITask CreateRepeatingTask(string name, TaskProcess process, int repetitionDelay)
        {
            var newTask = new TaskTypes.RepeatingTask(name, process, repetitionDelay);
            _taskList.Add(newTask);
            return newTask;
        }

        public ITask CreateRepeatingTaskWithDelay(string name, TaskProcess process, int initalDelay, int repetitionDelay)
        {
            var newTask = new TaskTypes.RepeatingTaskWithInitialDelay(name, process, initalDelay, repetitionDelay);
            _taskList.Add(newTask);
            return newTask;
        }

        public void UnscheduleTask(ITask task)
        {
            _taskList.Remove(task);
        }

        public void ProcessAll()
        {
            var executeStartTime = Timer.GetTime();
            foreach (var taskListTask in _taskList)
            {
                var task = ((Task) taskListTask);
                task.Execute();
            }
            var executeEndTime = Timer.GetTime();
            _lastTaskExecutionTime = executeEndTime - executeStartTime;
            _averageTaskExecutionTime += _lastTaskExecutionTime;
            _averageTaskExecutionTime /= 2;
        }
    }
}
