using UnityEngine;

public class DebugCameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of camera movement

    void Update()
    {
        // Get input from arrow keys or WASD
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float vertical = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontal, vertical, 0);

        // Move the camera
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}