using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class TurnBasedCharacter : MonoBehaviour
{

    public TurnManager turnManager;
    public TurnManager.TurnInstance turn;
    public bool isTurn = false;
    //public KeyCode moveKey;
    [SerializeField]
    private int maxMovementRange = 5;
    [SerializeField]
    private CharacterType characterType = CharacterType.Player;
    [SerializeField]
    private GameObject turnIndicator = null;
    private int currentMovementRemaining;
    private bool isMoving = false;
    protected Vector3 targetMoveToPosition;
    

    public enum CharacterType
    {
        Player,
        NPC,
        Wall
    }

    public CharacterType GetCharacterType()
    {
        return characterType;
    }


    void Start()
    {
        targetMoveToPosition = this.transform.position;

        //Find the turn manager in game; use it to
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
        for(int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            if (this.gameObject.transform.GetChild(i).gameObject.name.Equals("turnIndicator"))
            {

                turnIndicator = this.gameObject.transform.GetChild(i).gameObject;
                break;
            }
        }

        //Set the turn value to that established by the turn manager
        foreach (TurnManager.TurnInstance currentTurn in turnManager.playersGroup)
        {
            if (currentTurn.playerGameObject.name == gameObject.name)
            {
                turn = currentTurn;
            }
        }
        ResetMovement();

    }

    // Update is called once per frame
    void Update()
    {
        if (characterType == CharacterType.Player)
        {
            UpdateTurnForPlayer();

        }
        if (characterType == CharacterType.NPC)
        {
            UpdateTurnForNPC();

        }

        UpdateTurnIndicator();

        //MoveCharacter();
        Fall();

        //If new position is ever updated (via player input or other external factors), move the character to the new position
        if(this.transform.position != targetMoveToPosition)
        {

            this.transform.position = Vector3.MoveTowards(this.transform.position, targetMoveToPosition, 5f * Time.deltaTime);
            isMoving = true;
            
            if (turn.isTurn)
            {
                Debug.Log(this.gameObject.name + ": I'm moving");
            }
        }
        else
        {
            isMoving = false;

            if (turn.isTurn)
            {
                Debug.Log(this.gameObject.name + ": I'm standing still");
            }
        }

    }

    private void Fall()
    {
        if(!FloorIsPresent(this.transform.position) && !isMoving)
        {
            targetMoveToPosition = this.transform.position + Vector3.down;
        }
    }


    public abstract void SpecialAction();


    //
    private void UpdateTurnForPlayer()
    {
        if (!isMoving)//Deactivate controls if character isMoving from point to point
        {

            isTurn = turn.isTurn;

            if (isTurn && turn.isEnabled)
            {
                if (currentMovementRemaining > 0)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        //transform.position += Vector3.right;
                        Vector3 currentPosition = transform.position;
                        if (OkayToMoveToNextTile(currentPosition + Vector3.forward))
                        {

                            currentMovementRemaining--;

                            targetMoveToPosition = currentPosition + Vector3.forward;
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }


                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        //transform.position += Vector3.right;
                        //Vector3 currentPosition = transform.position;
                        //targetMoveToPosition = currentPosition + Vector3.back;

                        //currentMovementRemaining--;

                        //Debug.Log("Current Position: " + currentPosition);
                        //Debug.Log("New Position: " + targetMoveToPosition);

                        //transform.position += Vector3.right;
                        Vector3 currentPosition = transform.position;
                        if (OkayToMoveToNextTile(currentPosition + Vector3.back))
                        {

                            currentMovementRemaining--;

                            targetMoveToPosition = currentPosition + Vector3.back;
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        //transform.position += Vector3.right;
                        //Vector3 currentPosition = transform.position;
                        //targetMoveToPosition = currentPosition + Vector3.left;

                        //currentMovementRemaining--;

                        //Debug.Log("Current Position: " + currentPosition);
                        //Debug.Log("New Position: " + targetMoveToPosition);

                        //transform.position += Vector3.right;
                        Vector3 currentPosition = transform.position;
                        if (OkayToMoveToNextTile(currentPosition + Vector3.left))
                        {

                            currentMovementRemaining--;

                            targetMoveToPosition = currentPosition + Vector3.left;
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }

                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        //transform.position += Vector3.right;
                        //Vector3 currentPosition = transform.position;
                        //targetMoveToPosition = currentPosition + Vector3.right;

                        //currentMovementRemaining--;

                        //Debug.Log("Current Position: " + currentPosition);
                        //Debug.Log("New Position: " + targetMoveToPosition);

                        //transform.position += Vector3.right;
                        Vector3 currentPosition = transform.position;
                        if (OkayToMoveToNextTile(currentPosition + Vector3.right))
                        {

                            currentMovementRemaining--;

                            targetMoveToPosition = currentPosition + Vector3.right;
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }

                    }
                }
                else
                {
                    isTurn = false;
                    turn.isTurn = isTurn;
                    turn.wasTurnPrev = true;
                }
            }
            else if (!turn.isEnabled)
            {
                isTurn = false;
                turn.isTurn = isTurn;
                turn.wasTurnPrev = true;
            }
        }
    }

    //Check if it is OK to move to the next
    protected bool OkayToMoveToNextTile(Vector3 nextTilePosition)
    {
        //Check Walls
        //Collider[] wallHitColliders = Physics.OverlapSphere(nextTilePosition, .1f);
        //Collider[] floorHitCollider = Physics.OverlapSphere(nextTilePosition + Vector3.down, .1f);
        
        if (FloorIsPresent(nextTilePosition)) //There is a floor
        {
            if(NoWallIsPresent(nextTilePosition)) //There is no blocking wall 
            {
                return true;
            }
            else
            {

                GameObject wall = Physics.OverlapSphere(nextTilePosition, .1f)[0].gameObject;
                PushableTurnBasedObject pushableWall = wall.GetComponent<PushableTurnBasedObject>();
                if(pushableWall != null)
                {
                    return pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f);
                }
            }
        }
        //else //A wall is found
        //{
        //    GameObject wall = Physics.OverlapSphere(nextTilePosition, .1f)[0].gameObject;
        //    PushableTurnBasedObject pushableWall = wall.GetComponent<PushableTurnBasedObject>();

        //    pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f);
        //}
        
        //Either there isa wall, or there is no floor to walk on
        //Debug.Log("Wall Present: " + (wallHitColliders[0].gameObject.name ));
        //Debug.Log("Floor present: " + (floorHitCollider[0].gameObject.name));
        
        return false;
        
    }

    private bool NoWallIsPresent(Vector3 nextTilePosition)
    {
        Collider[] wallHitColliders = Physics.OverlapSphere(nextTilePosition, .1f);//1 is purely chosen arbitrarly

        return wallHitColliders.Length == 0 || wallHitColliders[0].gameObject.GetComponent<BoxCollider>().isTrigger;
    }
    private bool FloorIsPresent(Vector3 nextTilePosition)
    {
        Collider[] floorHitCollider = Physics.OverlapSphere(nextTilePosition + Vector3.down, .1f);

        return floorHitCollider.Length > 0;
    }

    private void UpdateTurnForNPC()
    {
        isTurn = turn.isTurn;

        if (isTurn)
        {
            StartCoroutine("WaitAndMove");
        }
    }

    IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(1f);
        transform.position += Vector3.forward;
        isTurn = false;
        turn.isTurn = isTurn;
        turn.wasTurnPrev = true;

        StopCoroutine("WaitAndMove");

    }
    public void ResetMovement()
    {

        currentMovementRemaining = maxMovementRange;
    }

    //This updates the turn indicator to active if it is this character's turn
    private void UpdateTurnIndicator()
    {
        if(turnIndicator != null)
        {
            turnIndicator.SetActive(isTurn);
        }
    }
}
