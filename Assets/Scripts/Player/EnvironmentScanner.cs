using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] private float forwardRayLength = 0.8f;
    [SerializeField] private float heightRayLength = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();

        Vector3 forwardOrigin = transform.position + Vector3.up * characterController.height * 0.2f;
        Vector3 forwardDirection = transform.forward;

        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, forwardDirection,
            out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, forwardDirection * forwardRayLength, hitData.forwardHitFound ? Color.red : Color.white);

        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            var heightDirection = Vector3.down;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, heightDirection,
            out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, heightDirection * heightRayLength, hitData.heightHitFound ? Color.red : Color.white);
        }

        return hitData;
    }
}

public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}
