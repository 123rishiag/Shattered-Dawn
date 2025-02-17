using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Inspector Elements
    [Header("Input Manager")]
    [SerializeField] private InputManager inputManager;
    [Space]
    [Header("Camera Variables")]
    [SerializeField] private Transform followTarget;

    [SerializeField] private bool invertHorizontalMouse;
    [SerializeField] private bool invertVerticalMouse;

    [SerializeField] private float distanceToFollow;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float upVerticalAngle;
    [SerializeField] private float downVerticalAngle;

    [SerializeField] private Vector2 framingOffset;

    // Private Variables
    private float invertHorizontalMouseVal;
    private float invertVerticalMouseVal;

    private float rotationX;
    private float rotationY;

    private void Start()
    {
        invertHorizontalMouseVal = invertHorizontalMouse ? -1 : 1;
        invertVerticalMouseVal = invertVerticalMouse ? -1 : 1;
    }

    private void Update()
    {
        CalculateCameraRotation();
        UpdateCameraPosition();
    }

    private void CalculateCameraRotation()
    {
        // Adjusting rotations based on input
        rotationX += inputManager.GetCameraLookDelta.y * invertVerticalMouseVal * rotationSpeed * Time.deltaTime * 100f;
        rotationX = Mathf.Clamp(rotationX, upVerticalAngle, downVerticalAngle);

        rotationY += inputManager.GetCameraLookDelta.x * invertHorizontalMouseVal * rotationSpeed * Time.deltaTime * 100f;
    }

    private void UpdateCameraPosition()
    {
        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        Vector3 focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.position = focusPosition - targetRotation * Vector3.forward * distanceToFollow;
        transform.rotation = targetRotation;
    }

    public Quaternion GetCameraPlanerRotation() => Quaternion.Euler(0f, rotationY, 0f);
}
