using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Inspector Elements
    [Header("Input Manager")]
    [SerializeField] private InputManager inputManager;
    [Space]
    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravityForce;
    [Space]
    [Header("Aim Variables")]
    [SerializeField] private Transform aimObject;
    [SerializeField] private LayerMask aimLayerMask;

    // Private Variables
    private CharacterController characterController;

    private Vector3 movementDirection;
    private float verticalVelocity;
    private Vector3 aimDirection;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        SetMovement();
        SetAim();
    }

    private void SetMovement()
    {
        movementDirection = new Vector3(inputManager.GetPlayerMovement.x, 0f, inputManager.GetPlayerMovement.y);

        SetGravity();

        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void SetGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityForce * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }

    private void SetAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(inputManager.GetPlayerAimPosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            aimDirection = hitInfo.point - transform.position;
            aimDirection.y = 0f;
            aimDirection.Normalize();

            transform.forward = aimDirection;

            aimObject.position = 
                new Vector3(hitInfo.point.x, transform.position.y + characterController.height - .2f, hitInfo.point.z);
        }
    }
}
