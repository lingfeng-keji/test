using UnityEngine;
using UnityEngine.Assertions;

public class ClickToMove : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Assert.IsNotNull(playerController);
    }

    // Update is called once per frame
    void Update()
    {
        // 之后要改为新的Input System
        // Detect left mouse button click
        if (Input.GetMouseButtonDown(1))
        {
            // Get mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 2D position
            Vector3 position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z); // Keep current z position

            // 也许判断下目前是什么模式，或者需要走命令接口
            // 然后必须选择后才能响应用户控制
            playerController.StartMovingToDest(position);
        }
    }
}
