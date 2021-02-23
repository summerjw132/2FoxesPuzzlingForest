using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxHole : MonoBehaviour
{

    [SerializeField]
    private FoxHole destinationFoxhole = null;

    private WarningMessagesController warnController = null;
    private static string coveredWarning = "The exit of this Fox Hole is covered by something!";

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

    private void Start()
    {
        warnController = GameObject.Find("UI Canvas").GetComponent<WarningMessagesController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && destinationFoxhole != null) 
        {
            GameObject player = other.gameObject;
            TurnBasedCharacter playerTBC = player.GetComponent<TurnBasedCharacter>();

            StartCoroutine(WarpToMeAfterMoving(player, playerTBC));
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

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
            warnController.Warn(coveredWarning);
        }
    }

    public IEnumerator WarpToMeAfterMoving(GameObject player, TurnBasedCharacter TBC)
    {
        //This check makes sure the fox doesn't start TPing back and forth, only the time when
        // it actually walks onto the foxhole will trigger anything.
        if (TBC.GetIsMoving())
        {
            while (TBC.GetIsMoving())
            {
                yield return null;
            }
            destinationFoxhole.WarpToMe(player);
        }
    }
}
