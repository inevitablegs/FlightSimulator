using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;   // Plane
    public float distance = 8f;
    public float sensitivity = 3f;

    float mouseX;
    float mouseY;

    void LateUpdate()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity;

        mouseY = Mathf.Clamp(mouseY, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

        Vector3 position = target.position - rotation * Vector3.forward * distance;

        transform.position = position;
        transform.LookAt(target);
    }
}