using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject commandInQueue;
    private List<TMP_Text> commandTexts;
    private GameObject activeCommand;
    private TMP_Text activeCommandText;

    private GameObject characterInfoDisplay;
    private TextMeshProUGUI nameText;

    public GameObject uiActionBubblePrefab;

    // https://discussions.unity.com/t/can-i-should-i-call-awake-in-parent-class-manually/61587
    protected override void Awake()
    {
        base.Awake();
        commandTexts = new List<TMP_Text>();
        activeCommand = FindChild(gameObject, "ActiveCommand");
        commandInQueue = FindChild(gameObject, "CommandInQueue");

        characterInfoDisplay = FindChild(gameObject, "CharacterInfoDisplay");
        nameText = characterInfoDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>();

        characterInfoDisplay.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int commandDisplayCount = 8;
        for (int i = 0; i < commandDisplayCount; i++)
        {
            // https://discussions.unity.com/t/how-do-i-change-a-ui-elements-y-position-in-code/740452
            Vector2 anchoredPos = commandInQueue.GetComponent<RectTransform>().anchoredPosition + (-Vector2.up * 20.0f * i);
            GameObject commandItem = Instantiate(commandInQueue, Vector3.zero, Quaternion.identity, commandInQueue.transform.parent);
            commandItem.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            commandItem.SetActive(true);
            // https://stackoverflow.com/questions/49172311/how-to-get-the-textmeshpro-component
            var commandText = commandItem.GetComponent<TMP_Text>(); //TextMeshProUGUI
            commandText.text = string.Empty;
            commandTexts.Add(commandText);
        }

        Vector2 activeAnchoredPos = activeCommand.GetComponent<RectTransform>().anchoredPosition;
        GameObject activeCommandItem = Instantiate(activeCommand, Vector3.zero, Quaternion.identity, activeCommand.transform.parent);
        activeCommandItem.GetComponent<RectTransform>().anchoredPosition = activeAnchoredPos;
        activeCommandItem.SetActive(true);
        activeCommandText = activeCommandItem.GetComponent<TMP_Text>();
        activeCommandText.text = string.Empty;

        SelectionManager.Instance.Selected += OnSelected;
        SelectionManager.Instance.Deselected += OnDeselected;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var text in commandTexts)
        {
            text.text = string.Empty;
        }

        activeCommandText.text = string.Empty;

        var currentSelected = SelectionManager.Instance.currentSelected;
        if (currentSelected != null)
        {
            CommandExecutor executor = null;
            PlayerController playerController = null;
            if (currentSelected.selectedType == SelectionManager.SelectedType.Character)
            {
                executor = currentSelected.transform.parent.GetComponent<CommandExecutor>();
                playerController = executor.GetComponent<PlayerController>();
            }
            else
            {
                executor = currentSelected.GetComponent<CommandExecutor>();
            }

            if (executor != null)
            {
                var commandQueue = executor.commandQueue;

                int i = 0;
                foreach (var command in commandQueue)
                {
                    // https://code-maze.com/csharp-get-class-name-as-string/
                    var commandName = command.GetType().Name;
                    commandTexts[i].text = commandName;
                    ++i;

                    if (i >= commandTexts.Count)
                    {
                        break;
                    }
                }

                var active = executor.activeCommand;
                if (active != null)
                {
                    var activeName = active.GetType().Name;
                    activeCommandText.text = activeName;
                }
            }

            if (playerController != null)
            {
                nameText.text = $"姓名 {playerController.playerName}";
            }
        }      
    }

    GameObject FindChild(GameObject parent, string childName)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }

            // Recursive search in child's children
            GameObject result = FindChild(child.gameObject, childName);
            if (result != null)
            {
                return result;
            }
        }

        return null; // Return null if not found
    }

    public void OnSelected(Selection selection)
    {
        if (selection.selectedType == SelectionManager.SelectedType.Character)
        {
            characterInfoDisplay.SetActive(true);
        }
    }

    public void OnDeselected(Selection selection)
    {
        if (selection.selectedType == SelectionManager.SelectedType.Character)
        {
            characterInfoDisplay.SetActive(false);
        }
    }
}
