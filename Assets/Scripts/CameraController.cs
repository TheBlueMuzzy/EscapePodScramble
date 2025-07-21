// CameraController.cs
// Attach this script to your Main Camera in the scene to enable vertical pan/drag.
// Configure minY and maxY in the Inspector to clamp between bottom and top of the board.

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 0.5f;      // Adjust drag sensitivity
    public float minY = -30f;          // Lowest Y camera can go
    public float maxY = 0f;            // Highest Y camera can go

    private Vector3 lastPanPosition;
    private bool isPanning = false;

    void Update()
    {
        // Handle mouse drag (including first touch on mobile)
        if (Input.GetMouseButtonDown(0))
        {
            isPanning = true;
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            PanCamera(Input.mousePosition);
        }
    }

    void PanCamera(Vector3 newPanPosition)
    {
        Vector3 delta = newPanPosition - lastPanPosition;
        float moveY = delta.y * panSpeed * Time.deltaTime;

        Vector3 newPos = transform.position + new Vector3(0f, moveY, 0f);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;

        lastPanPosition = newPanPosition;
    }
}
