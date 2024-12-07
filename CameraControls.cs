using UnityEngine;
using UnityEngine.UI;

public class CameraControls : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera cam;

    [Header("Bounds Settings")]
    [SerializeField] private Vector2 maxBounds; // Maximum X and Y bounds
    [SerializeField] private Vector2 minBounds; // Minimum X and Y bounds

    [Header("Panning Settings")]
    [SerializeField] private float panSpeed = 10f; // Speed of panning

    [Header("UI Components")]
    [SerializeField] private GameObject returnToPlayerBarracksButton;

    [Header("Drag State")]
    [SerializeField] private Vector3 dragOrigin;

    private void Awake()
    {
        returnToPlayerBarracksButton.GetComponentInChildren<Button>().onClick.AddListener(() => { ReturnToPlayerBarracksPostition(); });
    }

    private void Start()
    {
        if (cam == null) {
            cam = FindAnyObjectByType<Camera>();
        }
    }

    private void Update()
    {
        HandleKeyboardPanning();
        HandleMouseDragging();
        ClampCameraPosition();

        returnToPlayerBarracksButton.SetActive(ShowReturnToPlayerBarracksButton());
    }

    private void HandleKeyboardPanning()
    {
        // Get horizontal input (A/D keys or Left Arrow/Right Arrow)
        float horizontalInput = Input.GetAxis("Horizontal"); // -1 for A/LeftArrow, +1 for D/RightArrow
        if (horizontalInput != 0) {
            Vector3 movement = new Vector3(horizontalInput * panSpeed * Time.deltaTime, 0, 0);
            cam.transform.position += movement;
        }
    }

    private void HandleMouseDragging()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition); // Record the starting position
        }

        if (Input.GetMouseButton(1)) // Right mouse button held
        {
            Vector3 currentMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dragDifference = dragOrigin - currentMousePos;

            cam.transform.position += new Vector3(dragDifference.x, 0, 0); // Apply drag horizontally
        }
    }

    private void ClampCameraPosition()
    {
        // Ensure the camera stays within the bounds
        Vector3 pos = cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        cam.transform.position = pos;
    }

    private bool ShowReturnToPlayerBarracksButton()
    {
        return cam.transform.position.x > -30f;
    }

    private void ReturnToPlayerBarracksPostition()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        cam.transform.position = new Vector3(minBounds.x, 0, -10);
    }
}
