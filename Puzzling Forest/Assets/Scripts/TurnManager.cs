using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //List of players under turn system
    public List<TurnInstance> playersGroup;

    private int currentPlayerMaxMovementRange = 0;
    private int currentPlayerMovementRangeRemaining = 0;

    private void Start()
    {
        ResetTurns();

    }
    private void Update()
    {
        UpdateTurns();
    }

    //Resets the turn system to start with the first player 
    void ResetTurns()
    {
        for(int i = -0; i < playersGroup.Count; i++)
        {
            if(i == 0)
            {
                playersGroup[i].isTurn = true;
                playersGroup[i].wasTurnPrev = false;
                playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>().ResetMovement();
            }
            else
            {

                playersGroup[i].isTurn = false;
                playersGroup[i].wasTurnPrev = false;
                playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>().ResetMovement();
            }
        }
    }

    //  
    void UpdateTurns()
    {
        for(int i = 0; i < playersGroup.Count; i++)
        {
            if (playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>())
            {
                if (!playersGroup[i].wasTurnPrev)
                {
                    playersGroup[i].isTurn = true;
                    break;
                }
                else if(i == playersGroup.Count - 1 &&
                        playersGroup[i].wasTurnPrev)
                {
                    ResetTurns();
                }
            }
            else
            {
                Debug.LogWarning("The player object named '" + playersGroup[i].playerGameObject.name + "' does not have 'Turn Based Character' component.  Skipping...");
            }

        }

    }

    //The instance of each turn for each player
    [System.Serializable]
    public class TurnInstance
    {
        public GameObject playerGameObject;
        public bool isTurn = false; //True when a player's turn is active, false before and after the player's turn
        public bool wasTurnPrev = false; //Once a player's turn has ended, this will be set to true (unless Reset turn is called
    }
}
