using UnityEngine;

public class AtomSpawner : MonoBehaviour
{
    [Header("Setup References")]
    [Tooltip("Drag your SilverAtom Prefab here")]
    [SerializeField] private GameObject atomPrefab;

    [Tooltip("Drag the Furnace GameObject here")]
    [SerializeField] private Transform furnaceTransform;

    [Header("Spawn Offset Settings")]
    [Tooltip("Local offset from furnace origin to aperture hole")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0.00001f, 0.00001f, 0.00001f);

    [Tooltip("Additional rotation offset added to furnace rotation (X=90)")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(-90f, 0f, 180f);

    [Header("Spawner Timing")]
    [SerializeField] private float spawnInterval = 0.2f;
    [SerializeField] private bool autoSpawn = true;

    private float timer;

    void Update()
    {
        if (!autoSpawn) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAtom();
            timer = 0f;
        }
    }

    public void SpawnAtom()
    {
        if (atomPrefab == null || furnaceTransform == null)
        {
            Debug.LogWarning("AtomSpawner: Missing Prefab or Furnace Transform reference!");
            return;
        }

        // Calculate world position based on furnace local coordinates
        Vector3 spawnPosition = furnaceTransform.position + furnaceTransform.TransformDirection(spawnOffset);

        // Apply furnace rotation + 90 degrees offset on local X axis
        Quaternion spawnRotation =  Quaternion.Euler(rotationOffset);

        // Instantiate the atom prefab with corrected orientation
        Instantiate(atomPrefab, transform.position + new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0), transform.rotation);
    }
}