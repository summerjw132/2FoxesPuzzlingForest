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

    private GameObject GM;
    private Timer tm;

    //Save/Load, permanent progress stuff
    private Scene curScene;
    private string curLevelName;
    private LevelManager levelManager;

    private void Start()
    {
        GetPlayersFromTurnManager();
        GM = GameObject.Find("GameManager");
        if (GM == null)
            GM = GameObject.Find("Game Manager");
        tm = GM.GetComponent<Timer>();

        levelCompletePanel.SetActive(false);
        tm.isLevelComplete = false;
        turnManager.isLevelComplete = false;

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

        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

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
        if(registeredPlayerList.Contains(other.gameObject.GetComponent<TurnBasedCharacter>()) && other.gameObject.GetComponent<TurnBasedCharacter>().CheckTurn())
        {
            Debug.Log(other.gameObject.name + "Has reached the level finish!");
            levelCompletePlayerCount++;
            other.gameObject.GetComponent<TurnBasedCharacter>().StopTakingTurns();
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

            //Save/Load stuff
            //This completeLevel fxn sets the current level to complete, stores the score if its best
            levelManager.completeLevel(curLevelName, turnManager.totalMoveCount.ToString());
            Debug.LogFormat("Called completeLevel");
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
