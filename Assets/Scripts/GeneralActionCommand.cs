using UnityEngine.Assertions;

public class GeneralActionCommand : Command
{
    PlayerController controller;
    string text;

    public GeneralActionCommand(CommandExecutor executor, string text = "")
        : base(executor)
    {
        controller = executor.GetComponent<PlayerController>();
        Assert.IsNotNull(controller);
        this.text = text;
    }

    public override void StartCommand()
    {
        controller.CharActionStateMachine.generalActionState.Init(this.text);
        controller.CharActionStateMachine.TransitionTo(controller.CharActionStateMachine.generalActionState);
    }

    public override bool CheckIfFinished()
    {
        return controller.CharActionStateMachine.CurrentState == controller.CharActionStateMachine.generalActionState;
    }

    public override bool CheckIfSuccessful()
    {
        return CheckIfFinished();
    }

    public override void OnFinished() 
    {
    }
}
