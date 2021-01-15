using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxHole : MonoBehaviour
{

    [SerializeField]
    private FoxHole destinationFoxhole = null;
    private bool isActivated = true;

    /*
     * Pseudocode - Teleport player from foxhole A to B
     *      
     *      1. Check if player has walked onto foxhole
     *      2. if 1. = true, change player position = destinationFoxhole's position + Vector3.up
     *      
     *      
     *      - Translate player's position to destination foxhole position
     *      - 
     * 
     */

    public bool GetIsActivated()
    {
        return isActivated;
    }
    public void SetActivated(bool activate)
    {
        this.isActivated = activate;
    }

    private void Update()
    {
        //CheckIfPlayerWalkedOntoFoxhole();

    }


    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag.Equals("Player") 
            && destinationFoxhole != null 
            && destinationFoxhole.GetIsActivated())
        {
            Debug.Log("Player Entered " + this.gameObject.name);
            GameObject player = other.gameObject;
            TurnBasedCharacter playerTBC = player.GetComponent<TurnBasedCharacter>();

            if (!playerTBC.GetIsMoving()) // Check if the player has stopped moving before teleporting
            {
                destinationFoxhole.WarpToMe(player);
                this.isActivated = false;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && destinationFoxhole != null)
        {
            destinationFoxhole.SetActivated(true);
        }
    }


    //Called by the other paired foxhole to warp the target object to this tile
    public void WarpToMe(GameObject objectToWarp)
    {
        if (objectToWarp.tag.Equals("Player") && isActivated)
        {
            Vector3 destinationPosition = this.gameObject.transform.position + Vector3.up;
            
            objectToWarp.GetComponent<TurnBasedCharacter>().SetTargetMoveToPosition(destinationPosition);
            objectToWarp.transform.position = destinationPosition;
            
            isActivated = false;
        }
    }
}
