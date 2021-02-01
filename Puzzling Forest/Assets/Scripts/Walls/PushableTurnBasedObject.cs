using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTurnBasedObject : TurnBasedCharacter
{
    [SerializeField]
    private bool stackPushingIsEnabled = true;

    public override void SpecialAction()
    {
        throw new System.NotImplementedException();
    }

    public bool IsStackPushingEnabled()
    {
        return stackPushingIsEnabled;
    }

    //Given a direction (ideally, 1 unit on the x or z axis), this object is pushed forward in that direction at the given speed
    public bool PushForwardInDirectionOnGridTile(Vector3 direction, float movementSpeed)
    {
        Vector3 targetPosition = this.transform.position + direction;

        
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
