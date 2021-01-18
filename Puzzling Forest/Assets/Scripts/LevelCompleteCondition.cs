using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteCondition : MonoBehaviour
{
    public List<TurnBasedCharacter> registeredPlayerList;
    private int levelCompletePlayerCount = 0;
    private TurnManager turnManager = null;
    [SerializeField]
    private String nextLevelName;
    
    [SerializeField]
    private GameObject levelCompletePanel;
    [SerializeField]
    private Text totalMoveCount;
    [SerializeField]
    private Text totalTime;

    private Timer tm;

    private void Start()
    {
        GetPlayersFromTurnManager();
        tm = GameObject.Find("GameManager").GetComponent<Timer>();
        levelCompletePanel.SetActive(false);
        tm.isLevelComplete = false;
        turnManager.isLevelComplete = false;
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
                VictoryData();
                levelCompletePanel.SetActive(true);
                
            }

#if UNITY_EDITOR
                SceneManager.LoadScene(nextLevelName);
#else
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
#endif

            
        }
    }

    private void VictoryData()
    {
        tm.isLevelComplete = true;
        turnManager.isLevelComplete = true;
        totalMoveCount.text = turnManager.totalMoveCount.ToString();
        totalTime.text = tm.totaltime.ToString();
    }
}
