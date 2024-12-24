using UnityEngine;

public class IdleState : IState
{
    private PlayerController controller;

    public IdleState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        Debug.Log("Entering Idle State");
        controller.HideBubble();
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
