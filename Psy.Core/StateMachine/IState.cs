namespace Psy.Core.StateMachine
{
    public interface IState<TSharedContext>
    {
        TSharedContext Context { get; set; }
        void OnTransitionOut();
        void OnTransitionIn();
    }
}