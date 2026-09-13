using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed of the player movement in meters per second.")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform; // Assign your Main Camera here in the Inspector
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float minPitch = -80f; // Limit looking straight down
    [SerializeField] private float maxPitch = 80f;  // Limit looking straight up

    private float xRotation = 0f;
    private bool isCursorLocked = true;

    void Start()
    {
        SetCursorLock(true);
    }

    void Update()
    {
        // Toggle cursor lock state when pressing Left Control
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetCursorLock(!isCursorLocked);
        }

        // Only handle rotation and movement when the cursor is locked
        if (isCursorLocked)
        {
            HandleMouseLook();
        }

        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // 1. Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Rotate camera Up/Down (X-axis pitch) and clamp to avoid flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // 3. Rotate player body Left/Right (Y-axis yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D
        float verticalInput = Input.GetAxis("Vertical");     // W/S

        float upDownInput = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            upDownInput += 1f; // Up
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            upDownInput -= 1f; // Down
        }

        Vector3 direction = new Vector3(horizontalInput, upDownInput, verticalInput);

        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }

        // Translate relative to player's current facing direction
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void SetCursorLock(bool lockState)
    {
        isCursorLocked = lockState;
        Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockState;
    }
}