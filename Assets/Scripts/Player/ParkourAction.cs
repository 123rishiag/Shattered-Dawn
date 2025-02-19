using UnityEngine;

[CreateAssetMenu(fileName = "ParkourAction", menuName = "Scriptable Objects/ParkourAction")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] public string animName;
    [SerializeField] public string obstacleTag;

    [SerializeField] public float minHeight;
    [SerializeField] public float maxHeight;

    [SerializeField] public bool rotateToObstacle;
    [SerializeField] public float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] public bool enableTargetMatching;
    [SerializeField] public AvatarTarget matchBodyPart;
    [SerializeField] public float matchStartTime;
    [SerializeField] public float matchTargetTime;
    [SerializeField] public Vector3 matchPositionWeight;

    public Quaternion TargetRotation { get; set; }

    public Vector3 MatchPosition { get; set; }

    public bool Mirror { get; set; }

    public virtual bool CheckIfPossible(ObstacleHitData _hitData, Transform _player)
    {
        if(!string.IsNullOrEmpty(obstacleTag) && !_hitData.forwardHit.transform.CompareTag(obstacleTag))
        {
            return false;
        }

        float height = _hitData.heightHit.point.y - _player.position.y;

        if (height < minHeight || height > maxHeight)
        {
            return false;
        }

        if (rotateToObstacle)
        {
            TargetRotation = Quaternion.LookRotation(-_hitData.forwardHit.normal);
        }

        if (enableTargetMatching)
        {
            MatchPosition = _hitData.heightHit.point;
        }

        return true;
    }
}
