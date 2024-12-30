using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class AIAgent : Singleton<AIAgent>
{
    int worldHour = 1;
    int day = 0;

    int maxPlayerCount = 4;

    Dictionary<string, PlayerController> playerNameToPlayer = null;
    Dictionary<string, string> playerNameToLastAction = null;
    public GameObject aiCharacterPrefab = null;

    bool inited = false;

    // 请求的 URL
    const string url = "http://192.168.31.151:8087/get_plan";

    //UnityWebRequest request = null;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(aiCharacterPrefab);
        playerNameToPlayer = new Dictionary<string, PlayerController>();
        playerNameToLastAction = new Dictionary<string, string>();
        // 必须在Awake或Start new这个对象
        //request = new UnityWebRequest(url, "POST");
    }

    void Start()
    {
        StartCoroutine(InitRequest());
    }

    void Update()
    {

    }

    IEnumerator InitRequest()
    {
        // 构造请求数据
        string jsonData = "{\"world_id\":\"1d05a315-d6cd-4419-999d-26aa2a3c5f02\",\"hour\":1}";
        Debug.Log(jsonData);

        // 将 JSON 转换为字节数组
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // 创建 UnityWebRequest 对象
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        bool requestSuccess = false;
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            requestSuccess = true;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

        if (requestSuccess)
        {
            string json = request.downloadHandler.text;
            List<PlayerAction> actions = JsonConvert.DeserializeObject<List<PlayerAction>>(json);

            int count = 0;
            Vector3 spawnPos = LocationManager.Instance.NameToLocationDict["PlayerSpawnPoint"].transform.position;

            foreach (PlayerAction action in actions)
            {
                GameObject player = Instantiate(aiCharacterPrefab, spawnPos, Quaternion.identity);

                PlayerController controller = player.GetComponent<PlayerController>();
                controller.playerName = action.playerName;
                playerNameToPlayer.Add(action.playerName, controller);

                ++count;
                if (count == maxPlayerCount)
                {
                    break;
                }
            }

            foreach (KeyValuePair<string, PlayerController> kvp in playerNameToPlayer)
            {
                Debug.Log($"{kvp.Key}");
            }

            inited = true;
        }
    }

    public void OnNextButtonClick()
    {
        if (!inited)
        {
            return;
        }

        Debug.Log($"Current World Hour is {worldHour}");

        bool allCommandsFinished = true;
        foreach (KeyValuePair<string, PlayerController> kvp in playerNameToPlayer)
        {
            var controller = kvp.Value;
            CommandExecutor executor = controller.GetComponent<CommandExecutor>();
            if (executor.commandQueue.Count != 0 )
            {
                allCommandsFinished = false;
                break;
            }
        }

        if (allCommandsFinished)
        {
            StartCoroutine(SendPostRequest());
        }
    }

    IEnumerator SendPostRequest()
    {
        // 构造请求数据
        string jsonData = $"{{\"world_id\":\"1d05a315-d6cd-4419-999d-26aa2a3c5f02\",\"hour\":{worldHour}}}";

        // 将 JSON 转换为字节数组
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // 创建 UnityWebRequest 对象
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 是不是要禁用按钮或者有个判断当前还在响应

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        bool requestSuccess = false;
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            requestSuccess = true;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

        if (requestSuccess)
        {
            string json = request.downloadHandler.text;

            List<PlayerAction> actions = JsonConvert.DeserializeObject<List<PlayerAction>>(json);

            if (actions != null && actions.Count > 0)
            {
                foreach (PlayerAction action in actions)
                {
                    string taskName = action.taskName;
                    string playerName = action.playerName;

                    if (playerNameToPlayer.ContainsKey(playerName))
                    {
                        if (playerNameToLastAction.ContainsKey(playerName))
                        {
                            if (playerNameToLastAction[playerName] != taskName)
                            {
                                PlayerController controller = playerNameToPlayer[playerName];
                                CommandExecutor executor = controller.GetComponent<CommandExecutor>();

                                TaskToCommands(taskName, executor);
                                playerNameToLastAction[playerName] = taskName;
                            }
                        }
                        else
                        {
                            PlayerController controller = playerNameToPlayer[playerName];
                            CommandExecutor executor = controller.GetComponent<CommandExecutor>();

                            TaskToCommands(taskName, executor);
                            playerNameToLastAction.Add(playerName, taskName);
                        }
                    }
                }
            }

            ++worldHour;
        }
    }

    void TaskToCommands(string task, CommandExecutor executor)
    {
        if (task == "休息" || task == "烹饪")
        {
            var location = LocationManager.Instance.NameToLocationDict["Home"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "学习")
        {
            var location = LocationManager.Instance.NameToLocationDict["Home"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "精加工" || task == "熔炼" || task == "粗加工" || task == "塑型")
        {
            var location = LocationManager.Instance.NameToLocationDict["Blacksmith"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "上架" || task == "购买" || task == "下架")
        {
            var location = LocationManager.Instance.NameToLocationDict["Market"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "治疗")
        {
            var location = LocationManager.Instance.NameToLocationDict["Hostpital"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "种植" || task == "采集")
        {
            var location = LocationManager.Instance.NameToLocationDict["Farmland"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "屠宰" )
        {
            var location = LocationManager.Instance.NameToLocationDict["Farm"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else if (task == "挖矿")
        {
            var location = LocationManager.Instance.NameToLocationDict["Mine"];
            Command command = new MoveToCommand(executor, location.transform.position);
            command.EnqueueSelf();

            command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
        else
        {
            Command command = new GeneralActionCommand(executor, task);
            command.EnqueueSelf();
        }
    }


    public class PlayerAction
    {
        [JsonProperty("action_name")]
        public string taskName;

        [JsonProperty("player_name")]
        public string playerName;

        [JsonProperty("world_hour")]
        public int worldHour;
    }
}
