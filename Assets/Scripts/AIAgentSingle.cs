using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AIAgentSingle : Singleton<AIAgentSingle>
{
    int worldHour = 1;

    public CommandExecutor executor;
    string playerName = null;
    string lastAction = null;

    // 请求的 URL
    const string url = "http://192.168.31.151:8087/get_plan";

    PlayerController playerController = null;

    //UnityWebRequest request = null;

    protected override void Awake()
    {
        base.Awake();

        // 必须在Awake或Start new这个对象
        //request = new UnityWebRequest(url, "POST");
    }

    void Start()
    {
        playerController = executor.GetComponent<PlayerController>();
    }

    void Update()
    {
        
    }

    public void OnNextButtonClick()
    {
        Debug.Log($"Current World Hour is {worldHour}");

        // 如果所有玩家都执行玩当前小时的行动，才生效
        if (executor.commandQueue.Count != 0)
        {
            return;
        }

        StartCoroutine(SendPostRequest());
    }

    IEnumerator SendPostRequest()
    {
        // 构造请求数据
        string jsonData = $"{{\"world_id\":\"1d05a315-d6cd-4419-999d-26aa2a3c5f02\", \"day\":\"0\",\"hour\":{worldHour}}}";

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

        bool shouldContinue = false;
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            shouldContinue = true;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

        if (shouldContinue)
        {
            string json = request.downloadHandler.text;

            List<PlayerAction> actions = JsonConvert.DeserializeObject<List<PlayerAction>>(json);

            string actionName = null;
            if (actions != null && actions.Count > 0)
            {
                if (playerName == null)
                {
                    playerName = actions[0].playerName;
                    playerController.playerName = playerName;
                }

                foreach (PlayerAction action in actions)
                {
                    if (action.playerName == playerName)
                    {
                        actionName = action.actionName;
                        break;
                    }
                }
            }

            Debug.Log($"Player Name: {playerName}");
            Debug.Log($"Action Name: {actionName}");

            if (actionName != lastAction)
            {
                if (actionName == "休息")
                {
                    var homeLocation = LocationManager.Instance.NameToLocationDict["Home"];
                    Command command = new MoveToCommand(executor, homeLocation.transform.position);
                    command.EnqueueSelf();

                    command = new GeneralActionCommand(executor, "Rest");
                    command.EnqueueSelf();
                }
                if (actionName == "学习")
                {
                    var homeLocation = LocationManager.Instance.NameToLocationDict["Flowers"];
                    Command command = new MoveToCommand(executor, homeLocation.transform.position);
                    command.EnqueueSelf();

                    command = new GeneralActionCommand(executor, "Study");
                    command.EnqueueSelf();
                }

                lastAction = actionName;
            }

            ++worldHour;
        }
    }


    public class PlayerAction
    {
        [JsonProperty("action_name")]
        public string actionName;

        [JsonProperty("player_name")]
        public string playerName;

        [JsonProperty("world_hour")]
        public int worldHour;
    }
}
