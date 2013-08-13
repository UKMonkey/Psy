namespace Psy.Core.StateMachine
{
    public class StateFactory<TStateInterface, TSharedContext> : 
        IStateFactory<TStateInterface, TSharedContext> 
        where TStateInterface : IState<TSharedContext>, new() 
        where TSharedContext : class
    {
        public TStateInterface Create(TSharedContext context)
        {
            var state = new TStateInterface {Context = context};
            return state;
        }
    }
}