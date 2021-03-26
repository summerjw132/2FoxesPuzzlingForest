using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This is the parent class for Foxes and Walls. Everything in here applies to all movable objects.
///  There are children classes FoxCharacter and PushableTurnBasedObject for foxes and walls respectively.
/// </summary>
public abstract class TurnBasedCharacter : MonoBehaviour
{
    //CharacterType do distinguish players and walls
    [SerializeField]
    protected CharacterType characterType = CharacterType.Player;

    //Used for determining when to move and where to
    [Tooltip("How many seconds should it take a fox to move to another tile? Affects walk and push speeds, was 0.5")]
    [SerializeField] protected float SecondsToMove = 0.5f;
    protected bool isMoving = false;
    protected Vector3 targetMoveToPosition;

    //Used for undoing moves, writes positions of each object using undoManager.
    // The bool is used for when to "seal" a set of positions as "one move."
    protected UndoManager undoManager;
    protected bool needToWrite = false;
 
    //Some constants that might need to be tuned later.
    protected float moveSpeed = 2.0f; //Move Speed. Used in Vector3.MoveTowards()
    protected int maxDepth = 20; //How far to check a cliff-fall to see if it will hit anything.

    //UI Warn Message Stuff
    protected WarningMessagesController warnController = null;
    //In the string below, the warning message is cut off immediately after "Twelve" That's your max length
    //                                            "One Two Three Four Five Six Seven Eight Nine Ten Eleven Twelve Thirteen Fourteen";
    private static string vertStackWarnMessage  = "One of the blocks you tried to move is weighed down!";
    private static string crushFoxWarnMessage   = "Be careful! This move would crush a fox!";



    //Awake happens before Start(). Use this as "initialize my stuff" and Start() as "initialize your stuff".
    // Basically, if it is inependent, put it in awake, if it needs something else to be initialized, put that
    //  in start and whatever is needed better be in Awake to be sure it's ready.
    protected virtual void Awake()
    {
        //a reference to the WarningController script that handles UI warning messages
        warnController = GameObject.Find("UI Canvas").GetComponent<WarningMessagesController>();

        //a reference to the UndoManager script that handles undoing stuff (every movable object needs this)
        undoManager = GameObject.Find("GameManager").GetComponent<UndoManager>();
    }

