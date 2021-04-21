using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTurnBasedObject : TurnBasedCharacter
{
    //Whether or not another block can push this block or if only the PC/Fox can. Recursive pushing.
    [SerializeField]
    private bool stackPushingIsEnabled = true;

    //Specifies any constraint in axes the block is allowed to move along
    [SerializeField]
    private DirectionConstraint directionConstraint = DirectionConstraint.None;

    //Specifies any constraint about who can push this block. No constraint or one of the foxes.
    [SerializeField]
    private CharacterConstraint characterConstraint = CharacterConstraint.None;

    //These are used to resolve character-constraint issues. Foxes are not always named the same thing...
    private List<string> Fox_1_names = new List<string> { "Turn-Based Player", "Turn-Based Player #1" };
    private List<string> Fox_2_names = new List<string> { "Turn-Based Player (1)", "Turn-Based Player #2" };

    //SFX Stuff
    private AudioSource blockMove;

    protected override void Awake()
    {
        base.Awake();

        blockMove = GameObject.Find("Audio Manager").transform.Find("BlockMove").GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        base.Start();

        if (PlayerPrefs.HasKey("Speed"))
        {
            if (PlayerPrefs.GetString("Speed") == "Normal")
                SecondsToMove = normalSpeedRock;
            else if (PlayerPrefs.GetString("Speed") == "Hyper")
                SecondsToMove = hyperSpeedRock;
            else
            {
                SecondsToMove = normalSpeedRock;
            }
        }
        else
        {
            PlayerPrefs.SetString("Speed", "Normal");
            PlayerPrefs.Save();
            SecondsToMove = normalSpeedRock;
        }

        UpdateSpeed();
    }

    protected override void UpdateSpeed()
    {
        moveSpeed = 1f / SecondsToMove;
    }

    /// <summary>
    /// This is really the only function that is special to the PTBO class that isn't in the TBC class already.
    ///  This is basically just a wrapper on TBC.OkayToMoveToNextTile().
    ///  
    /// First, wall-specific checks are done (the switch statements).
    /// Next, OkayToMove() is called on this wall. This can lead to a recursive chain of these methods
    ///  if another wall is encountered. If OkayToMove() returns true, targetPosition is simply updated.
    /// </summary>
    public bool PushForwardInDirectionOnGridTile(Vector3 direction, GameObject pusher)
    {
        //only allow push if the block is not constrained against this direction
        switch (directionConstraint)
        {
            //can only move along the x axis
            case DirectionConstraint.Constrained_to_X:
                if (isAlongX(direction))
                    break;
                else
                {
                    Debug.Log(this.gameObject.name + " can't be pushed because it is constrained to the X-axis");
                    return false;
                }

            case DirectionConstraint.Constrained_to_Z:
                if (isAlongZ(direction))
                    break;
                else
                {
                    Debug.Log(this.gameObject.name + " can't be pushed because it is constrained to the Z-axis");
                    return false;
                }

            //No directional constraint so go ahead.
            default:
                break;
        }

        //make sure the block is allowed to be pushed by whoever the pusher is
        switch (characterConstraint)
        {
            case CharacterConstraint.Fox_1:
                if (Fox_1_names.Contains(pusher.name))
                    break;
                else
                {
                    Debug.Log(this.gameObject.name + " can't be pushed because only Fox_1 is allowed to and I don't think Fox_1 tried to push me.");
                    return false;
                }

            case CharacterConstraint.Fox_2:
                if (Fox_2_names.Contains(pusher.name))
                    break;
                else
                {
                    Debug.Log(this.gameObject.name + " can't be pushed because only Fox_2 is allowed to and I don't think Fox_2 tried to push me.");
                    return false;
                }

            //No character constraint so go ahead.
            default:
                break;
        }

        Vector3 targetPosition = this.transform.position + direction;

        if (OkayToMoveToNextTile(targetPosition))
        {
            undoManager.LogState(this.gameObject);

            targetMoveToPosition = targetPosition;
            blockMove.Play();
            return true;
        }
        else
        {
            Debug.Log(this.gameObject.name + " can't be pushed here.");
            
            return false;
        }
    }

    public override void UndoMyTurn(Vector3 oldPosition, Quaternion oldRotation)
    {
        //for blocks that were "destroyed" from falling
        if (!this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(true);

        this.gameObject.transform.rotation = oldRotation;

        this.gameObject.transform.position = oldPosition;
        targetMoveToPosition = oldPosition;
    }

    #region Helpers
    public bool IsStackPushingEnabled()
    {
        return stackPushingIsEnabled;
    }

    public bool isAlongX(Vector3 direction)
    {
        direction = direction.normalized;
        if (direction == Vector3.left || direction == Vector3.right)
            return true;
        else
            return false;
    }

    public bool isAlongZ(Vector3 direction)
    {
        if (direction == Vector3.forward || direction == Vector3.back)
            return true;
        else
            return false;
    }
    #endregion

    #region Enums
    public enum DirectionConstraint
    {
        None,
        Constrained_to_X,
        Constrained_to_Z
    }

    public enum CharacterConstraint
    {
        None,
        Fox_1,
        Fox_2
    }
    #endregion
}
