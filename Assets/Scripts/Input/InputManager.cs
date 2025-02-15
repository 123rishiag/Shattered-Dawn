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

        inputControls.Player.Movement.performed += ctx => GetPlayerMovement = ctx.ReadValue<Vector2>();
        inputControls.Player.Movement.canceled += ctx => GetPlayerMovement = Vector2.zero;

        inputControls.Player.Aim.performed += ctx => GetPlayerAimPosition = ctx.ReadValue<Vector2>();
        inputControls.Player.Aim.canceled += ctx => GetPlayerAimPosition = Vector2.zero;

        inputControls.Player.Shoot.started += ctx => IsPlayerShooting = true;
        inputControls.Player.Shoot.canceled += ctx => IsPlayerShooting = false;
    }

    private void OnDisable()
    {
        inputControls.Player.Movement.performed -= ctx => GetPlayerMovement = ctx.ReadValue<Vector2>();
        inputControls.Player.Movement.canceled -= ctx => GetPlayerMovement = Vector2.zero;

        inputControls.Player.Aim.performed -= ctx => GetPlayerAimPosition = ctx.ReadValue<Vector2>();
        inputControls.Player.Aim.canceled -= ctx => GetPlayerAimPosition = Vector2.zero;

        inputControls.Player.Shoot.started -= ctx => IsPlayerShooting = true;
        inputControls.Player.Shoot.canceled -= ctx => IsPlayerShooting = false;

        inputControls.Disable();
    }
    public bool IsPlayerShooting { get; private set; }
    public Vector2 GetPlayerMovement { get; private set; }
    public Vector2 GetPlayerAimPosition { get; private set; }
}
