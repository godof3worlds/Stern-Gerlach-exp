using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SternGerlachHUDController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement cursorDot;
    private VisualElement cursorFollower;
    private Label quantumDriftLabel;
    private Button startButton;
    private Label buttonText;
    private VisualElement buttonIndicator;

    private Vector2 mousePosition;
    private Vector2 followerPosition;
    private bool isHovering = false;
    private bool isBeamActive = false;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        root = uiDocument.rootVisualElement;

        // Query visual elements
        cursorDot = root.Q<VisualElement>("cursor-dot");
        cursorFollower = root.Q<VisualElement>("cursor-follower");
        quantumDriftLabel = root.Q<Label>("quantum-drift");
        startButton = root.Q<Button>("btn-start");
        buttonText = root.Q<Label>("btn-text");
        buttonIndicator = root.Q<VisualElement>("btn-indicator");

        // Event Registrations
        root.RegisterCallback<PointerMoveEvent>(OnPointerMove);

        if (startButton != null)
        {
            startButton.clicked += OnStartButtonClicked;
            RegisterHoverCallbacks(startButton);
        }

        // Register hover triggers for all footer buttons
        var footerButtons = root.Query<Button>(className: "footer-link").ToList();
        foreach (var btn in footerButtons)
        {
            RegisterHoverCallbacks(btn);
        }
    }

    private void Update()
    {
        // Smooth Cursor Follower LERP Interpolation
        float ease = isHovering ? 0.28f : 0.16f;
        followerPosition = Vector2.Lerp(followerPosition, mousePosition, ease);

        if (cursorFollower != null)
        {
            cursorFollower.style.left = followerPosition.x;
            cursorFollower.style.top = followerPosition.y;
        }
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        mousePosition = evt.position;

        // Snap inner dot immediately
        if (cursorDot != null)
        {
            cursorDot.style.left = mousePosition.x;
            cursorDot.style.top = mousePosition.y;
        }

        // Telemetry calculation based on Y position normalization
        float screenHeight = root.layout.height > 0 ? root.layout.height : Screen.height;
        float normY = ((mousePosition.y / screenHeight) - 0.5f) * 2.0f;
        float driftVal = normY * 0.22f;

        if (quantumDriftLabel != null)
        {
            string sign = driftVal >= 0 ? "+" : "";
            quantumDriftLabel.text = $"Δz = {sign}{driftVal:F2} mm";
        }
    }

    private void RegisterHoverCallbacks(VisualElement element)
    {
        element.RegisterCallback<PointerEnterEvent>(e =>
        {
            isHovering = true;
            cursorFollower?.AddToClassList("hovering");
        });

        element.RegisterCallback<PointerLeaveEvent>(e =>
        {
            isHovering = false;
            cursorFollower?.RemoveFromClassList("hovering");
        });
    }

    private void OnStartButtonClicked()
    {
        if (isBeamActive) return;

        isBeamActive = true;
        if (buttonText != null) buttonText.text = "Beam Initialized...";
        if (buttonIndicator != null) buttonIndicator.RemoveFromClassList("hidden");
    }
}