using System;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterActionStateMachine
{
    public IState CurrentState { get; private set; }

    // reference to the state objects
    public IdleState idleState;
    public GeneralActionState generalActionState;

    // event to notify other objects of the state change
    //public event Action<IState> stateChanged;

    // pass in necessary parameters into constructor 
    public CharacterActionStateMachine(PlayerController controller)
    {
        // create an instance for each state and pass in PlayerController
        generalActionState = new GeneralActionState(controller);
        idleState = new IdleState(controller);
    }

    // set the starting state
    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();

        // notify other objects that state has changed
        //stateChanged?.Invoke(state);
    }

    // exit this state and enter another
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

        // notify other objects that state has changed
        //stateChanged?.Invoke(nextState);
    }

    // allow the StateMachine to update this state
    public void Execute()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
    }
}
