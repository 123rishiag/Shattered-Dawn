using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private List<ParkourAction> parkourActions;

    private PlayerManager playerManager;
    private Animator playerAnimator;
    private EnvironmentScanner environmentScanner;

    private bool inAction;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerAnimator = GetComponent<Animator>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        if (inputManager.IsPlayerRunning && !inAction)
        {
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }
    }


    private IEnumerator DoParkourAction(ParkourAction _action)
    {
        inAction = true;
        playerManager.SetControl(false);

        playerAnimator.SetBool("mirrorAction", _action.Mirror);

        if (_action.rotateToObstacle)
        {
            transform.rotation =  _action.TargetRotation;
        }

        playerAnimator.CrossFade(_action.animName, 0.1f);

        yield return null;

        var animState = playerAnimator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(_action.animName))
        {
            Debug.LogError("Parkour Animation is Wrong!!");
        }

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            if (_action.rotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _action.TargetRotation,
                    cameraManager.GetRotationSpeed() * Time.deltaTime);
            }

            if (_action.enableTargetMatching)
                MatchTarget(_action);

            if (playerAnimator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(_action.postActionDelay);

        playerManager.SetControl(true);
        inAction = false;
    }

    private void MatchTarget(ParkourAction _action)
    {
        if (playerAnimator.isMatchingTarget)
            return;

        playerAnimator.MatchTarget(_action.MatchPosition, transform.rotation, _action.matchBodyPart,
            new MatchTargetWeightMask(_action.matchPositionWeight, 0), _action.matchStartTime, _action.matchTargetTime);
    }
}