using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    //List of players under turn system
    public List<TurnInstance> playersGroup;

    [SerializeField]
    private Text moveCountUIText;
    [SerializeField]
    private int turnCycleCount = 0;




    private void Start()
    {
        ResetTurns();

    }
    private void Update()
    {
        UpdateTurns();
    }

    public void SetMoveCountUIText(string text)
    {
        if(moveCountUIText != null)
        {
            moveCountUIText.text = text;
        }
    }
    //Resets the turn system to start with the first player 
    void ResetTurns()
    {
        bool firstEnabledPlayerWasFound = false; //The logic will continue to look for the first enabled player as long as this is false
        for(int i = -0; i < playersGroup.Count; i++)
        {
            if(!firstEnabledPlayerWasFound)
            {
                if (playersGroup[i].isEnabled) //If true, we have found the first enabled player in the turn list
                {
                    playersGroup[i].isTurn = true;
                    playersGroup[i].wasTurnPrev = false;
                    playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>().ResetMovement();
                    firstEnabledPlayerWasFound = true;
                }
                else //If not true, skip this player and go to the next
                {
                    Debug.Log("The player object named '" + playersGroup[i].playerGameObject.name + "' is not enabled.  Skipping...");
                }
            }
            else //For every other player in the list, set their turns to false
            {

                playersGroup[i].isTurn = false;
                playersGroup[i].wasTurnPrev = false;
                playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>().ResetMovement();
            }
        }
    }

    // Cycles to the next turn
    void UpdateTurns()
    {
        for(int i = 0; i < playersGroup.Count; i++)
        {

            //Only enabled if the current object is a TurnBasedCharacter AND if its turn is enabled 
            if (playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>() && playersGroup[i].isEnabled) 
            {
                if (!playersGroup[i].wasTurnPrev)
                {
                    playersGroup[i].isTurn = true;
                    break;
                }
                else if(i >= playersGroup.Count - 1 &&
                        playersGroup[i].wasTurnPrev)
                {
                    ResetTurns();
                    turnCycleCount++;
                }
            }
            else
            {
                Debug.LogWarning("The player object named '" + playersGroup[i].playerGameObject.name + "' is disabled or does not have 'Turn Based Character' component.  Skipping...");
                
                if (i >= playersGroup.Count - 1)//If this is the last turn-based character and it is not enabled, skip and reset the turn order
                {
                    ResetTurns();
                    turnCycleCount++;
                }
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
        public bool isEnabled = true; // Turn will only activate if it is enabled; otherwise, the turn will be skipped (i.e. if one player makes ti to the goal); default = true.
    }
}
