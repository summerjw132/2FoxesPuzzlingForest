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
    //used for setting the direction the fox is facing
    private Quaternion rotation;
    //integer hashes of the parameters in the animator for optimization
    int isWalkingHash;
    int isPushingHash;

    void Start()
    {
        //initialize variables
        anim = GetComponent<Animator>();
        foxTransform = this.gameObject.transform.Find("Fox");

        isWalkingHash = Animator.StringToHash("isWalking");
        isPushingHash = Animator.StringToHash("isPushing");
    }

    void Update()
    {
        
    }

    //Turn to face the correct directions -- No Anim as of now
    public void faceNorth()
    {
        rotation = Quaternion.LookRotation(Vector3.forward);
        foxTransform.rotation = rotation;
    }
    public void faceEast()
    {
        rotation = Quaternion.LookRotation(Vector3.right);
        foxTransform.rotation = rotation;
    }
    public void faceSouth()
    {
        rotation = Quaternion.LookRotation(Vector3.back);
        foxTransform.rotation = rotation;
    }
    public void faceWest()
    {
        rotation = Quaternion.LookRotation(Vector3.left);
        foxTransform.rotation = rotation;
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
}