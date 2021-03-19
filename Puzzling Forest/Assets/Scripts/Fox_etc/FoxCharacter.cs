using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxCharacter : TurnBasedCharacter
{
    private Transform foxTransform = null;
    private GameObject turnIndicator = null;

    //turn-system stuff
    protected TurnManager turnManager;
    public bool isMyTurn { get; private set; } = false;
    private bool isTakingTurns = true;

    //animation stuff
    protected foxAnimationStateController animController;
    private Animation indicatorAnim;
    //used as flags for knowing which type of animation to call each move
    private MoveOptions thisMove = MoveOptions.None;
    public bool isAnimating { get; private set; }

    //GUI stuff for foxholes
    private FoxHole curFoxholeScript = null;
    private bool displayButton;
     //for styling/sizing the button. Look in OnGUI() for how these are set
    private float butX;
    private float butY;
    private float butWidth;
    private float butHeight;
    private float butOffsetX;
    private float butOffsetY;
    private GUIStyle guiStyle;
    private Vector3 curScreenPos;
    //reference to the camera so that we can display the button in screen coords
    static Camera cam = null;
    //a flag so that the style stuff isn't set every frame
    private bool setFontSize = true;

    // Awake is called before Start
    protected override void Awake()
    {
        base.Awake();

        //Find the turn manager in game; use it to
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

        //Get the camera
        cam = GameObject.Find("GameManager").GetComponentInChildren<Camera>();

        //animation controller
        animController = GetComponent<foxAnimationStateController>();
        foxTransform = this.transform.Find("Fox");

        turnIndicator = this.transform.Find("turnIndicator").gameObject;
        indicatorAnim = turnIndicator.GetComponent<Animation>();
    }

    protected override void Start()
    {
        base.Start();

        UpdateSpeed();
    }

    //This is called whenever a GUI event happens, it's used here to do the warp button above
    // the foxes' heads
    void OnGUI()
    {
        //does this stuff once, sets the font size and button size relative to screen size
        if (setFontSize)
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
            curScreenPos = cam.WorldToScreenPoint(this.transform.position);
            butX = curScreenPos.x - butOffsetX;
            butY = (cam.pixelHeight - curScreenPos.y) - butOffsetY;

            if (GUI.Button(new Rect(butX, butY, butWidth, butHeight), "Enter", guiStyle))
            {
                curFoxholeScript.InitiateWarpCoroutine();
            }
        }
    }

    protected override void UpdateSpeed()
    {
        moveSpeed = 1f / SecondsToMove;

        animController.UpdateDuration(SecondsToMove);
    }

    //TurnManager calls this with a W or UP input, tries to move the fox forwards
    public void MoveForward()
    {
        //The foxes current facing direction used for any input
        Vector3 curFacing = foxTransform.forward.normalized;     
        Vector3 currentPosition = transform.position;

        if (OkayToMoveToNextTile(currentPosition + curFacing))
        {
            //undo stuff
            undoManager.LogState(this.gameObject);
            needToWrite = true;

            //moving stuff
            targetMoveToPosition = currentPosition + curFacing;
            IncrementMoveCounter();

            //animation for this move
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
    }

    //TurnManager calls this with A or D input, tries to rotate fox left or right respectively
    public void Turn(string dir)
    {
        Quaternion curRotation = foxTransform.rotation;
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

    public float Dive()
    {
        return animController.diveIntoFoxhole();
    }

    public override void UndoMyTurn(Vector3 oldPosition, Quaternion oldRotation)
    {
        this.gameObject.transform.Find("Fox").rotation = oldRotation;

        this.gameObject.transform.position = oldPosition;
        targetMoveToPosition = oldPosition;
    }

    //overrided method from TBC script because foxes want to ignore houses in this check
    protected override bool NoWallIsPresent(Vector3 nextTilePosition)
    {
        Collider[] wallHitColliders = Physics.OverlapSphere(nextTilePosition, .1f);

        if (wallHitColliders.Length > 0) //there's something here, could be hut or wall etc.
        {
            for (int i = 0; i < wallHitColliders.Length; i++)
            {
                if (wallHitColliders[i].CompareTag("House"))
                {
                    return true;
                }
            }
            return false;
        }
        else //no Colliders in target position
        {
            return true;
        }
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

    //Now that warping is an interactive button, we need to write this action to the
    // undo stack. This is simply called every time a foxhole initiates a warp
    public void WriteFoxholeToUndoStack()
    {
        undoManager.LogState(this.gameObject);
        undoManager.WriteTurnState();
    }

    public bool CheckTurn()
    {
        return isMyTurn;
    }

    public void SetTurnActive(bool value)
    {
        isMyTurn = value;
        //turnIndicator.SetActive(value);
    }

    public void SetFairyActive(bool value)
    {
        turnIndicator.SetActive(value);
    }

    public bool CheckIfTakingTurns()
    {
        return isTakingTurns;
    }

    public void StopTakingTurns()
    {
        isTakingTurns = false;
        turnManager.SwapFoxes();
    }

    public void StartTakingTurns()
    {
        isTakingTurns = true;
    }

    protected override void setThisMove(MoveOptions option)
    {
        thisMove = option;
    }

    /// <summary>
    /// This function is part of what will be called when the player swaps foxes.
    ///  I'm envisioning this as just handling the animation stuff, I will have a second function
    ///  in the TurnMaanager script for actually changing whose turn it is.
    /// </summary>
    public void PassTheBall()
    {
        beginAnimation();
        animController.startPassTheBall();
    }
    
    public void TouchTheBall()
    {
        indicatorAnim.Play("RaiseBall");
    }
    public void CatchTheBall()
    {
        indicatorAnim.Play("DropBall");
    }
    //Fairies use this to determine, based on fox position, whether to put text box on left or right
    public string LeftOrRight()
    {
        if (cam.WorldToScreenPoint(this.transform.position).x < Screen.width / 2f)
            return "right";
        else
            return "left";
    }
    //These are just wrapper functions to set/reset an "isAnimating" flag.
    // The animations themselves call these functions using animation events.
    // While the flag is set, user input is not accepted.
    public void beginAnimation()
    {
        //Debug.LogFormat("Begin at {0}", Time.time);
        isAnimating = true;
        turnManager.beginAnimation();
    }
    public void completeAnimation()
    {
        //Debug.LogFormat("End at {0}", Time.time);
        isAnimating = false;
        turnManager.completeAnimation();
    }
    public void cAnimation()
    {
        StartCoroutine(delay(0.5f));
    }

    IEnumerator delay(float time)
    {
        yield return new WaitForSeconds(time);
        isAnimating = false;
        turnManager.completeAnimation();
    }

    public void ToggleIndicator(bool b)
    {
        turnIndicator.SetActive(b);
    }
}
