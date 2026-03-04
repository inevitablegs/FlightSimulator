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

        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 position = target.position - rotation * Vector3.forward * distance + Vector3.up * height;

        transform.position = position;
        transform.LookAt(target);
    }
}