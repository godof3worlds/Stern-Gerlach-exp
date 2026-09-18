using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SilverIO : MonoBehaviour
{
    [SerializeField] private MagnetManager MMRefrence;

    [Header("Element Settings")]
    [Tooltip("Atomic Number (e.g., 47 = Silver, 11 = Sodium, 10 = Neon, 7 = Nitrogen)")]
    [SerializeField] private int n = 47;

    [Header("Quantum Properties")]
    [Tooltip("Current spin state projection along the measured axis")]
    [SerializeField] private float spin = 0f;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float deflectionStrength = 2.0f;

    private float timeE = 0.0f;
    [SerializeField] private float delTime = 15.0f;
    private Rigidbody rb;

    private Collider atomCollider;
    private bool hasHitScreen = false;
    private Vector3 prevAxis = Vector3.zero;

    private int Measured = 0;
    private float totalJ = 0.5f;
    private int numStates = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        atomCollider = GetComponent<Collider>();

        // Auto-assign reference if missing in Inspector
        if (MMRefrence == null)
        {
            MMRefrence = FindFirstObjectByType<MagnetManager>();
        }

        // Fetch values on spawn
        if (MMRefrence != null)
        {
            n = MMRefrence.atomicNumber;
            speed = MMRefrence.atomSpeed;
        }

        ConfigureAtomFromAtomicNumber(n);

        rb.useGravity = false;
        rb.linearVelocity = transform.forward * speed;
    }

    private void ConfigureAtomFromAtomicNumber(int atomicNumber)
    {
        if (atomicNumber == 1 || atomicNumber == 3 || atomicNumber == 11 || atomicNumber == 19 ||
            atomicNumber == 29 || atomicNumber == 37 || atomicNumber == 47 || atomicNumber == 79)
        {
            totalJ = 0.5f;
        }
        else if (atomicNumber == 2 || atomicNumber == 4 || atomicNumber == 10 || atomicNumber == 12 ||
                 atomicNumber == 18 || atomicNumber == 20 || atomicNumber == 36 || atomicNumber == 80)
        {
            totalJ = 0f;
        }
        else if (atomicNumber == 7)
        {
            totalJ = 1.5f;
        }
        else if (atomicNumber == 26 || atomicNumber == 8 || atomicNumber == 16 || atomicNumber == 34 || atomicNumber == 52||atomicNumber == 84 || atomicNumber ==22 || atomicNumber ==40||atomicNumber==72|| atomicNumber == 26)
        {
            totalJ = 2f;
        }
        else
        {
            totalJ = 0.5f;
        }

        numStates = Mathf.RoundToInt(2f * totalJ + 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitScreen) return;

        if (other.CompareTag("Field"))
        {
            Vector3 fieldAxis = other.transform.up.normalized;
            MeasureSpin(fieldAxis);
            ApplyDeflection(fieldAxis);
        }
        else if (other.CompareTag("Screen"))
        {
            StopOnScreen(other.transform);
        }
    }

    private void StopOnScreen(Transform screenTransform)
    {
        hasHitScreen = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (atomCollider != null)
        {
            atomCollider.enabled = false;
        }

        transform.SetParent(screenTransform);
    }

    private void MeasureSpin(Vector3 currentAxis)
    {
        if (totalJ == 0f)
        {
            spin = 0f;
            prevAxis = currentAxis;
            return;
        }

        if (prevAxis == Vector3.zero)
        {
            int chosenIndex = Random.Range(0, numStates);
            spin = -totalJ + chosenIndex;
        }
        else
        {
            float angleDeg = Vector3.Angle(prevAxis, currentAxis);

            if (Mathf.Approximately(angleDeg, 0f) || angleDeg < 0.01f)
            {
                return;
            }

            if (Mathf.Approximately(angleDeg, 180f) || angleDeg > 179.99f)
            {
                spin = -spin;
            }
            else if (totalJ == 0.5f)
            {
                float angleRad = angleDeg * Mathf.Deg2Rad;
                float probUp = Mathf.Pow(Mathf.Cos(angleRad / 2.0f), 2.0f);
                spin = (Random.value < probUp) ? 0.5f : -0.5f;
            }
            else
            {
                int chosenIndex = Random.Range(0, numStates);
                spin = -totalJ + chosenIndex;
            }
        }

        prevAxis = currentAxis;
    }

    private void ApplyDeflection(Vector3 fieldAxis)
    {
        Measured += 1;
        float normalizedSpinFactor = (totalJ > 0f) ? (spin / totalJ) : 0f;
        Vector3 deflectionForce = fieldAxis * (normalizedSpinFactor * deflectionStrength * speed);
        rb.linearVelocity += deflectionForce;
    }

    private void Update()
    {
        // Safe null-check prevents crashes if reference is missing
        if (MMRefrence != null)
        {
            int newN = Mathf.Clamp(MMRefrence.atomicNumber, 1, 118);
            if (newN != n)
            {
                n = newN;
                ConfigureAtomFromAtomicNumber(n);
            }

            speed = MMRefrence.atomSpeed;
        }

        if (hasHitScreen)
        {
            timeE += Time.deltaTime;
            if (timeE > delTime)
            {
                Destroy(gameObject);
            }
        }
    }
}