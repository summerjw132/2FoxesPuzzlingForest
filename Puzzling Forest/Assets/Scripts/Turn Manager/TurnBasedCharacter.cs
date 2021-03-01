﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class TurnBasedCharacter : MonoBehaviour
{
    //turn-system stuff
    protected TurnManager turnManager;
    private bool isMyTurn = false;
    private bool isTakingTurns = true;

    //undo-system stuff
    protected UndoManager undoManager;
    private bool needToWrite = false;

    [SerializeField]
    private CharacterType characterType = CharacterType.Player;
    [SerializeField]
    private GameObject turnIndicator = null;

    private bool isMoving = false;
    protected Vector3 targetMoveToPosition;
    private Transform foxTransform = null;
 
    private float moveSpeed = 2.5f;
    private int maxDepth = 15; //This is how deep to check for a fox before pushing a block that could crush it

    //animation stuff
    // animation controller script
    protected foxAnimationStateController animController;
    // used as flags for knowing which type of animation to call each move
    private enum MoveOptions { Walk, Push, Left, Right, None };
    private MoveOptions thisMove = MoveOptions.None;
    private bool isAnimating = false;

    //UI Warn Message Stuff
    private WarningMessagesController warnController = null;
    //In the string below, the warning message is cut off immediately after "Twelve" That's your max length
    //                                            "One Two Three Four Five Six Seven Eight Nine Ten Eleven Twelve Thirteen Fourteen";
    private static string vertStackWarnMessage  = "One of the blocks you tried to move is weighed down!";
    private static string crushFoxWarnMessage   = "Be careful! This move would crush a fox!";

    //For finding and bringing up the pause menu!
    private GameObject UI_Canvas;
    private PauseMenuManager pauseManager;

    private bool isPauseMenuOpen = false;
    private bool isCameraModeOpen = false;

    //GUI stuff for foxholes
    // script so that we can call initiate warp on the foxhole we're standing on
    private FoxHole curFoxholeScript = null;
    // for toggling when to display the button
    private bool displayButton;
    // for styling/sizing the button. Look in OnGUI() for how these are set
    private float butX;
    private float butY;
    private float butWidth;
    private float butHeight;
    private float butOffsetX;
    private float butOffsetY;
    private GUIStyle guiStyle;
    // reference to the camera so that we can display the button in screen coords
    Camera cam = null;
    // a flag so that the style stuff isn't set every frame
    private bool setFontSize = true;

    void Start()
    {
        targetMoveToPosition = this.transform.position;

        //Fox/PlayerCharacter specific stuff
        if (characterType == CharacterType.Player)
        {
            //animation controller
            animController = GetComponent<foxAnimationStateController>();
            foxTransform = this.gameObject.transform.Find("Fox");

            UI_Canvas = GameObject.Find("UI Canvas");
            pauseManager = UI_Canvas.GetComponent<PauseMenuManager>();

            cam = GameObject.Find("GameManager").GetComponentInChildren<Camera>();
        }

        //a reference to the WarningController script that handles UI warning messages
        warnController = GameObject.Find("UI Canvas").GetComponent<WarningMessagesController>();

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

        undoManager = GameObject.Find("GameManager").GetComponent<UndoManager>();
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
            //No NPC implemented yet
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
                switch (thisMove)
                {
                    case MoveOptions.Walk:
                        animController.startWalking();
                        thisMove = MoveOptions.None;
                        break;

                    case MoveOptions.Push:
                        animController.startPushing();
                        thisMove = MoveOptions.None;
                        break;

                    default:
                        break;
                }
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, targetMoveToPosition, moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            if (needToWrite)
            {
                needToWrite = false;
                undoManager.WriteTurnState();
            }
            isMoving = false;
        }
    }

    void OnGUI()
    {
        //does this stuff once, sets the font size and button size relative to screen size
        if (setFontSize && this.characterType == CharacterType.Player)
        {
            guiStyle = GUI.skin.button;
            guiStyle.fontSize = (int)(cam.pixelHeight / 35f);
            butWidth = cam.pixelWidth / 20f;
            butHeight = cam.pixelHeight / 20f;
            butOffsetX = butWidth / 2f;
            butOffsetY = cam.pixelHeight / 10f;

            setFontSize = false;
        }
        //displays the button and calls the warp if it's clicked
        if (displayButton && !isMoving)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(this.transform.position);
            butX = screenPos.x - butOffsetX;
            butY = (cam.pixelHeight - screenPos.y) - butOffsetY;

            if (GUI.Button(new Rect(butX, butY, butWidth, butHeight), "Warp", guiStyle))
            {
                curFoxholeScript.InitiateWarp();
            }
        }
    }

    private void Fall()
    {
        if (!FloorIsPresent(this.transform.position, out string uneededHere) && !isMoving)
        {
            targetMoveToPosition = this.transform.position + Vector3.down;
        }
    }

    public abstract void SpecialAction();

    private void UpdateTurnForPlayer()
    {
        //Deactivates controls if it's the other players turn.
        if (isMyTurn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseManager.togglePauseMenu();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                isCameraModeOpen = !isCameraModeOpen;
            }
            //Deactivate controls if character isMoving from point to point or if an animation is going
            if (!isMoving && !isAnimating && !isPauseMenuOpen && !isCameraModeOpen)
            {
                if (Input.GetKeyDown(KeyCode.E)) //end the turn if 'E' is pressed
                {
                    StartCoroutine("EndMyTurn");
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    undoManager.UndoTurn();
                }

                //The foxes current facing direction used for any input
                Vector3 curFacing = foxTransform.forward.normalized;
                Quaternion curRotation = foxTransform.rotation;
                //Movement input/controls happens here!
                // UP/W moves fox *forwards* which is dependent on the orientation of the fox
                // LEFT/A and RIGHT/D rotates the fox left and right in-place, respectively
                // DOWN/S makes the fox do a 180 in-place
                // So to *move* left, the user should press 'A' to turn, then 'W' to move
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    Vector3 currentPosition = transform.position;
                    if (OkayToMoveToNextTile(currentPosition + curFacing))
                    {
                        //undo stuff
                        undoManager.LogState(this.gameObject);
                        needToWrite = true;

                        //move count stuff
                        turnManager.totalMoveCount++;
                        turnManager.UpdateMoveCount();

                        //moving stuff
                        targetMoveToPosition = currentPosition + curFacing;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    Turn("back", curRotation);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    Turn("left", curRotation);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    Turn("right", curRotation);
                }
            }
        }
    }

    private void Turn(string dir, Quaternion curRotation)
    {
        switch (dir)
        {
            case "back":
                //no back/180 turn currently implemented
                break;

            case "left":
                animController.startTurningLeft(curRotation);
                break;

            case "right":
                animController.startTurningRight(curRotation);
                break;

            default:
                Debug.Log("Turn method was given a direction it doesn't recognize");
                break;
        }
    }

    //Check if it is OK to move to the next
    protected bool OkayToMoveToNextTile(Vector3 nextTilePosition)
    {
        //Check Walls
        //Collider[] wallHitColliders = Physics.OverlapSphere(nextTilePosition, .1f);
        //Collider[] floorHitCollider = Physics.OverlapSphere(nextTilePosition + Vector3.down, .1f);
        string potentialFloorTag;

        if (FloorIsPresent(nextTilePosition, out potentialFloorTag)) //There is a floor
        {
            //Check to make sure we're not walking onto a Fox! That's bad for their backs!
            if (potentialFloorTag == "Player")
            {
                warnController.Warn(crushFoxWarnMessage);
                return false;
            }
            //Second parameter is whether or not it's the fox trying to move
            else if (NoWallIsPresent(nextTilePosition, this.gameObject.tag.Equals("Player"))) //There is no blocking wall 
            {
                //This move is into open space therefore it is a walk, not a push
                thisMove = MoveOptions.Walk;
                return true;
            }
            else
            {
                GameObject wall = Physics.OverlapSphere(nextTilePosition, .1f)[0].gameObject;
                PushableTurnBasedObject pushableWall = wall.GetComponent<PushableTurnBasedObject>();

                //this stuff is used to make sure the wall doesn't have another block weighting it on top
                Collider[] checkForWeightZone = Physics.OverlapSphere(nextTilePosition + Vector3.up, .1f);
                GameObject stackedWall = null;
                if (checkForWeightZone.Length > 0)
                {
                    stackedWall = checkForWeightZone[0].gameObject;
                }

                if (pushableWall != null)
                {
                    //This checks to see if the wall is vertically stacked and prevents the move if so!
                    if (stackedWall != null) //there is some gameobject on top of the pushableWall!
                    {
                        warnController.Warn(vertStackWarnMessage);
                        return false;
                    }

                    if (this.gameObject.tag.Equals("Player") || pushableWall.IsStackPushingEnabled()) // If this object is a player OR (if not a player, and) the pushable object has stack pushing
                    {
                        if (pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f, this.gameObject))
                        {
                            //This move is into a wall therefore it is a push, not a walk
                            thisMove = MoveOptions.Push;
                            return true;
                        }
                        else
                            return false;
                        //return pushableWall.PushForwardInDirectionOnGridTile(nextTilePosition - this.targetMoveToPosition, .2f, this.gameObject);
                    }

                }
            }
        } else if (!this.gameObject.tag.Equals("Player"))//This ensures that we can push off an edge (no floor)
        {
            if (potentialFloorTag == "Player")
            {
                warnController.Warn(crushFoxWarnMessage);
                return false;
            }
            else
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
                if (wallHitColliders.Length > 1) //can only happen if a fox and a house are overlapped
                    return true;
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
    protected bool FloorIsPresent(Vector3 nextTilePosition, out string potentialFloorTag)
    {
        //is there a box collider in the tile below the given tile?
        Collider[] floorHitCollider = Physics.OverlapSphere(nextTilePosition + Vector3.down, .1f);

        if (floorHitCollider.Length > 0) //if this array isn't empty, there is a box collider
        {
            //Now we need to make sure the potential floor (owner of box collider) isn't falling before it counts as floor
            GameObject potentialFloor = floorHitCollider[0].gameObject;

            //Make sure it's not a Fox, that shouldn't count as a floor for crushing concerns!
            potentialFloorTag = potentialFloor.tag;

            //Do not count the collider that handles destroying falling blocks as a floor
            if (potentialFloor.CompareTag("DestroyBounds"))
            {
                return false;
            }

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
        //there was no box collider in the first place; no floor, but we must check multiple depths to avoid crushing foxies
        return crushedObjectCheck(nextTilePosition + Vector3.down, out potentialFloorTag);
    }

    //This is a lil fxn to check the fallzone for maxDepth blocks and returns the first floor found
    // returns false no matter what bc "floorIsPresent" is most helpful referring to only depth_1 stuff
    // the string returns the tag which is helpful for blocking moves that crush Foxes
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

    //This just moves the fox to the position and rotation given as args
    public void UndoMyTurn(Vector3 oldPosition, Quaternion oldRotation)
    {
        //for blocks that were "destroyed" from falling
        if (!this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(true);

        if (characterType == CharacterType.Player)
            this.gameObject.transform.Find("Fox").rotation = oldRotation;
        else
            this.gameObject.transform.rotation = oldRotation;
        
        this.gameObject.transform.position = oldPosition;
        targetMoveToPosition = oldPosition;
    }

    //Now that warping is an interactive button, we need to write this action to the
    // undo stack. This is simply called every time a foxhole initiates a warp
    public void WriteFoxholeToUndoStack()
    {
        undoManager.LogState(this.gameObject);
        undoManager.WriteTurnState();
    }

    //This is just a public method for incrementing the movement counter. Foxholes
    // will call this when they initiate the warp
    public void IncrementMoveCounter()
    {
        turnManager.totalMoveCount++;
        turnManager.UpdateMoveCount();
    }

    //This is used by foxholes to tell the foxes when they should and shouldn't display the
    // "Warp" button
    public void ShowFoxholeButton(bool enabled, FoxHole curScript)
    {
        displayButton = enabled;
        curFoxholeScript = curScript;
    }
   
    //Lil' getters and setters
    public bool GetIsMoving()
    {
        return isMoving;
    }

    public void SetTargetMoveToPosition(Vector3 newTargetMoveToPosition)
    {
        this.targetMoveToPosition = newTargetMoveToPosition;
    }

    public bool CheckTurn()
    {
        return isMyTurn;
    }

    public void SetTurnActive(bool value)
    {
        isMyTurn = value;
    }

    public bool CheckIfTakingTurns()
    {
        return isTakingTurns;
    }

    public void StopTakingTurns()
    {
        isTakingTurns = false;
        turnManager.EndTurn();
    }

    public void StartTakingTurns()
    {
        isTakingTurns = true;
    }

    public void togglePauseMenuBlock()
    {
        isPauseMenuOpen = !isPauseMenuOpen;
    }

    //if you don't have this delay, it ends the turn so quickly that the other fox gets the same input so it swaps back
    private IEnumerator EndMyTurn()
    {
        yield return new WaitForSeconds(0.05f);
        turnManager.EndTurn();
    }

    //This updates the turn indicator to active if it is this character's turn
    private void UpdateTurnIndicator()
    {
        if(turnIndicator != null)
        {
            turnIndicator.SetActive(isMyTurn);
        }
    }

    //These are just wrapper functions to set/reset an "isAnimating" flag.
    // The animations themselves call these functions using animation events.
    // While the flag is set, user input is not accepted.
    public void beginAnimation()
    {
        //Debug.Log("begin anim");
        isAnimating = true;
    }
    public void completeAnimation()
    {
        //Debug.Log("complete anim");
        isAnimating = false;
    }

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
}
