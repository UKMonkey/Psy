using System.Collections.Generic;

namespace Psy.Core.StateMachine
{
    public class StateMachine<TStateInterface, TSharedContext> : 
        IStateMachine<TStateInterface, TSharedContext> 
        where TStateInterface : class, IState<TSharedContext>
        where TSharedContext : class
    {
        private readonly Dictionary<string, IStateFactory<TStateInterface, TSharedContext>> _stateFactories;
        public TStateInterface CurrentState { get; private set; }

        public string CurrentStateName { get; private set; }

        public TSharedContext Context { get; private set; }

        public StateMachine(TSharedContext context)
        {
            Context = context;
            _stateFactories = new Dictionary<string, IStateFactory<TStateInterface, TSharedContext>>();
        }

        public void TransitionTo(string stateName)
        {
            if (CurrentState != null)
                CurrentState.OnTransitionOut();
            CurrentState = _stateFactories[stateName].Create(Context);
            CurrentStateName = stateName;
            CurrentState.OnTransitionIn();
        }

        public void RegisterState(string stateName, IStateFactory<TStateInterface, TSharedContext> factory)
        {
            _stateFactories[stateName] = factory;
        }
    }
}