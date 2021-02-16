using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private List<GameObject> PlayerGroup = new List<GameObject>();
    private TurnBasedCharacter[] PlayerScripts;

    [SerializeField]
    [Tooltip("Leave as 0 for automatic setup. Indicate number of players needing turns for manual setup.")]
    public int desiredNumPlayers = 0;
    [SerializeField]
    [Tooltip("Leave empty for automatic setup. For manual: Ensure this array.size == desiredNumPlayers. Include names of GameObjects at the 'Turn-Based Player' level.")]
    public string[] desiredPlayerNames;
    [Tooltip("Should I set up the PlayerGroup by default: [2-players, Turn-Based Player and Turn-Based Player (1)] ?")]
    public bool useDefaultPlayerGroup = true;

    static private int numPlayers;
    private string[] playerNames;
    private string[] playerName1 = new string[] { "Turn-Based Player", "Turn-Based Player #1" };
    private string[] playerName2 = new string[] { "Turn-Based Player (1)", "Turn-Based Player #2" };

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

    private void Update()
    {

    }

    private void SetUpPlayerGroup()
    {
        //Designer manually-set up the PlayerGroup
        if (!useDefaultPlayerGroup)
        {
            numPlayers = desiredNumPlayers;
            playerNames = desiredPlayerNames;
        }
        //Designer left blank for automatic/default set up of PlayerGroup
        else
        {
            numPlayers = 2;
            int idx = 0;
            if (GameObject.Find(playerName1[0]) != null)
                idx = 0;
            else if (GameObject.Find(playerName1[1]) != null)
                idx = 1;
            playerNames = new string[] { playerName1[idx], playerName2[idx] };
        }
        try
        {
            PlayerScripts = new TurnBasedCharacter[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                PlayerGroup.Add(GameObject.Find(playerNames[i]));
                PlayerScripts[i] = PlayerGroup[i].GetComponent<TurnBasedCharacter>();
            }
        }
        catch
        {
            Debug.Log("Failed to set up player group.");
        }
    }

    private void incrementIDX()
    {
        curTurnIndex++;
        if (curTurnIndex > numPlayers - 1)
            curTurnIndex = 0;
    }

    private void GiveTurn(int idx)
    {
        TurnBasedCharacter curScript = PlayerScripts[idx];

        if (curScript.CheckIfTakingTurns())
        {
            curScript.SetTurnActive(true);
        }
    }

    private void TakeTurn(int idx)
    {
        TurnBasedCharacter curScript = PlayerScripts[idx];

        if (curScript.CheckTurn())
        {
            curScript.SetTurnActive(false);
        }
    }

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

    public int GetNumPlayers()
    {
        return numPlayers;
    }

    public TurnBasedCharacter GetPlayerScript(int idx)
    {
        return PlayerScripts[idx];
    }

    public void UpdateMoveCount()
    {
        moveCountUIText.text = totalMoveCount.ToString();
    }
}
