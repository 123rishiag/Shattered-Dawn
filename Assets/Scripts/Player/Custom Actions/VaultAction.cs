using UnityEngine;

[CreateAssetMenu(fileName = "VaultParkourAction", menuName = "Scriptable Objects/VaultParkourAction")]
public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(ObstacleHitData _hitData, Transform _player)
    {
        if (!base.CheckIfPossible(_hitData, _player))
            return false;

        var hitPoint = _hitData.forwardHit.transform.InverseTransformPoint(_hitData.forwardHit.point);

        if(hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
        {
            Mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }
        else
        {
            Mirror = true;
            matchBodyPart = AvatarTarget.RightHand;
        }
        return true;
    }
}
