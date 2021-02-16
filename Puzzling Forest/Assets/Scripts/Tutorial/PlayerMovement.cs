using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Block-commented the entire class as it isn't currently used but references legacy code
public class PlayerMovement : MonoBehaviour
{

    //public TurnManager turnManager;
    //public TurnManager.TurnInstance turn;
    //public bool isTurn = false;
    ////public KeyCode moveKey;
    //public int maxMovementRange = 5;
    //private int currentMovementRemaining;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    //Find the turn manager in game; use it to
    //    turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

    //    foreach(TurnManager.TurnInstance currentTurn in turnManager.playersGroup)
    //    {
    //        if(currentTurn.playerGameObject.name == gameObject.name)
    //        {
    //            turn = currentTurn;
    //        }
    //    }
    //    ResetMovement();

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //isTurn = turn.isTurn;

    //    //if (isTurn)
    //    //{

    //    //        if (Input.GetKeyDown(KeyCode.UpArrow))
    //    //        {
    //    //            transform.position += Vector3.forward;
    //    //            //currentMovementRemaining--;
    //    //            currentMovementRemaining--;
    //    //        }
    //    //        if (Input.GetKeyDown(KeyCode.DownArrow))
    //    //        {
    //    //            transform.position += Vector3.back;
    //    //            //currentMovementRemaining--;
    //    //            currentMovementRemaining--;

    //    //        }
    //    //        if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    //        {
    //    //            transform.position += Vector3.left;
    //    //            //currentMovementRemaining--;
    //    //            currentMovementRemaining--;

    //    //        }
    //    //        if (Input.GetKeyDown(KeyCode.RightArrow))
    //    //        {
    //    //            transform.position += Vector3.right;
    //    //            //currentMovementRemaining--;
    //    //            currentMovementRemaining--;

    //    //        }




    //    //    isTurn = false;
    //    //    turn.isTurn = isTurn;
    //    //    turn.wasTurnPrev = true;

    //    //}
    //    UpdateTurnForPlayer();

    //}

    //private void UpdateTurnForPlayer()
    //{
    //    isTurn = turn.isTurn;

    //    if (isTurn)
    //    {
    //        //if it is turn, AND there is current movement remaining
    //            //Listen for input, move accordingly

    //        //else, 
    //        if(currentMovementRemaining > 0)
    //        {
    //            if (Input.GetKeyDown(KeyCode.UpArrow))
    //            {
    //                transform.position += Vector3.forward;
    //                currentMovementRemaining--;
                    
    //            }
    //        }
    //        else
    //        {
    //            isTurn = false;
    //            turn.isTurn = isTurn;
    //            turn.wasTurnPrev = true;
    //        }
    //    }
    //}

    //public void ResetMovement()
    //{

    //    currentMovementRemaining = maxMovementRange;
    //}
}
