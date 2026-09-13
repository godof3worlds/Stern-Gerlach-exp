using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SilverIO : MonoBehaviour
{
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

    // Quantum angular momentum characteristics derived from 'n'
    private float totalJ = 0.5f; // Total angular momentum quantum number J
    private int numStates = 2;   // 2J + 1 distinct deflection paths

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        atomCollider = GetComponent<Collider>();

        // Configure quantum properties based on atomic number 'n'
        ConfigureAtomFromAtomicNumber(n);

        // Disable gravity so atoms travel straight unless deflected
        rb.useGravity = false;

        // Set initial constant forward velocity
        rb.linearVelocity = transform.forward * speed;
    }

    private void ConfigureAtomFromAtomicNumber(int atomicNumber)
    {
        // Group 1 & 11 (Alkali metals, Ag, Au, Cu, H) -> Single valence electron (J = 1/2, 2 beams)
        if (atomicNumber == 1 || atomicNumber == 3 || atomicNumber == 11 || atomicNumber == 19 ||
            atomicNumber == 29 || atomicNumber == 37 || atomicNumber == 47 || atomicNumber == 79)
        {
            totalJ = 0.5f;
        }
        // Noble Gases & Alkaline Earth Metals (He, Ne, Ar, Ca, Hg) -> Closed subshells (J = 0, 1 undeflected beam)
        else if (atomicNumber == 2 || atomicNumber == 4 || atomicNumber == 10 || atomicNumber == 12 ||
                 atomicNumber == 18 || atomicNumber == 20 || atomicNumber == 36 || atomicNumber == 80)
        {
            totalJ = 0f;
        }
        // Group 15 Nitrogen (Z = 7) -> 3 unpaired p-electrons (J = 3/2, 4 beams)
        else if (atomicNumber == 7)
        {
            totalJ = 1.5f;
        }
        // General fallback for unhandled atoms: default to spin-1/2 behavior
        else
        {
            totalJ = 0.5f;
        }

        // Calculate total discrete states (2J + 1)
        numStates = Mathf.RoundToInt(2f * totalJ + 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitScreen) return;

        if (other.CompareTag("Field"))
        {
            // Use the magnet frame's UP direction as the magnetic field gradient axis
            Vector3 fieldAxis = other.transform.up.normalized;

            // Perform quantum measurement based on angle relative to previous axis
            MeasureSpin(fieldAxis);

            // Apply transverse force along or against the magnetic field axis
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

        // Zero out velocity & physical movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Disable collider so subsequent atoms don't collide with this one
        if (atomCollider != null)
        {
            atomCollider.enabled = false;
        }

        // Attach to screen object so it moves if the screen moves
        transform.SetParent(screenTransform);
    }

    private void MeasureSpin(Vector3 currentAxis)
    {
        // If atom has closed shells (J = 0), it experiences no splitting
        if (totalJ == 0f)
        {
            spin = 0f;
            prevAxis = currentAxis;
            return;
        }

        if (prevAxis == Vector3.zero)
        {
            // Unpolarized initial beam: Equal probability across all (2J + 1) states
            int chosenIndex = Random.Range(0, numStates);
            spin = -totalJ + chosenIndex; // Maps index to -J, -J+1, ..., +J
        }
        else
        {
            // Angular measurement transition for spin-1/2 particles
            if (totalJ == 0.5f)
            {
                float angleRad = Vector3.Angle(prevAxis, currentAxis) * Mathf.Deg2Rad;
                float probUp = Mathf.Pow(Mathf.Cos(angleRad / 2.0f), 2.0f);
                spin = (Random.value < probUp) ? 0.5f : -0.5f;
            }
            else
            {
                // Multi-beam random collapse state for non-spin-1/2 atoms
                int chosenIndex = Random.Range(0, numStates);
                spin = -totalJ + chosenIndex;
            }
        }

        // Store current axis for subsequent measurements
        prevAxis = currentAxis;
    }

    private void ApplyDeflection(Vector3 fieldAxis)
    {
        // Normalize spin factor relative to J=0.5 baseline so beam spacing remains consistent
        float normalizedSpinFactor = (totalJ > 0f) ? (spin / totalJ) : 0f;

        // Add velocity perpendicular to travel direction, aligned with the magnet gradient
        Vector3 deflectionForce = fieldAxis * (normalizedSpinFactor * deflectionStrength * speed);

        // Add deflection to existing forward momentum
        rb.linearVelocity += deflectionForce;
    }

    private void Update()
    {
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