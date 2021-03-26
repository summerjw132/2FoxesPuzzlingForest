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
    int isTurningAroundLeft;
    int isTurningAroundRight;
    int isWarpingHash;
    int isPassing;
    int isCatching;
    int walkSpeedHash;
    int pushSpeedHash;
    int turnSpeedHash;
    int warpSpeedHash;
    int freezeStateHash;
    //Any durations given in float that may be needed
    private float walkDuration;
    private float pushDuration;
    private float turnDurationLeft;
    private float turnDurationRight;
    private float warpAnimDuration;

    //sfx stuff
    private AudioSource warpNoise;

    void Awake()
    {
        //initialize variables
        anim = GetComponent<Animator>();
        foxTransform = this.gameObject.transform.Find("Fox");

        isWalkingHash = Animator.StringToHash("isWalking");
        isPushingHash = Animator.StringToHash("isPushing");
        isTurningLeft = Animator.StringToHash("isTurningLeft");
        isTurningRight = Animator.StringToHash("isTurningRight");
        isTurningAroundLeft = Animator.StringToHash("isTurningAroundLeft");
        isTurningAroundRight = Animator.StringToHash("isTurningAroundRight");
        isWarpingHash = Animator.StringToHash("isWarping");
        isPassing = Animator.StringToHash("Pass");
        isCatching = Animator.StringToHash("Catch");
        walkSpeedHash = Animator.StringToHash("walk_speed");
        pushSpeedHash = Animator.StringToHash("push_speed");
        turnSpeedHash = Animator.StringToHash("turn_speed");
        warpSpeedHash = Animator.StringToHash("warp_speed");
        freezeStateHash = Animator.StringToHash("freezeState");

        warpNoise = GameObject.Find("Audio Manager").transform.Find("Warp").GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(DelayStartup());
    }

    void Update()
    {
        
    }

    private void ShowDurations()
    {
        Debug.LogFormat("walkDuration: {0}", walkDuration);
        Debug.LogFormat("pushDuration: {0}", pushDuration);
        Debug.LogFormat("turnDurationLeft: {0}", turnDurationLeft);
        Debug.LogFormat("turnDurationRight: {0}", turnDurationRight);
        Debug.LogFormat("warpAnimDuration: {0}", warpAnimDuration);
    }

    public void UpdateDuration(float TTM)
    {
        //walking animation is 0.5f all natural
        float speed = 0.5f / TTM;
        anim.SetFloat(walkSpeedHash, speed);
        //push animation is 1f naturally
        speed = 1f / TTM;
        anim.SetFloat(pushSpeedHash, speed);
        //turning animations are 1f naturally
        anim.SetFloat(turnSpeedHash, speed);
    }

    private IEnumerator DelayStartup()
    {
        yield return new WaitForSeconds(0.05f);
        GetDurations();
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
                case "Fox_Run_InPlace_With_Events":
                    speedMultiplier = anim.GetFloat(walkSpeedHash);
                    walkDuration = clip.length / speedMultiplier;
                    break;

                case "Fox_Somersault_InPlace_With_Events":
                    speedMultiplier = anim.GetFloat("push_speed");
                    pushDuration = clip.length / speedMultiplier;
                    break;

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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fox_Walk_InPlace"))
            return;
        anim.SetTrigger(isWalkingHash);
    }
    public void startPushing()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fox_Somersault_InPlace"))
            return;
        anim.SetTrigger(isPushingHash);
    }
    //Same trigger setup for the turning animation. Also makes use of the CoRoutine "TurnSmoothly"
    // in order to actually rotate the fox at the same rate as the animation dictates.
    public void startTurningLeft(Quaternion curRotation)
    {
        targetRotation = curRotation * Quaternion.AngleAxis(-90, Vector3.up);
        anim.SetTrigger(isTurningLeft);
        StartCoroutine(TurnSmoothly(curRotation, targetRotation, turnDurationLeft));
        
    }
    public void startTurningRight(Quaternion curRotation)
    {
        targetRotation = curRotation * Quaternion.AngleAxis(90, Vector3.up);
        anim.SetTrigger(isTurningRight);
        StartCoroutine(TurnSmoothly(curRotation, targetRotation, turnDurationRight));
    }
    public void startTurningAround(Quaternion curRotation)
    {
        float randChoice = Random.value;
        if (randChoice >= 0.55)
        {
            targetRotation = curRotation * Quaternion.AngleAxis(180, Vector3.up);
            anim.SetTrigger(isTurningAroundLeft);
            StartCoroutine(TurnSmoothly(curRotation, targetRotation, turnDurationLeft * 2));
        }
        else
        {
            targetRotation = curRotation * Quaternion.AngleAxis(-180, Vector3.up);
            anim.SetTrigger(isTurningAroundRight);
            StartCoroutine(TurnSmoothly(curRotation, targetRotation, turnDurationRight * 2));
        }
    }
    //The foxhole warp animation. Returns the duration of the animation for the coroutine in foxhole
    public float diveIntoFoxhole()
    {
        anim.SetTrigger(isWarpingHash);
        warpNoise.PlayDelayed(warpAnimDuration / 2f);
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

    public void FreezeState(bool val)
    {
        anim.SetBool(freezeStateHash, val);
    }

    public float GetWalkDuration()
    {
        return walkDuration;
    }

    public float GetPushDuration()
    {
        return pushDuration;
    }
}