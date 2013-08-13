namespace Psy.Core.Tasks
{
    public interface ITask
    {
        TaskProcess TaskProcess { get; }
        TaskStatus TaskStatus { get; }
        int InitialDelay { get; }
        int RepetitionDelay { get; }
        string GetDebugString();
    }
}
