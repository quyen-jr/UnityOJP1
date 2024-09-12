using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;
    private float mouseSensivity = 2f;
    private float cameraVerticalRotation = 0f;
    private Rigidbody rb => GetComponentInParent<Rigidbody>();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity;

        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        Quaternion verticalRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);

        transform.localRotation = verticalRotation;

        Quaternion rotationDelta = Quaternion.Euler(0, mouseX, 0);
        rb.MoveRotation(rb.rotation * rotationDelta);
    }
}
