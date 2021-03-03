using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Block-commented the entire class as it isn't currently used but references legacy code
public class EnemyMovement : MonoBehaviour
{
    //public TurnManager turnManager;
    //public TurnManager.TurnInstance turn;
    //public bool isTurn = false;
    //public KeyCode moveKey;

    //// Start is called before the first frame update
    //void Start()
    //{

    //    turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

    //    foreach (TurnManager.TurnInstance currentTurn in turnManager.playersGroup)
    //    {
    //        if (currentTurn.playerGameObject.name == gameObject.name)
    //        {
    //            turn = currentTurn;
    //        }
    //    }
    //}

    //private void Update()
    //{
    //    isTurn = turn.isTurn;

    //    if (isTurn)
    //    {
    //        StartCoroutine("WaitAndMove");
    //    }
    //}

    //IEnumerator WaitAndMove()
    //{
    //    yield return new WaitForSeconds(1f);
    //    transform.position += Vector3.forward;
    //    isTurn = false;
    //    turn.isTurn = isTurn;
    //    turn.wasTurnPrev = true;

    //    StopCoroutine("WaitAndMove");

    //}
}
