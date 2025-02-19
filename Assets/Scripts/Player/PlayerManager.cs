using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Inspector Elements
    [Header("Input Manager")]
    [SerializeField] private InputManager inputManager;
    [Header("Camera Manager")]
    [SerializeField] private CameraManager cameraManager;
    [Space]
    [Header("Gizmos Variables")]
    [SerializeField] private bool shouldDrawGizmos;
    [Space]
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityForce;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask groundLayerMask;
    [Space]
    [Header("Aim Variables")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private LayerMask aimLayerMask;

    // Private Variables
    private CharacterController characterController;
    private Animator playerAnimator;

    private PlayerMoveState playerMoveState;

    private Vector3 movementDirection;
    private float verticalVelocity;
    private Vector3 aimDirection;

    private bool hasControl = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerMoveState = PlayerMoveState.IDLE;
    }

    private void Update()
    {
        SetPlayerState();

        if (hasControl)
        {
            SetMovement();
            SetAim();
            SetAnimation();
        }
    }

    // Setters
    private void SetPlayerState()
    {
        SetPlayerMoveState();
        SetPlayerCombatState();
    }
    private void SetPlayerMoveState()
    {
        if (!IsGrounded())
        {
            playerMoveState = PlayerMoveState.FALL;
        }
        else if (inputManager.GetPlayerMovement.magnitude > 0)
        {
            if (inputManager.IsPlayerRunning)
            {
                playerMoveState = PlayerMoveState.RUN;
            }
            else
            {
                playerMoveState = PlayerMoveState.WALK;
            }
        }
        else
        {
            playerMoveState = PlayerMoveState.IDLE;
        }
    }
    private void SetPlayerCombatState()
    {

    }

    private void SetMovement()
    {
        movementDirection = new Vector3(inputManager.GetPlayerMovement.x, movementDirection.y, inputManager.GetPlayerMovement.y);
        SetGravity();
        movementDirection = cameraManager.GetCameraPlanerRotation() * movementDirection;

        if (playerMoveState == PlayerMoveState.RUN)
        {
            characterController.Move(movementDirection * runSpeed * Time.deltaTime);
        }
        else if (playerMoveState == PlayerMoveState.WALK)
        {
            characterController.Move(movementDirection * walkSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(movementDirection * Time.deltaTime);
        }
    }
    private void SetGravity()
    {
        if (playerMoveState == PlayerMoveState.FALL)
        {
            verticalVelocity -= gravityForce * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
        movementDirection.y = verticalVelocity;
    }

    private void SetAim()
    {
        if (inputManager.IsPlayerAiming)
        {
            aimTransform.gameObject.SetActive(true);

            Ray ray = Camera.main.ScreenPointToRay(inputManager.GetPlayerAimPosition);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
            {
                aimDirection = hitInfo.point - transform.position;

                aimTransform.position = new Vector3(
                    hitInfo.point.x,
                    transform.position.y + characterController.height - 0.2f,
                    hitInfo.point.z
                    );
            }
            else
            {
                aimDirection = Camera.main.transform.forward;
            }

            SetPlayerAimDirection();
        }
        else
        {
            aimTransform.gameObject.SetActive(false);
            aimDirection = movementDirection.magnitude == 0f ? transform.forward : movementDirection;

            SetPlayerAimDirection();
        }
    }

    private void SetPlayerAimDirection()
    {
        aimDirection.y = 0f;
        aimDirection.Normalize();

        if (aimDirection.magnitude == 0f)
        {
            return;
        }
        transform.forward = aimDirection;
    }

    private void SetAnimation()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        playerAnimator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        string newAnimation = GetAnimationTrigger();

        if (!IsCurrentAnimation(newAnimation))
        {
            playerAnimator.CrossFade(newAnimation, 0.1f);
        }
    }


    // Getters
    private bool IsCurrentAnimation(string _animationName)
    {
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(_animationName) || playerAnimator.IsInTransition(0);
    }

    private string GetAnimationTrigger()
    {
        switch (playerMoveState)
        {
            case PlayerMoveState.RUN:
                return "Run";
            case PlayerMoveState.WALK:
                return "Walk";
            default:
                return "Idle";
        }
    }

    private bool IsGrounded() => Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayerMask);

    // Gizmos
    private void OnDrawGizmos()
    {
        if (shouldDrawGizmos)
        {
            DrawGroundGizmos();
        }
    }
    private void DrawGroundGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public void SetControl(bool _hasControl)
    {
        hasControl = _hasControl;
        characterController.enabled = _hasControl;
    }
}