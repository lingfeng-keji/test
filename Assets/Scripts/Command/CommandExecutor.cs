using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandExecutor : MonoBehaviour
{
    public Queue<Command> commandQueue = new Queue<Command>();
    public Command activeCommand = null;
    public List<Command> historyCommands = new List<Command>();

    // https://discussions.unity.com/t/order-of-execution-whence-start-after-addcomponent/757064
    private void Start()
    {
        CommandManager.Instance.RegisterExecuter(this);
    }

    public void ResetCommandExecutor()
    {
        activeCommand = null;
        commandQueue.Clear();
        historyCommands.Clear();
    }

    public void CancelCommands()
    {
        activeCommand = null;
        commandQueue.Clear();
    }

    public void EnqueueCommand(Command command)
    {
        commandQueue.Enqueue(command);
    }

    public void UpdateCommands(float deltaTime)
    {
        if (activeCommand == null && commandQueue.Count > 0)
        {
            activeCommand = commandQueue.Dequeue();
            activeCommand.StartCommand();
            historyCommands.Add(activeCommand);
        }

        if (activeCommand != null)
        {
            activeCommand.UpdateCommand(deltaTime);

            if (activeCommand.IsFinished())
            {
                // 要不要加个finish command?

                if (!activeCommand.IsSuccessful())
                {
                    // Clear the queue if a command fails
                    commandQueue.Clear();
                }

                activeCommand = null;
            }
        }
    }

    private void OnDestroy()
    {
        // 这里可能会有问题，也许不应该依赖OnDestroy来进行资源管理
        // CommandManager.Instance.UnregisterExecuter(this);
    }
}
