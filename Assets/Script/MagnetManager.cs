using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MagnetManager : MonoBehaviour
{
    [Header("Setup References")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject magnetPrefab;
    [SerializeField] private Transform playerTransform;

    [Header("Detector Screen Reference")]
    [SerializeField] private Transform detectorScreen;

    [Header("Atom Configuration Output")]
    public int atomicNumber = 7;
    public float atomSpeed = 1.0f;

    // Internal State
    private readonly List<GameObject> spawnedMagnets = new List<GameObject>();
    private GameObject selectedMagnet;
    private bool isUpdatingUI = false;

    // UI Elements - Magnets
    private Button btnAddMagnet;
    private Button btnDeleteMagnet;
    private DropdownField dropdownMagnets;
    private FloatField inputPosX, inputPosY, inputPosZ;
    private FloatField inputRotX, inputRotY, inputRotZ;

    // UI Elements - Screen Position
    private FloatField screenPosX, screenPosY, screenPosZ;

    // UI Elements - Atom Config
    private IntegerField inputAtomicNumber;
    private FloatField inputAtomSpeed;
    private Button btnUpdateAtom;

    private void OnEnable()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Query Magnet Controls
        btnAddMagnet = root.Q<Button>("btn-add-magnet");
        btnDeleteMagnet = root.Q<Button>("btn-delete-magnet");
        dropdownMagnets = root.Q<DropdownField>("dropdown-magnets");

        inputPosX = root.Q<FloatField>("input-pos-x");
        inputPosY = root.Q<FloatField>("input-pos-y");
        inputPosZ = root.Q<FloatField>("input-pos-z");

        inputRotX = root.Q<FloatField>("input-rot-x");
        inputRotY = root.Q<FloatField>("input-rot-y");
        inputRotZ = root.Q<FloatField>("input-rot-z");

        // Query Screen Position Controls
        screenPosX = root.Q<FloatField>("screen-pos-x");
        screenPosY = root.Q<FloatField>("screen-pos-y");
        screenPosZ = root.Q<FloatField>("screen-pos-z");

        // Query Atom Config Controls
        inputAtomicNumber = root.Q<IntegerField>("input-atomic-number");
        inputAtomSpeed = root.Q<FloatField>("input-atom-speed");
        btnUpdateAtom = root.Q<Button>("btn-update-atom");

        // Event Bindings - Magnets
        btnAddMagnet.clicked += SpawnMagnetAtPlayer;
        btnDeleteMagnet.clicked += DeleteSelectedMagnet;
        dropdownMagnets.RegisterValueChangedCallback(OnDropdownSelectionChanged);

        inputPosX.RegisterValueChangedCallback(_ => OnPositionInputChanged());
        inputPosY.RegisterValueChangedCallback(_ => OnPositionInputChanged());
        inputPosZ.RegisterValueChangedCallback(_ => OnPositionInputChanged());

        inputRotX.RegisterValueChangedCallback(_ => OnRotationInputChanged());
        inputRotY.RegisterValueChangedCallback(_ => OnRotationInputChanged());
        inputRotZ.RegisterValueChangedCallback(_ => OnRotationInputChanged());

        // Event Bindings - Screen Position
        screenPosX.RegisterValueChangedCallback(_ => OnScreenPositionInputChanged());
        screenPosY.RegisterValueChangedCallback(_ => OnScreenPositionInputChanged());
        screenPosZ.RegisterValueChangedCallback(_ => OnScreenPositionInputChanged());

        // Event Bindings - Atom Config
        btnUpdateAtom.clicked += OnUpdateAtomPrefab;

        RefreshDropdown();
        UpdateScreenUIFromTransform();
    }

    private void OnDisable()
    {
        if (btnAddMagnet != null) btnAddMagnet.clicked -= SpawnMagnetAtPlayer;
        if (btnDeleteMagnet != null) btnDeleteMagnet.clicked -= DeleteSelectedMagnet;
        if (btnUpdateAtom != null) btnUpdateAtom.clicked -= OnUpdateAtomPrefab;
    }

    private void SpawnMagnetAtPlayer()
    {
        if (magnetPrefab == null || playerTransform == null)
        {
            Debug.LogWarning("[MagnetManager] Missing Magnet Prefab or Player Transform assignment!");
            return;
        }

        GameObject newMagnet = Instantiate(magnetPrefab, playerTransform.position, playerTransform.rotation);
        newMagnet.name = $"Magnet_{spawnedMagnets.Count + 1}";
        spawnedMagnets.Add(newMagnet);

        RefreshDropdown();
        dropdownMagnets.value = newMagnet.name;
    }

    private void RefreshDropdown()
    {
        List<string> options = new List<string>();
        foreach (var magnet in spawnedMagnets)
        {
            if (magnet != null) options.Add(magnet.name);
        }

        dropdownMagnets.choices = options;

        if (options.Count == 0)
        {
            dropdownMagnets.value = string.Empty;
            selectedMagnet = null;
            ClearTransformFields();
        }
    }

    private void OnDropdownSelectionChanged(ChangeEvent<string> evt)
    {
        int index = dropdownMagnets.choices.IndexOf(evt.newValue);
        if (index >= 0 && index < spawnedMagnets.Count)
        {
            selectedMagnet = spawnedMagnets[index];
            UpdateUIFromSelectedMagnet();
        }
    }

    private void UpdateUIFromSelectedMagnet()
    {
        if (selectedMagnet == null) return;

        isUpdatingUI = true;

        Vector3 pos = selectedMagnet.transform.position;
        Vector3 rot = selectedMagnet.transform.eulerAngles;

        inputPosX.value = pos.x/10f;
        inputPosY.value = pos.y/10f;
        inputPosZ.value = pos.z/10f;

        inputRotX.value = rot.x;
        inputRotY.value = rot.y;
        inputRotZ.value = rot.z;

        isUpdatingUI = false;
    }

    private void OnPositionInputChanged()
    {
        if (isUpdatingUI || selectedMagnet == null) return;
        selectedMagnet.transform.position = new Vector3(inputPosX.value, inputPosY.value, inputPosZ.value);
    }

    private void OnRotationInputChanged()
    {
        if (isUpdatingUI || selectedMagnet == null) return;
        selectedMagnet.transform.eulerAngles = new Vector3(inputRotX.value, inputRotY.value, inputRotZ.value);
    }

    private void DeleteSelectedMagnet()
    {
        if (selectedMagnet == null) return;

        spawnedMagnets.Remove(selectedMagnet);
        Destroy(selectedMagnet);
        RefreshDropdown();
    }

    private void ClearTransformFields()
    {
        isUpdatingUI = true;
        inputPosX.value = inputPosY.value = inputPosZ.value = 0f;
        inputRotX.value = inputRotY.value = inputRotZ.value = 0f;
        isUpdatingUI = false;
    }

    // --- Screen Control Functions ---

    private void UpdateScreenUIFromTransform()
    {
        if (detectorScreen == null) return;

        isUpdatingUI = true;
        Vector3 pos = detectorScreen.position;
        screenPosX.value = pos.x/10f;
        screenPosY.value = pos.y/10f;
        screenPosZ.value = pos.z/10f;
        isUpdatingUI = false;
    }

    private void OnScreenPositionInputChanged()
    {
        if (isUpdatingUI || detectorScreen == null) return;
        detectorScreen.position = new Vector3(screenPosX.value, screenPosY.value, screenPosZ.value);
    }

    // --- Atom Configuration ---

    private void OnUpdateAtomPrefab()
    {
        atomicNumber = inputAtomicNumber.value;
        atomSpeed = inputAtomSpeed.value;

        Debug.Log($"Atom Config Updated: Atomic No = {atomicNumber}, Speed = {atomSpeed}");
    }
}