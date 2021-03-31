using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoxHole : MonoBehaviour
{

    [SerializeField]
    private FoxHole destinationFoxhole = null;

    //stuff to warn the player via UI if the exit hole is blocked
    private WarningMessagesController warnController = null;
    private static string coveredWarning = "The exit of this Fox Hole is covered by something!";

    //stuff to track and affect the game object on top of the foxhole
    private GameObject standingOnMe = null;
    private FoxCharacter playerTBC = null;
    private TurnManager turnManager;

    public UnityEvent onStart, onEnd;
    /*
     * Pseudocode - Teleport player from foxhole A to B
     *      
     *      1. Check if player has walked onto foxhole
     *      2. if 1. = true, tell paired foxhole to warp this fox to itself
     *      3. Other foxhole checks to make sure it's clear
     *      4. If it is clear, warp this fox to itself + up
     *      
     *      - Translate player's position to destination foxhole position
     *      - 
     * 
     */

    private void Awake()
    {
        warnController = GameObject.Find("UI Canvas").GetComponent<WarningMessagesController>();
        turnManager = GameObject.FindGameObjectWithTag("TurnBasedSystem").GetComponent<TurnManager>();
    }

    // Update the vars tracking the GameObject that is on top of this foxhole
    private void OnTriggerEnter(Collider other)
    {
        standingOnMe = other.gameObject;
        
        if (standingOnMe.CompareTag("Player"))
        {
            playerTBC = standingOnMe.GetComponent<FoxCharacter>();

            if (destinationFoxhole)
            {
                playerTBC.ShowFoxholeButton(true, this);
                ToggleEffect(true);
            }
        }
    }

    // Update the vars to reflect that this hole is no longer covered
    private void OnTriggerExit(Collider other)
    {
        ToggleEffect(false);
        playerTBC.ShowFoxholeButton(false, null);
        standingOnMe = null;
        playerTBC = null;
    }

    // Now using Update to check for key input. Blocked behind conditionals to make sure a player is on top
    //  before allowing input
    private void Update()
    {
        //There's something on this foxhole
        if (standingOnMe)
        {
            //There's a player standing on this foxhole
            if (standingOnMe.CompareTag("Player") && playerTBC.isMyTurn)
            {
                if (Input.GetKeyDown(KeyCode.F) && !turnManager.GetKeyJustPressed())
                {
                    turnManager.PressKey();
                    StartCoroutine(InitiateWarp());
                }
            }
        }
    }

    //Starts diving animation then triggers warp once it's finished
    private IEnumerator InitiateWarp()
    {
        if (destinationFoxhole)
        {
            if (destinationFoxhole.CheckIfUncovered())
            {
                if (!playerTBC.isAnimating && !playerTBC.GetIsMoving())
                {
                    playerTBC.WriteFoxholeToUndoStack();
                    yield return new WaitForSeconds(playerTBC.Dive());
                    playerTBC.IncrementMoveCounter();
                    destinationFoxhole.WarpToMe(standingOnMe);
                }
            }
            else
            {
                //warnController.Warn(coveredWarning);
                turnManager.Say(coveredWarning);
            }
        }
    }

    //public method for starting a warp.
    public void InitiateWarpCoroutine()
    {
        StartCoroutine(InitiateWarp());
    }

    //a way to return whether or not the foxhole is blocked before warping to it
    private bool CheckIfUncovered()
    {
        Collider[] potentialCoverings = Physics.OverlapSphere(this.transform.position + Vector3.up, .1f);

        return !(potentialCoverings.Length > 0);
    }

    //Called by the other paired foxhole to warp the target object to this tile
    private void WarpToMe(GameObject objectToWarp)
    {
        if (CheckIfUncovered())
        {
            Vector3 destinationPosition = this.gameObject.transform.position + Vector3.up;
            
            objectToWarp.GetComponent<TurnBasedCharacter>().SetTargetMoveToPosition(destinationPosition);
            objectToWarp.transform.position = destinationPosition;

            Debug.LogFormat("Warping {0} to {1}", objectToWarp.name, this.name);
        }
        else
        {
            //warnController.Warn(coveredWarning);
            turnManager.Say(coveredWarning);
        }
    }

    public void ToggleEffect(bool b)
    {
        if(b)
        {
            destinationFoxhole.onStart?.Invoke();
        }
        else
        {
            destinationFoxhole.onEnd?.Invoke();
        }
    }
}
