using UnityEngine;

// ENCAPSULATION: look settings are private, tuned in the Inspector
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private Transform cameraPivot;      // drag Main Camera here
    private float xRotation = 0f;                         // current up/down angle

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Look();
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }

    private void Look()
    {
        // No Time.deltaTime: mouse input is already "movement this frame"
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Up/down: tilt only the camera, clamped so you can't flip over
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Left/right: turn the whole player body
        transform.Rotate(Vector3.up * mouseX);
    }
}
