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
    
    [SerializeField] private GameObject levelCompletePanel = null;
    [SerializeField] private Text totalMoveCount = null;
    [SerializeField] private Text totalTime = null;

    private Timer tm;

    //Save/Load, permanent progress stuff
    private Scene curScene;
    private string curLevelName;
    private LevelManager levelManager;

    private void Awake()
    {
        levelCompletePanel.SetActive(false);
        tm = GameObject.Find("Turn-Based System").GetComponent<Timer>();

        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
    }

    private void Start()
    {
        tm.isLevelComplete = false;

        turnManager.isLevelComplete = false;
        GetPlayersFromTurnManager();

        //Save/Load stuff
        curScene = SceneManager.GetActiveScene();
        curLevelName = curScene.name;
        try
        {
            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        }
        catch
        {
            Debug.Log("levelManager not found. Expected if you loaded a scene/level directly");
        }
    }

    private void GetPlayersFromTurnManager()
    {
        for (int i = 0; i < turnManager.GetNumPlayers(); i++)
        {
            TurnBasedCharacter characterInstance = turnManager.GetPlayerScript(i);

            if (characterInstance.GetCharacterType() == TurnBasedCharacter.CharacterType.Player)
            {
                registeredPlayerList.Add(characterInstance);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(registeredPlayerList.Contains(other.gameObject.GetComponent<FoxCharacter>()))
        {
            Debug.Log(other.gameObject.name + "Has reached the level finish!");
            levelCompletePlayerCount++;
            other.gameObject.GetComponent<FoxCharacter>().StopTakingTurns();
        }

        if(levelCompletePlayerCount.Equals(registeredPlayerList.Count))
        {
            Debug.Log("You Win!");
            
            if (levelCompletePanel != null)
            {
                VictoryData();
                levelCompletePanel.SetActive(true);
                
            }

            //Save/Load stuff
            //This completeLevel fxn sets the current level to complete, stores the score if its best
            try
            {
                levelManager.completeLevel(curLevelName, turnManager.totalMoveCount, turnManager.undoCount);
                Debug.LogFormat("Called completeLevel");
            }
            catch (NullReferenceException nre)
            {
                Debug.LogFormat("Level Manager not set, expected iff you loaded a level directly\nerror: {0}", nre);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Elvis has left the building by undo");
        levelCompletePlayerCount--;
        other.gameObject.GetComponent<FoxCharacter>().StartTakingTurns();
    }

    private void VictoryData()
    {
        tm.isLevelComplete = true;
        turnManager.isLevelComplete = true;
        totalMoveCount.text = turnManager.totalMoveCount.ToString();
        totalTime.text = tm.totaltime.ToString();
    }
}
