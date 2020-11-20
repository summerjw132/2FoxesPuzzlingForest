using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteCondition : MonoBehaviour
{
    public List<TurnBasedCharacter> registeredPlayerList;
    private int levelCompletePlayerCount = 0;
    private TurnManager turnManager = null;
    
    [SerializeField]
    private GameObject levelCompletePanel;

    private void Start()
    {
        GetPlayersFromTurnManager();
    }

    private void GetPlayersFromTurnManager()
    {

        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

        for(int i = 0; i< turnManager.playersGroup.Count; i++)
        {
            TurnBasedCharacter characterInstance = turnManager.playersGroup[i].playerGameObject.GetComponent<TurnBasedCharacter>();

            if(characterInstance.GetCharacterType() == TurnBasedCharacter.CharacterType.Player)
            {
                registeredPlayerList.Add(characterInstance);
            }
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(registeredPlayerList.Contains(other.gameObject.GetComponent<TurnBasedCharacter>()) && other.gameObject.GetComponent<TurnBasedCharacter>().turn.isEnabled)
        {
            Debug.Log(other.gameObject.name + "Has reached the level finish!");
            levelCompletePlayerCount++;
            other.gameObject.GetComponent<TurnBasedCharacter>().turn.isEnabled = false;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        if(levelCompletePlayerCount.Equals(registeredPlayerList.Count))
        {
            Debug.Log("You Win!");
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }
        }
    }
}
