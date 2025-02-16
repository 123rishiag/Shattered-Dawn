using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Inspector Elements
    [Header("Input Manager")]
    [SerializeField] private InputManager inputManager;
    [Space]
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityForce;
    [Space]
    [Header("Aim Variables")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private LayerMask aimLayerMask;

    // Private Variables
    private CharacterController characterController;
    private Animator playerAnimator;

    private PlayerMoveState playerMoveState;
    private PlayerCombatState playerCombatState;

    private Vector3 movementDirection;
    private float verticalVelocity;
    private Vector3 aimDirection;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerMoveState = PlayerMoveState.IDLE;
        playerCombatState = PlayerCombatState.NONE;
    }

    private void Update()
    {
        SetPlayerState();
        SetGravity();
        SetMovement();
        SetAim();
        SetAnimation();
    }

    private void SetPlayerState()
    {
        SetPlayerMoveState();
        SetPlayerCombatState();
    }
    private void SetPlayerMoveState()
    {
        if (inputManager.GetPlayerMovement.magnitude > 0)
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
        else if (!characterController.isGrounded)
        {
            playerMoveState = PlayerMoveState.FALL;
        }
        else
        {
            playerMoveState = PlayerMoveState.IDLE;
        }
    }
    private void SetPlayerCombatState()
    {

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
            verticalVelocity = -2f;
        }
        movementDirection.y = verticalVelocity;
    }
    private void SetMovement()
    {
        movementDirection = new Vector3(inputManager.GetPlayerMovement.x, movementDirection.y, inputManager.GetPlayerMovement.y);

        if (playerMoveState == PlayerMoveState.RUN)
        {
            characterController.Move(movementDirection * runSpeed * Time.deltaTime);
        }
        else if (playerMoveState == PlayerMoveState.WALK)
        {
            characterController.Move(movementDirection * walkSpeed * Time.deltaTime);
        }
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
}