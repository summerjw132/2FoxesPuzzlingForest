using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTurnBasedObject : TurnBasedCharacter
{
    [SerializeField]
    private bool stackPushingIsEnabled = true;
    [SerializeField]
    private DirectionConstraint directionConstraint = DirectionConstraint.None;

    public enum DirectionConstraint
    {
        None,
        Constrained_to_X,
        Constrained_to_Z
    }

    private CharacterConstraint characterConstraint = CharacterConstraint.None;

    private List<string> Fox_1_names = new List<string> { "Turn-Based Player", "Turn-Based Player #1" };
    private List<string> Fox_2_names = new List<string> { "Turn-Based Player (1)", "Turn-Based Player #2" };

    public enum CharacterConstraint
    {
        None,
        Fox_1,
        Fox_2

    }

    public override void SpecialAction()
    {
        throw new System.NotImplementedException();
    }

    public bool IsStackPushingEnabled()
    {
        return stackPushingIsEnabled;
    }

    //Given a direction (ideally, 1 unit on the x or z axis), this object is pushed forward in that direction at the given speed
    public bool PushForwardInDirectionOnGridTile(Vector3 direction, float movementSpeed, GameObject pusher)
    {
        //only allow push if the block is not constrained against this direction
        switch (directionConstraint)
        {
            //can only move along the x axis
            case DirectionConstraint.Constrained_to_X:
                //checks if the attempted direction is along the x axis
                if (isAlongX(direction))
                    break;
                //tried to push block in a direction it is not allowed to move in
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

            //not constrained so go ahead
            default:
                break;
        }

        Vector3 targetPosition = this.transform.position + direction;

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
            default:
                break;
        }

        
        if (OkayToMoveToNextTile(targetPosition))
        {
            Debug.Log(this.gameObject.name + " is being pushed to " + targetPosition);
            targetMoveToPosition = targetPosition;
            return true;
        }
        else
        {
            Debug.Log(this.gameObject.name + " can't be pushed due to obstruction.");
            
            return false;
        }
        
    }

    //lil helper fxns
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

}
