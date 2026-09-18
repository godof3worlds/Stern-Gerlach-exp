using UnityEngine;

public class CinematicOrbitCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The point the camera orbits around. Defaults to (0,0,0) if left empty.")]
    public Transform target;
    public Vector3 targetOffset = Vector3.zero;

    [Header("Orbit Parameters")]
    [Tooltip("Distance from the origin/target.")]
    public float distance = 10.0f;
    [Tooltip("Height relative to the target.")]
    public float height = 3.0f;
    [Tooltip("Time in seconds to complete one 360-degree rotation.")]
    public float rotationDuration = 10.0f;

    [Header("Camera Motion")]
    [Tooltip("Tilt the camera slightly down toward the origin.")]
    public float pitchAngle = 15.0f;
    [Tooltip("Smooth out camera motion.")]
    public bool enableSmoothing = true;
    public float smoothSpeed = 5.0f;

    private float currentAngle = 0.0f;

    void Start()
    {
        // Calculate initial angle based on camera's starting position
        Vector3 focusPoint = GetFocusPoint();
        Vector3 dir = transform.position - focusPoint;
        currentAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    void LateUpdate()
    {
        // Calculate angle progression based on duration
        float speed = 360.0f / Mathf.Max(rotationDuration, 0.1f);
        currentAngle += speed * Time.deltaTime;

        // Calculate orbit position using trigonometry
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 targetPosition = GetFocusPoint();

        Vector3 desiredPosition = new Vector3(
            targetPosition.x + distance * Mathf.Sin(radians),
            targetPosition.y + height,
            targetPosition.z + distance * Mathf.Cos(radians)
        );

        // Apply position with optional smoothing
        if (enableSmoothing)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            transform.position = desiredPosition;
        }

        // Point the camera at the focus point with pitch adjustment
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        targetRotation *= Quaternion.Euler(pitchAngle, 0, 0);

        if (enableSmoothing)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    private Vector3 GetFocusPoint()
    {
        return (target != null ? target.position : Vector3.zero) + targetOffset;
    }
}