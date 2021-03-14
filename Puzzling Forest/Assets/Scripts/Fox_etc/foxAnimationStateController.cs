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
    //integer hashes of the parameters in the animator for optimization
    int isWalkingHash;
    int isPushingHash;
    int isTurningLeft;
    int isTurningRight;
    int isWarpingHash;
    int isPassing;
    int isCatching;
    //Any durations given in float that may be needed
    private float turnDurationLeft;
    private float turnDurationRight;
    private float warpAnimDuration;

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
        isWarpingHash = Animator.StringToHash("isWarping");
        isPassing = Animator.StringToHash("Pass");
        isCatching = Animator.StringToHash("Catch");

        GetDurations();
    }

    void Update()
    {
        
    }

    //Goes into the runtime animator controller and calculates the duration in seconds (float) that each anim takes.
    private void GetDurations()
    {
        float speedMultiplier; 
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                //feel free to add more cases if you need more clip times...
                case "TurnLeft":
                    speedMultiplier = anim.GetFloat("turn_speed");
                    turnDurationLeft = clip.length / speedMultiplier;
                    break;

                case "TurnRight":
                    speedMultiplier = anim.GetFloat("turn_speed");
                    turnDurationRight = clip.length / speedMultiplier;
                    break;

                case "Fox_Dive":
                    speedMultiplier = anim.GetFloat("warp_speed");
                    warpAnimDuration = clip.length / speedMultiplier;
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
    //The foxhole warp animation. Returns the duration of the animation for the coroutine in foxhole
    public float diveIntoFoxhole()
    {
        anim.SetTrigger(isWarpingHash);
        return warpAnimDuration;
    }

    public void startPassTheBall()
    {
        anim.SetTrigger(isPassing);
    }

    public void startCatchTheBall()
    {
        anim.SetTrigger(isCatching);
    }
}