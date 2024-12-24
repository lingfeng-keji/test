using System.Collections.Generic;
using UnityEngine;

public class CommandManager : Singleton<CommandManager>
{
    // later change to map
    private HashSet<CommandExecutor> excutors = new HashSet<CommandExecutor>();

    public void RegisterExecuter(CommandExecutor executor)
    {
        // 也许Set更好？
        if (!excutors.Contains(executor))
            excutors.Add(executor);
    }

    public void UnregisterExecuter(CommandExecutor executor)
    {
        excutors.Remove(executor);
    }

    public void UpdateAll(float deltaTime)
    {
        foreach (var executor in excutors)
        {
            executor.UpdateCommands(deltaTime);
        }
    }

    public void FixedUpdate()
    {
        UpdateAll(Time.fixedDeltaTime);
    }

    void OnDestroy()
    {
        //excutors.Clear();
    }
}
