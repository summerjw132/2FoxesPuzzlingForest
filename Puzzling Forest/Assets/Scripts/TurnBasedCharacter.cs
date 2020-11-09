using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int currentMovementRemaining;

    private enum CharacterType
    {
        Player,
        NPC
    }


    void Start()
    {
        //Find the turn manager in game; use it to
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

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
        if(characterType == CharacterType.Player)
        {
            UpdateTurnForPlayer();

        }
        if (characterType == CharacterType.NPC)
        {
            UpdateTurnForNPC();

        }
    }


    public abstract void SpecialAction();

    private void UpdateTurnForPlayer()
    {
        isTurn = turn.isTurn;

        if (isTurn)
        {
            //if it is turn, AND there is current movement remaining
            //Listen for input, move accordingly

            //else, 
            if (currentMovementRemaining > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    transform.position += Vector3.forward;
                    currentMovementRemaining--;


                }else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    transform.position += Vector3.back;
                    currentMovementRemaining--;


                }else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    transform.position += Vector3.left;
                    currentMovementRemaining--;


                } else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    transform.position += Vector3.right;
                    currentMovementRemaining--;


                }
            }
            else
            {
                isTurn = false;
                turn.isTurn = isTurn;
                turn.wasTurnPrev = true;
            }
        }
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
}
