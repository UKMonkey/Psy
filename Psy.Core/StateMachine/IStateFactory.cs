namespace Psy.Core.StateMachine
{
    public interface IStateFactory<out TStateInterface, in TSharedContext> 
        where TStateInterface : IState<TSharedContext>
        where TSharedContext : class
    {
        TStateInterface Create(TSharedContext context);
    }
}