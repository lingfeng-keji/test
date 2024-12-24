using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSleepButtonClick()
    {
        // is current selected specific type?
        var currentSelected = SelectionManager.Instance.currentSelected;
        if (currentSelected != null)
        {
            CommandExecutor executor = null;
            //PlayerController controller = null;
            if (currentSelected.selectedType == SelectionManager.SelectedType.Character)
            {
                executor = currentSelected.transform.parent.GetComponent<CommandExecutor>();
                //controller = currentSelected.transform.parent.GetComponent<PlayerController>();

                var homeLocation = LocationManager.Instance.NameToLocationDict["Home"];
                Command command = new MoveToCommand(executor, homeLocation.transform.position);
                command.EnqueueSelf();

                command = new GeneralActionCommand(executor, "Sleep");
                command.EnqueueSelf();
            }

        }
    }
}
