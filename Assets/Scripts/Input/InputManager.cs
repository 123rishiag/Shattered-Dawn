using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputControls inputControls;

    private void Awake()
    {
        inputControls = new InputControls();
    }

    private void OnEnable()
    {
        inputControls.Enable();
    }

    private void OnDisable()
    {
        inputControls.Disable();
    }

    private void Update()
    {
        GetPlayerMovement = inputControls.Player.Movement.ReadValue<Vector2>();
        IsPlayerRunning = inputControls.Player.IsRunning.IsPressed();
        IsPlayerShooting = inputControls.Player.IsShooting.IsPressed();
        IsPlayerAiming = inputControls.Player.IsAiming.IsPressed();
        GetPlayerAimPosition = inputControls.Player.AimPosition.ReadValue<Vector2>();

        GetCameraLookDelta = inputControls.Camera.MouseDelta.ReadValue<Vector2>();
    }

    public Vector2 GetPlayerMovement { get; private set; }
    public bool IsPlayerRunning { get; private set; }
    public bool IsPlayerShooting { get; private set; }
    public bool IsPlayerAiming { get; private set; }
    public Vector2 GetPlayerAimPosition { get; private set; }

    public Vector2 GetCameraLookDelta { get; private set; }
}
