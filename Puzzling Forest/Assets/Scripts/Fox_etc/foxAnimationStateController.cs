using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles turning the fox and triggering the push/walk animations
//  It should be noted that the fxns are called from the TurnBasedCharacter script

//  The only animation-related code not in this class handles arresting control
//   during animations. The animations themselves use animation events to set
//   and unset a flag "isAnimating" that can be found in the TurnBasedCharacter class.

public class foxAnimationStateController : MonoBehaviour
{
    //the Animator object attached to both foxes
    private Animator anim;
    //the Transform of the Fox (child of this.gameObject.transform)
    private Transform foxTransform;
    //this quaternion is used when the foxes turn
    private Quaternion targetRotation;
    //this float corresponds to the time of the turning animation
    private float turnDurationLeft;
    private float turnDurationRight;
    //integer hashes of the parameters in the animator for optimization
    int isWalkingHash;
    int isPushingHash;
    int isTurningLeft;
    int isTurningRight;

    void Awake()
    {
        //initialize variables
        anim = GetComponent<Animator>();
        foxTransform = this.gameObject.transform.Find("Fox");
    }

    void Start()
    {
        isWalkingHash = Animator.StringToHash("isWalking");
        isPushingHash = Animator.StringToHash("isPushing");
        isTurningLeft = Animator.StringToHash("isTurningLeft");
        isTurningRight = Animator.StringToHash("isTurningRight");

        GetTurnDuration();
    }

    void Update()
    {
        
    }

    private void GetTurnDuration()
    {
        float speedMultiplier = anim.GetFloat("turn_speed");
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                //feel free to add more cases if you need more clip times...
                case "TurnLeft":
                    turnDurationLeft = clip.length / speedMultiplier;
                    break;

                case "TurnRight":
                    turnDurationRight = clip.length / speedMultiplier;
                    break;

                default:
                    break;
            }
        }
    }

    //This is a co-routine to make turning take the same amount of time as the turning animation!
    private IEnumerator TurnSmoothly(Quaternion startAngle, Quaternion targetAngle, float duration)
    {
        float curDuration = 0f;
        while (curDuration <= duration)
        {
            curDuration += Time.deltaTime;
            foxTransform.rotation = Quaternion.Lerp(startAngle, targetAngle, curDuration / duration);
            yield return null;
        }
    }

    //Setter functions for the triggers controlling the walk/push animations
    //Triggers are like booleans but automatically reset after the anim completes
    public void startWalking()
    {
        anim.SetTrigger(isWalkingHash);
    }
    public void startPushing()
    {
        anim.SetTrigger(isPushingHash);
    }
    //Same trigger setup for the turning animation. Also makes use of the CoRoutine "TurnSmoothly"
    // in order to actually rotate the fox at the same rate as the animation dictates.
    public void startTurningLeft(Quaternion curRotation)
    {
        targetRotation = curRotation * Quaternion.AngleAxis(-90, Vector3.up);
        StartCoroutine(TurnSmoothly(curRotation, targetRotation, turnDurationLeft));
        anim.SetTrigger(isTurningLeft);
    }
    public void startTurningRight(Quaternion curRotation)
    {
        targetRotation = curRotation * Quaternion.AngleAxis(90, Vector3.up);
        StartCoroutine(TurnSmoothly(curRotation, targetRotation, turnDurationRight));
        anim.SetTrigger(isTurningRight);
    }
}