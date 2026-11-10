using UnityEngine;
using UnityEngine.InputSystem;

public class camera_movement : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 0.1f;
    Transform playerBody;

    private InputAction lookAction;
    private float xRotation = 0f;

    private void Start()
    {
        playerBody = transform.parent;
        lookAction = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
