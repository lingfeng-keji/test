using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MoveToCommand : Command
{
    PlayerController controller;
    Vector2 position;
    
    public MoveToCommand(CommandExecutor executor, Vector2 pos)
        : base(executor)
    {
        controller = executor.GetComponent<PlayerController>();
        Assert.IsNotNull(controller);
        position = pos;
    }
    public override void StartCommand()
    {
        controller.StartMovingToDest(position);
    }

    public override bool CheckIfFinished()
    {
        return !controller.IsMovingToDest();
    }

    public override bool CheckIfSuccessful()
    {
        return controller.ReachDest();
    }

    public override void Undo()
    {

    }
}
