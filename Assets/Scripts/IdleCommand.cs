using UnityEngine.Assertions;

public class IdleCommand : Command
{
    PlayerController controller;

    public IdleCommand(CommandExecutor executor)
        : base(executor)
    {
        controller = executor.GetComponent<PlayerController>();
        Assert.IsNotNull(controller);
    }

    public override void StartCommand()
    {
        controller.CharActionStateMachine.TransitionTo(controller.CharActionStateMachine.idleState);
    }

    public override bool CheckIfFinished()
    {
        return controller.CharActionStateMachine.CurrentState == controller.CharActionStateMachine.idleState;
    }

    public override bool CheckIfSuccessful()
    {
        return CheckIfFinished();
    }

    public override void OnFinished()
    {
    }
}