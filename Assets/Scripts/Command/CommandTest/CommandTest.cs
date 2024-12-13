using System.Collections.Generic;
using UnityEngine;

public class CommandTest : Singleton<CommandTest>
{
    public CommandExecutor playerExecutor;

    private int rows = 2;
    private int columns = 2;
    private float spacing = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int goCount = 4;

        List<GameObject> goList = new List<GameObject>();

        for (int i = 0; i < goCount; ++i)
        {
            GameObject go = new GameObject($"Test_{i}");
            go.transform.position = transform.position;
            go.transform.parent = transform;

            CommandExecutor executor = go.AddComponent<CommandExecutor>();

            // Maybe can use factory pattern to generate commands.
            float t = Random.Range(2.0f, 4.0f);
            Command sleepCommand = new SleepCommand(executor, t);
            sleepCommand.EnqueueSelf();

            t = Random.Range(2.0f, 4.0f);
            Command eatCommand = new EatCommand(executor, t, 0.5f);
            eatCommand.EnqueueSelf();

            t = Random.Range(2.0f, 4.0f);
            sleepCommand = new SleepCommand(executor, t, 0.5f);
            sleepCommand.EnqueueSelf();

            // Add a SpriteRenderer component
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();

            // Assign a default sprite (e.g., white square) from Unity's built-in sprites
            renderer.sprite = CreateSquareSprite();

            // Set the square's size
            go.transform.localScale = new Vector3(48, 48, 1);

            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Renderer-sortingOrder.html
            renderer.sortingOrder = 1;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            go.AddComponent<Selection>();

            goList.Add(go);
            
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;

                Vector3 position = new Vector3(col * spacing, row * spacing, 0);
                goList[index].transform.position = position;
            }
        }

        if (playerExecutor != null)
        {
            var locationA = LocationManager.Instance.NameToLocationDict["AAA"];
            var moveToCommand = new MoveToCommand(playerExecutor, locationA.transform.position);
            moveToCommand.EnqueueSelf();

            Command sleepCommand = new SleepCommand(playerExecutor, 4.0f);
            sleepCommand.EnqueueSelf();

            var locationB = LocationManager.Instance.NameToLocationDict["BBB"];
            moveToCommand = new MoveToCommand(playerExecutor, locationB.transform.position);
            moveToCommand.EnqueueSelf();
        }
    }

    // Function to create a simple white square sprite
    Sprite CreateSquareSprite()
    {
        // Create a 2x2 texture
        Texture2D texture = new Texture2D(2, 2);
        texture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        texture.Apply();

        // Create the sprite using the texture
        return Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class SleepCommand : Command
{
    private float sleepTime;
    private float sleepTimer;
    private float randomValueSuccess;
    private bool success;
    private bool finished;
    private float successRate;

    public SleepCommand(CommandExecutor executor, float time, float successRate = 1.0f)
        : base(executor)
    {
        sleepTime = time;
        this.successRate = successRate;
    }
    public override void StartCommand()
    {
        sleepTimer = sleepTime;
        finished = false;
        success = false;
        Debug.Log($"{Executor.name} start sleeping! (Will take {sleepTime} seconds)");
        randomValueSuccess = Random.Range(0.0f, 1.0f);
    }
    public override void UpdateCommand(float deltaTime)
    {
        if (Executor == null)
        {
            Debug.Log($"There is no game objects!");
            finished = true;
            success = false;
            return;
        }

        if (randomValueSuccess <= successRate)
        {
            sleepTimer -= deltaTime;

            if (sleepTimer <= 0)
            {
                Debug.Log($"{Executor.name} sleep success!");
                finished = true;
                success = true;
            }
        }
        else
        {
            finished = true;
            success = false;
            Debug.Log($"{Executor.name} eating fails!");
        }

    }
    public override bool IsFinished()
    {
        return finished;
    }
    public override bool IsSuccessful()
    {
        return success;
    }
    public override void Undo()
    {

    }
}
public class EatCommand : Command
{
    private float eatTime;
    private float eatTimer;
    private float randomValueSuccess; // maybe later let gpt refactor name
    private bool success;
    private bool finished;
    float successRate;

    public EatCommand(CommandExecutor executor, float time, float successRate = 1.0f)
        : base(executor)
    {
        eatTime = time;
        this.successRate = Mathf.Clamp(successRate, 0.0f, 1.0f);
    }
    public override void StartCommand()
    {
        eatTimer = eatTime;
        finished = false;
        success = false;
        Debug.Log($"{Executor.name} start eating! (Will take {eatTime} seconds)");
        randomValueSuccess = Random.Range(0.0f, 1.0f);
    }
    public override void UpdateCommand(float deltaTime)
    {
        if (Executor == null)
        {
            Debug.Log($"Executor doesn't exist!");
            finished = true;
            success = false;
            return;
        }

        if (randomValueSuccess <= successRate)
        {
            eatTimer -= deltaTime;

            if (eatTimer <= 0)
            {
                Debug.Log($"{Executor.name} eating succeeds!");
                finished = true;
                success = true;
            }
        }
        else
        {
            finished = true;
            success = false;
            Debug.Log($"{Executor.name} eating fails!");
        }
    }
    public override bool IsFinished()
    {
        return finished;
    }
    public override bool IsSuccessful()
    {
        return success;
    }
    public override void Undo()
    {

    }
}