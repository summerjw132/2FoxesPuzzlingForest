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
    protected int currentMovementRemaining;
    private bool isMoving = false;
    protected Vector3 targetMoveToPosition;
    //animation
    Animator foxAnim;
    Transform foxTransform;
    private bool thisMoveIsAPush = false;
    

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

        if (characterType == CharacterType.Player)
        {
            foxAnim = GetComponent<Animator>();
            for (int i = 0; i < this.gameObject.transform.childCount - 1; i++)
            {
                Debug.Log(this.gameObject.transform.GetChild(i).transform.name);
                if (this.gameObject.transform.GetChild(i).transform.name == "Fox")
                {
                    foxTransform = this.gameObject.transform.GetChild(i).transform;
                }
            }
        }

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
            if (characterType == CharacterType.Player)
            {
                if (thisMoveIsAPush)
                    foxAnim.SetInteger("push", 1);
                else
                    foxAnim.SetInteger("fwd", 1);
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, targetMoveToPosition, 5f * Time.deltaTime);
            isMoving = true;

//<<<<<<< HEAD
//            //Log whenever a non-player is moving
//            //if (!this.gameObject.tag.Equals("Player") && this.gameObject.transform.position.y >= 0)
//            //{
//            //    Debug.Log(this.gameObject.name + ": I'm moving");
//            //}

//            if (turn.isTurn)
//            {
//                //These msgs are super loud bc they print every update
//=======
           
            
            if (turn.isTurn)
            {
//>>>>>>> adding_walking_animation
//                //Debug.Log(this.gameObject.name + ": I'm moving");
            }
        }
        else
        {
            isMoving = false;

            //if it's a fox, idle animation
            if (characterType == CharacterType.Player)
            {
                if (thisMoveIsAPush)
                {
                    foxAnim.SetInteger("push", 0);
                    thisMoveIsAPush = false;
                }
                else
                    foxAnim.SetInteger("fwd", 0);
            }

            if (turn.isTurn)
            {
                //Debug.Log(this.gameObject.name + ": I'm standing still");
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
                turnManager.SetMoveCountUIText(currentMovementRemaining.ToString());//Update the movement count UI text

                if (Input.GetKeyDown(KeyCode.E)) //end the turn if 'E' is pressed
                {
                    currentMovementRemaining = 0;
                }

                if (currentMovementRemaining > 0)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    {
                        //transform.position += Vector3.right;
                        Vector3 currentPosition = transform.position;
                        if (OkayToMoveToNextTile(currentPosition + Vector3.forward))
                        {

                            currentMovementRemaining--;

                            targetMoveToPosition = currentPosition + Vector3.forward;
                            foxTransform.LookAt(new Vector3(0,0,100));
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }


                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
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
                            foxTransform.LookAt(new Vector3(0, 0, -100));
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
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
                            foxTransform.LookAt(new Vector3(-100, 0, 0));
                            //Debug("Current Position: " + currentPosition);
                            //Debug.Log("New Position: " + targetMoveToPosition);
                        }

                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
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
                            foxTransform.LookAt(new Vector3(100, 0, 0));
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
            //Second parameter is whether or not it's the fox trying to move
            if(NoWallIsPresent(nextTilePosition, this.gameObject.tag.Equals("Player"))) //There is no blocking wall 
            {
                return true;
            }
            else
            {
                GameObject wall = Physics.OverlapSphere(nextTilePosition, .1f)[0].gameObject;
                PushableTurnBasedObject pushableWall = wall.GetComponent<PushableTurnBasedObject>();

                if(pushableWall != null)
                {
                 
                    if (this.gameObject.tag.Equals("Player") || pushableWall.IsStackPushingEnabled()) // If this object is a player OR (if not a player, and) the pushable object has stack pushing
                    {
                        if (this.gameObject.tag.Equals("Player"))
                        {
                            //This just uses the result to determine if the fox needs to do a pushing animation
                            return pushAnim(pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f));
                        }
                        else
                            return pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f);
                    }
           
                }
            }
        }else if (!this.gameObject.tag.Equals("Player"))//This ensures that we can push off an edge (no floor)
        {
            return true;
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

    //added second parameter to deal with the hut (okay iff player is the object trying to move)
    protected bool NoWallIsPresent(Vector3 nextTilePosition, bool isPlayer)
    {
        Collider[] wallHitColliders = Physics.OverlapSphere(nextTilePosition, .1f);//1 is purely chosen arbitrarly

        if (wallHitColliders.Length > 0) //there's something here, could be hut or wall etc.
        {
            //if it's the player moving, return true iff the collider belongs to the hut
            if (isPlayer)
            {
                return wallHitColliders[0].gameObject.GetComponent<BoxCollider>().isTrigger;
            }
            else //if it's not the player moving, return false no matter what since a collider is in front
            {
                return false;
            }
        }
        else //no Colliders in front, so wallHitColliders.Length == 0 is true
        {
            return true;
        }
    }

    //Checks to see if there's floor under the given tile
    protected bool FloorIsPresent(Vector3 nextTilePosition)
    {
        //is there a box collider in the tile below the given tile?
        Collider[] floorHitCollider = Physics.OverlapSphere(nextTilePosition + Vector3.down, .1f);

        if (floorHitCollider.Length > 0) //if this array isn't empty, there is a box collider
        {
            //Now we need to make sure the potential floor (owner of box collider) isn't falling before it counts as floor
            GameObject potentialFloor = floorHitCollider[0].gameObject;

            //Can only be falling if it's a pushable wall, so we'll check for that script
            PushableTurnBasedObject pushableScript = potentialFloor.GetComponent<PushableTurnBasedObject>();

            //if pushableScript is NOT null, then the potential floor is a pushable wall, let's check for falling
            if (pushableScript != null)
            {
                //if the floor is moving, that's no good!
                if (pushableScript.isMoving)
                {
                    Debug.Log("That floor is falling!");
                    return false;
                }
                else //floor is stationary, good!
                {
                    return true;
                }
            }
            else //floor isn't pushable and therefore can't be falling so this is also good
            {
                return true;
            }
        }
        //there was no box collider in the first place; no floor
        return false;
    }

    protected bool pushAnim(bool pushed)
    {
        if (pushed)
        {
            thisMoveIsAPush = true;
        }
        return pushed;
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
