using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class derives from the FairyController class. 
///  THAT class is attached to every Summer/Fairy that is attached to the fox prefab.
///  THIS class is attached to Summers/Fairies that are used in tutorial levels and are NOT attached to foxes.
/// 
/// This version doesn't do anything too crazy. It's set up to work with the Tutorial System where each fairy
///  is attached to a DriftAnchor invisible game object. This script makes the tutorial fairies drift around
///  that point randomly to give Summer some life. 
///  
/// This script also contains two methods (FlyOff, FlyDown) for making it look like Summer flies off/back onto screen
///  when it is time to move to the next dialogue location or return to the normal Summer that is on the foxes.
/// </summary>
public class TutFairyController : FairyController
{
    //random drift/float effect
    protected float rand_x;
    protected float rand_y;
    protected float rand_z;
    protected bool isFlyingOff = false;
    protected bool isDrifting = false;
    protected float driftSpeed = 0.5f;
    protected float range = 0.2f;
    protected Transform driftAnchor;
    protected Vector3 centerPos;
    protected Vector3 randPos;
    protected bool goInactive = false;
    protected bool isFinding = false;
    protected Coroutine driftingRoutine = null;

    protected override void Awake()
    {
        foxAnim = transform.parent.GetComponent<foxAnimationStateController>();
        fox = transform.parent.GetComponent<FoxCharacter>();
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
            
        driftAnchor = this.gameObject.transform.parent.transform.parent.transform.Find("DriftAnchor");

        typingPause = new WaitForSeconds(typingSpeed);
        puncPause = new WaitForSeconds(puncSpeed);
        speechPause = new WaitForSeconds(speechPauseDuration);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        driftingRoutine = StartCoroutine(DriftAround());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This section deals with giving Summer the effect of randomly drifting around during the tutorial exposition.
    private IEnumerator DriftAround()
    {
        while (true)
        {
            DriftAroundFunc();
            yield return null; //waits one frame
        }
    }

    private void DriftAroundFunc()
    {
        if (isDrifting)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, randPos, driftSpeed * Time.deltaTime);
            if (this.transform.position == randPos)
            {
                isDrifting = false;
                if (goInactive)
                    this.gameObject.transform.parent.gameObject.SetActive(false);
            }

        }
        else
        {
            StartCoroutine(DelayFindRandomPosition());
        }
    }

    private IEnumerator DelayFindRandomPosition()
    {
        if (isFinding)
        {
            yield break;
        }
        isFinding = true;
        yield return new WaitForSeconds(0.1f);
        FindRandomPosition();
    }

    private void FindRandomPosition()
    {
        if (isFlyingOff)
            return;
        centerPos = driftAnchor.position;

        rand_x = centerPos.x + Random.Range(-range, range);
        rand_y = centerPos.y + Random.Range(-range, range);
        rand_z = centerPos.z + Random.Range(-range, range);

        randPos = new Vector3(rand_x, rand_y, rand_z);

        isDrifting = true;
        driftSpeed = 0.5f;

        isFinding = false;
    }

    //These two methods deal with the animation used for moving on to the next dialogue location.
    public void FlyOff()
    {
        isFlyingOff = true;
        randPos = new Vector3(driftAnchor.position.x, driftAnchor.position.y + 12, driftAnchor.position.z);
        driftSpeed = 15f;
        isDrifting = true;
        StopCoroutine(DelayFindRandomPosition());
        goInactive = true;
    }

    public void FlyDown()
    {
        isFlyingOff = false;
        goInactive = false;
        centerPos = driftAnchor.position;
        this.transform.position = new Vector3(centerPos.x, centerPos.y + 12, centerPos.z);
        randPos = centerPos;
        isDrifting = true;
        driftSpeed = 15f;
    }
}
