namespace Psy.Core.StateMachine
{
    public interface IStateMachine<out TStateInterface, out TSharedContext> 
        where TStateInterface : IState<TSharedContext>
    {
        TStateInterface CurrentState { get; }
        string CurrentStateName { get; }
        TSharedContext Context { get; }
    }
}