using UnityEngine;
using UnityEngine.Assertions;

public class ClickToMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 应该扩展为鼠标交互类
        
        // 之后要改为新的Input System
        // Detect left mouse button click
        if (Input.GetMouseButtonDown(1))
        {
            if (SelectionManager.Instance.currentSelected != null)
            {
                Transform root = SelectionManager.Instance.currentSelected.transform.root;
                PlayerController playerController = root.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    // Get mouse position in world space
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // 2D position
                    Vector3 position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z); // Keep current z position

                    CommandExecutor executor = playerController.GetComponent<CommandExecutor>();

                    var moveToCommand = new MoveToCommand(executor, position);

                    moveToCommand.EnqueueSelf(true);

                    transform.position = position;
                }
            }
        }
    }
}
