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
    //Move speed. Used like Vector3.MoveTowards(... , moveSpeed * Time.deltaTime)
    //  original value was 5f
    private float moveSpeed = 2.5f;

    //animation stuff
    // animation controller script
    protected foxAnimationStateController animController;
    // flags for knowing which type of animation to call each move
    private bool thisMoveIsAPush;
    private bool thisMoveIsAWalk;
    // bool to lock controls away from player while animation is going
    private bool isAnimating = false;

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

        //If this is a fox, get a reference to the animation controller class which handles
        // rotating the fox(es) and also triggering the animations
        if (characterType == CharacterType.Player)
        {
            animController = GetComponent<foxAnimationStateController>();
        }

        //Find the turn manager in game; use it to
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
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
        if (this.transform.position != targetMoveToPosition)
        {
            //This is the only way a fox can move ^
            if (characterType == CharacterType.Player)
            {
                //One of these flags was set when the Fox was deciding if it is OkayToMoveToNextTile depending
                // on whether or not it moved into open space or into a wall.
                if (thisMoveIsAWalk)
                {
                    //Triggers the corresponding animation
                    animController.startWalking();
                    //Reset the flag in this class so a new decision can be made for the next move
                    setWalkFlagFalse();
                }
                else if (thisMoveIsAPush)
                {
                    animController.startPushing();
                    setPushFlagFalse();
                }
            }
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetMoveToPosition, moveSpeed * Time.deltaTime);
            isMoving = true;

            //Log whenever a non-player is moving
            //if (!this.gameObject.tag.Equals("Player") && this.gameObject.transform.position.y >= 0)
            //{
            //    Debug.Log(this.gameObject.name + ": I'm moving");
            //}

            if (turn.isTurn)
            {
                //These msgs are super loud bc they print every update
                //Debug.Log(this.gameObject.name + ": I'm moving");
            }
        }
        else
        {
            isMoving = false;

            if (turn.isTurn)
            {
                //Debug.Log(this.gameObject.name + ": I'm standing still");
            }
        }

    }

    private void Fall()
    {
        if (!FloorIsPresent(this.transform.position) && !isMoving)
        {
            targetMoveToPosition = this.transform.position + Vector3.down;
        }
    }


    public abstract void SpecialAction();


    private void UpdateTurnForPlayer()
    {
        //Deactivate controls if character isMoving from point to point or if an animation is going
        if (!isMoving && !isAnimating)
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
                            //This call simply points the Fox in the new direction
                            animController.faceNorth();
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
                            animController.faceSouth();
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
                            animController.faceWest();
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
                            animController.faceEast();
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
            if (NoWallIsPresent(nextTilePosition, this.gameObject.tag.Equals("Player"))) //There is no blocking wall 
            {
                //This move is into open space therefore it is a walk, not a push
                setWalkFlagTrue();
                return true;
            }
            else
            {
                GameObject wall = Physics.OverlapSphere(nextTilePosition, .1f)[0].gameObject;
                PushableTurnBasedObject pushableWall = wall.GetComponent<PushableTurnBasedObject>();

                if (pushableWall != null)
                {

                    if (this.gameObject.tag.Equals("Player") || pushableWall.IsStackPushingEnabled()) // If this object is a player OR (if not a player, and) the pushable object has stack pushing
                    {
                        if (pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f))
                        {
                            //This move is into a wall therefore it is a push, not a walk
                            setPushFlagTrue();
                            return true;
                        }
                        else
                            return false;
                    }

                }
            }
        } else if (!this.gameObject.tag.Equals("Player"))//This ensures that we can push off an edge (no floor)
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

    //These functions are setters for various flags. Basically, when a Fox is
    // deciding if it is OkayToMoveToNextTile, depending on whether or not the
    // fox is moving or pushing, the corresponding flag will be set. Then,
    // once the update function actually sets the fox moving, depending on which
    // flag is set, the corresponding animation is started using the animController
    // class and the flag is unset before the next move.
    public void setPushFlagTrue()
    {
        thisMoveIsAPush = true;
    }
    public void setPushFlagFalse()
    {
        thisMoveIsAPush = false;
    }
    public void setWalkFlagTrue()
    {
        thisMoveIsAWalk = true;
    }
    public void setWalkFlagFalse()
    {
        thisMoveIsAWalk = false;
    }

    //These are just wrapper functions to set/reset an "isAnimating" flag.
    // The animations themselves call these functions using animation events.
    // While the flag is set, user input is not accepted.
    public void beginAnimation()
    {
        isAnimating = true;
    }
    public void completeAnimation()
    {
        isAnimating = false;
    }
}
