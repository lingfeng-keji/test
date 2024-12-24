using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// interface to wrap your actions in a "command object"
public abstract class Command
{
    protected bool success = false;
    protected bool finished = false;

    public CommandExecutor Executor { get; }

    public Command(CommandExecutor executor)
    {
        Executor = executor;
    }

    public void EnqueueSelf(bool cancelCommandsInQueue = false)
    {
        Assert.IsNotNull(Executor);
        if (cancelCommandsInQueue)
        {
            Executor.CancelCommands();
        }

        Executor.EnqueueCommand(this);
    }

    public abstract void StartCommand();             // Begin the command
    // Update command logic in FixedUpdate
    public virtual void UpdateCommand(float deltaTime)
    {
        finished = CheckIfFinished();

        if (IsFinished())
        {
            success = CheckIfSuccessful(); // 基本只执行一次
        }
    }

    public abstract bool CheckIfFinished();

    public abstract bool CheckIfSuccessful(); // 基本只执行一次

    //也许要加OnCancelCurrentCommand与OnCancelAllCommands
    public virtual void OnFinished() { }

    public virtual void OnSuccess() { }

    public virtual void OnFailure() { }

    public bool IsFinished() { return finished; }        // Check if the command is complete
    public bool IsSuccessful() { return success; }      // Check if the command was successful

    //也许要加个Final，无条件最后执行，回收资源，关闭UI啥的
    public virtual void Undo() { }              // Optional: Implement undo logic

    // 也许之后要加失败原因
}
