using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private GameObject[] PlayerGroup;
    private TurnBasedCharacter[] PlayerScripts;

    static private int numPlayers;

    private int curTurnIndex = 0;

    [SerializeField]
    private Text moveCountUIText;
    [SerializeField]
    public int turnCycleCount = 0;
    public int totalMoveCount = 0;
    [HideInInspector]
    public bool isLevelComplete;

    private void Start()
    {
        SetUpPlayerGroup();

        GiveTurn(curTurnIndex);
    }

    //Sets up the PlayerGroup by finding all game objects with the "Player" tag in the scene.
    private void SetUpPlayerGroup()
    {
        PlayerGroup = GameObject.FindGameObjectsWithTag("Player");
        numPlayers = PlayerGroup.Length;
        PlayerScripts = new TurnBasedCharacter[numPlayers];

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerScripts[i] = PlayerGroup[i].GetComponent<TurnBasedCharacter>();
        }
    }

    //Increments the current turn index whilst keeping an eye on going out of bounds
    private void incrementIDX()
    {
        curTurnIndex++;
        if (curTurnIndex > numPlayers - 1)
            curTurnIndex = 0;
    }

    //Enables the turn of the player from the PlayerGroup[idx]
    private void GiveTurn(int idx)
    {
        TurnBasedCharacter curScript = PlayerScripts[idx];

        if (curScript.CheckIfTakingTurns())
        {
            curScript.SetTurnActive(true);
        }
    }

    //Disables the turn of the player from the PlayerGroup[idx]
    private void TakeTurn(int idx)
    {
        TurnBasedCharacter curScript = PlayerScripts[idx];

        if (curScript.CheckTurn())
        {
            curScript.SetTurnActive(false);
        }
    }

    //Increments curIDX, disables the current fox's turn, enables the next valid fox's turn
    public void EndTurn()
    {
        TakeTurn(curTurnIndex);

        int loopIDX = 0;
        incrementIDX();
        while (!PlayerScripts[curTurnIndex].CheckIfTakingTurns())
        {
            incrementIDX();
            loopIDX++;
            if (loopIDX >= numPlayers)
            {
                Debug.Log("No players found taking turns.");
                return;
            }
        }

        GiveTurn(curTurnIndex);
    }

    //Lil' helper getter setter stuff
    public int GetNumPlayers()
    {
        return numPlayers;
    }

    public TurnBasedCharacter GetPlayerScript(int idx)
    {
        return PlayerScripts[idx];
    }

    //Updates the UI text "total Moves"
    public void UpdateMoveCount()
    {
        moveCountUIText.text = totalMoveCount.ToString();
    }
}
