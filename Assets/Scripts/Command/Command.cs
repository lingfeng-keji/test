using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// interface to wrap your actions in a "command object"
public abstract class Command
{
    public CommandExecutor Executor { get; }

    public Command(CommandExecutor executor)
    {
        Executor = executor;
    }

    public void EnqueueSelf()
    {
        Assert.IsNotNull(Executor);
        Executor.EnqueueCommand(this);
    }

    public abstract void StartCommand();             // Begin the command
    public abstract void UpdateCommand(float deltaTime);            // Update command logic in FixedUpdate
    public abstract bool IsFinished();        // Check if the command is complete
    public abstract bool IsSuccessful();      // Check if the command was successful
    public abstract void Undo();              // Optional: Implement undo logic

    // 也许之后要加失败原因
}
