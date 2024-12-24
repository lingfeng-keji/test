using UnityEngine;

public class GeneralActionState : IState
{
    private PlayerController controller;
    private string text;

    public GeneralActionState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Init(string text = "")
    {
        this.text = text;
    }

    public void Enter()
    {
        Debug.Log("Entering General Action State");
        if (text != string.Empty)
        {
            controller.ShowBubble(text);
        }   
    }

    public void Exit()
    {
        controller.HideBubble();
        Debug.Log("Exiting General Action State");
    }
}
