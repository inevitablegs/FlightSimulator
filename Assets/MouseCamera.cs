using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    public Transform target;      // Plane
    public float mouseSensitivity = 200f;
    public float distance = 8f;
    public float height = 3f;

    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -35f, 60f);

        // Include the target's rotation so the camera bank/roll matches the plane
        Quaternion rotation = target.rotation * Quaternion.Euler(xRotation, yRotation, 0);

        // Calculate offset position from the rotated plane's perspective
        Vector3 position = target.position - rotation * Vector3.forward * distance + target.up * height;

        transform.position = position;
        // Make the camera look at the target, but keep its 'up' vector aligned with the plane's 'up'
        transform.LookAt(target, target.up);
    }
}