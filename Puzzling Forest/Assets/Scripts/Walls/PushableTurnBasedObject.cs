using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTurnBasedObject : TurnBasedCharacter
{
    [SerializeField]
    private bool stackPushingIsEnabled = true;
    [SerializeField]
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

}