    //This doesn't happen until right before the first call to Update(). Importantly, after Awake()
    protected virtual void Start()
    {
        //so that the objects don't move to 0,0,0 or whatever it defaults to
        targetMoveToPosition = this.transform.position; 
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Fall();

        //If targetPosition is ever updated (via player input or other external factors), move the character to the new position
        if (this.transform.position != targetMoveToPosition)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetMoveToPosition, moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            //this is only ever set to true in FoxCharacters, not TBPOs.
            if (needToWrite)
            {
                needToWrite = false;
                undoManager.WriteTurnState();
            }
            isMoving = false;
        }
    }

    //Update the actual move and animation speeds using the Editor-editable var
    protected abstract void UpdateSpeed();

    //Check if it is OK to move to the next
    protected virtual bool OkayToMoveToNextTile(Vector3 nextTilePosition)
    {
        if (FloorIsPresent(nextTilePosition, out string potentialFloorTag))
        {
            //Check to make sure we're not walking onto a Fox! That's bad for their backs!
            if (potentialFloorTag == "Player")
            {
                warnController.Warn(crushFoxWarnMessage);
                return false;
            }
            //Second parameter is because Foxes don't count houses as walls but rocks do.
            if (NoWallIsPresent(nextTilePosition))
            {
                setThisMove(MoveOptions.Walk);
                return true;
            }
            else //There is a wall of some sort in the potential target position
            {
                GameObject wall = Physics.OverlapSphere(nextTilePosition, .1f)[0].gameObject;
                PushableTurnBasedObject pushableWall = wall.GetComponent<PushableTurnBasedObject>();

                //Check to see if the wall is weighted down and prevent its moving if so
                Collider[] checkForWeightZone = Physics.OverlapSphere(nextTilePosition + Vector3.up, .1f);
                bool weighted = false;
                if (checkForWeightZone.Length > 0)
                    weighted = true;

                //The object is pushable, not a cliff/tree/etc.
                if (pushableWall != null)
                {
                    if (weighted)
                    {
                        warnController.Warn(vertStackWarnMessage);
                        return false;
                    }
                    //Players can alwasy push pushables, pushables can only push others if the other has stack pushing enabled
                    else if (this.gameObject.tag.Equals("Player") || pushableWall.IsStackPushingEnabled())
                    {
                        if (pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, this.gameObject))
                        {
                            setThisMove(MoveOptions.Push);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
        //There's no floor, foxes cannot step off, but walls should go off and proceed to fall.
        else if (!this.gameObject.tag.Equals("Player"))
        {
            //This tag is set by a fxn that checks for the next floor to see what it lands on
            if (potentialFloorTag == "Player")
            {
                warnController.Warn(crushFoxWarnMessage);
                return false;
            }
            else
                return true;
        }
        return false;
    }

    //NOTE: Look into method overriding for giving FoxCharacter and PTBO their own versions instead of boolean argument
    //Checks to see if there's a wall in the given position, different logic for players and walls
    protected virtual bool NoWallIsPresent(Vector3 nextTilePosition)
    {
        Collider[] wallHitColliders = Physics.OverlapSphere(nextTilePosition, .1f);

        if (wallHitColliders.Length > 0) //there's something here, could be hut or wall etc.
        {
            if (wallHitColliders[0].CompareTag("ScriptTrigger"))//ignore colliders of this type
                return true;
            else
                return false;
        }
        else //no Colliders in target position
        {
            return true;
        }
    }

    //Checks to see if there's floor under the given tile
    protected bool FloorIsPresent(Vector3 nextTilePosition, out string potentialFloorTag)
    {
        Collider[] floorHitCollider = Physics.OverlapSphere(nextTilePosition + Vector3.down, .1f);

        if (floorHitCollider.Length > 0) //if this array isn't empty, there is a box collider below target position
        {
            //GameObject reference is used to check if floor is falling or stationary
            GameObject potentialFloor = floorHitCollider[0].gameObject;

            //Tag is used for special exceptions for the DestroyBounds collider and Foxes
            potentialFloorTag = potentialFloor.tag;

            //Do not count the collider that handles destroying falling blocks as a floor
            if (potentialFloorTag == "DestroyBounds" || potentialFloorTag == "ScriptTrigger")
                return false;

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
                else
                {
                    return true;
                }
            }
            else //floor isn't pushable and therefore can't be falling so this is also good
            {
                return true;
            }
        }
        //No immediate floor, but we must check multiple depths to avoid crushing foxies
        else
        {
            return crushedObjectCheck(nextTilePosition + Vector3.down, out potentialFloorTag);
        }
    }

    //This function checks the fallzone for maxDepth blocks and returns the tag of the first floor.
    // Basically, if a block is pushed off a cliff, what will it land on?
    // Always returns "false" referring to a floor at the original height. The out string is what
    //  actually sets things in motion to block the move
    private bool crushedObjectCheck(Vector3 startPosition, out string potentialFloorTag)
    {
        Vector3 curPosition = startPosition;
        Collider[] floorHitCollider;
        GameObject curFloor;
        for (int i = 0; i < maxDepth; i++)
        {
            floorHitCollider = Physics.OverlapSphere(curPosition, .1f);
            if (floorHitCollider.Length > 0) //found an object in this position
            {
                curFloor = floorHitCollider[0].gameObject;
                potentialFloorTag = curFloor.tag;
                return false;
            }
            curPosition = curPosition + Vector3.down;
        }
        potentialFloorTag = "NoFloor";
        return false;
    }


    //This just moves the object to the position and rotation given as args. Used by undoManager
    // Implemented differently in TBPO and FoxCharacter
    public abstract void UndoMyTurn(Vector3 oldPosition, Quaternion oldRotation);
   
    /// Simple function that checks if there's floor beneath the TBC and makes it fall if not.
    private void Fall()
    {
        if (!FloorIsPresent(this.transform.position, out string uneededHere) && !isMoving)
        {
            targetMoveToPosition = this.transform.position + Vector3.down;
        }
    }

    #region Helpers
    public bool GetIsMoving()
    {
        return isMoving;
    }

    public void SetTargetMoveToPosition(Vector3 newTargetMoveToPosition)
    {
        this.targetMoveToPosition = newTargetMoveToPosition;
    }

    public CharacterType GetCharacterType()
    {
        return characterType;
    }

    protected virtual void setThisMove(MoveOptions option)
    {
        //This is blank for walls, overriden in the FoxCharacter script 
    }
    #endregion

    public enum CharacterType
    {
        Player,
        NPC,
        Wall
    }

    protected enum MoveOptions 
    { 
        Walk, 
        Push, 
        Left, 
        Right, 
        None 
    }
}
