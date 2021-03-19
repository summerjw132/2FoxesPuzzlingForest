using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles input and controls characters' turns
/// </summary>
public class TurnManager : MonoBehaviour
{
    //Turn/Which Fox Can Move Stuff
    static private int numPlayers;
    private GameObject[] PlayerGroup;
    private FoxCharacter[] PlayerScripts;
    private FoxCharacter curPlayer;
    private int curTurnIndex = 0;

    //Scoring stuff
    [SerializeField] private Text moveCountUIText = null;
    [SerializeField] private Text undoCountUIText = null;
    [HideInInspector] public bool isLevelComplete;
    public int undoCount = 0;
    public int totalMoveCount = 0;

    //Pause Menu Stuff
    private PauseMenuManager pauseManager;

    //Undo Stuff
    private UndoManager undoManager;

    //Control Stuff
    private bool isAnimating = false;
    private bool keyJustPressed = false;
    private bool pauseLock = false;
    private bool cameraLock = false;
    

    private void Awake()
    {
        pauseManager = GameObject.Find("UI Canvas").GetComponent<PauseMenuManager>();
        undoManager = GameObject.Find("GameManager").GetComponent<UndoManager>();

        SetUpPlayerGroup();
    }

    private void Start()
    {
        GiveTurn();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseManager.togglePauseMenu();
            }
            if (!pauseLock)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    //camera mode is toggled (this is done in a camera script)
                }
                if (curPlayer && !curPlayer.GetIsMoving() && !isAnimating && !cameraLock)
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SwapFoxes();
                    }
                    else if (Input.GetKeyDown(KeyCode.U))
                    {
                        undoManager.UndoTurn();
                    }
                    else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    {
                        StartCoroutine(WADPressed(KeyCode.W));
                    }
                    else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        StartCoroutine(WADPressed(KeyCode.A));
                    }
                    else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    {
                        StartCoroutine(WADPressed(KeyCode.D));
                    }
            }

            UpdateMoveCount();
        }
    }

    /// <summary>
    /// Allowing the player to hold buttons was too fast and led to the animations restarting too soon.
    ///  This Coroutine just handles that to only re-read a held key every 0.25 seconds. (way shorter than any animation)
    /// </summary>
    private IEnumerator WADPressed(KeyCode code)
    {
        if (keyJustPressed)
            yield break;
        else
        {
            keyJustPressed = true;
            switch (code)
            {
                case KeyCode.W:
                    curPlayer.MoveForward();
                    break;

                case KeyCode.A:
                    curPlayer.Turn("left");
                    break;

                case KeyCode.D:
                    curPlayer.Turn("right");
                    break;

                default:
                    break;
            }
            yield return new WaitForSeconds(0.25f);
            keyJustPressed = false;
        }
    }

    

    //Sets up the PlayerGroup by finding all game objects with the "Player" tag in the scene.
    private void SetUpPlayerGroup()
    {
        PlayerGroup = GameObject.FindGameObjectsWithTag("Player");
        numPlayers = PlayerGroup.Length;
        PlayerScripts = new FoxCharacter[numPlayers];

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerScripts[i] = PlayerGroup[i].GetComponent<FoxCharacter>();
        }

        curPlayer = PlayerScripts[curTurnIndex];
    }

    //Increments the current turn index whilst keeping an eye on going out of bounds
    private void incrementIDX()
    {
        curTurnIndex++;
        if (curTurnIndex > numPlayers - 1)
            curTurnIndex = 0;
    }

    //Enables the turn of the player from the PlayerGroup[idx]
    private void GiveTurn()
    {
        if (curPlayer && curPlayer.CheckIfTakingTurns())
        {
            curPlayer.SetTurnActive(true);
            curPlayer.ToggleIndicator(true);
        }
    }

    //Disables the turn of the player from the PlayerGroup[idx]
    private void TakeTurn()
    {
        if (curPlayer && curPlayer.CheckTurn())
        {
            curPlayer.SetTurnActive(false);
            curPlayer.PassTheBall();
            curPlayer = null;
        }
    }

    //Increments curIDX, disables the current fox's turn, enables the next valid fox's turn
    public void SwapFoxes()
    {
        TakeTurn();

        int loopIDX = 0;
        incrementIDX();
        while (!PlayerScripts[curTurnIndex].CheckIfTakingTurns())
        {
            incrementIDX();
            loopIDX++;
            if (loopIDX >= numPlayers)
            {
                Debug.Log("No players found taking turns.");
                curTurnIndex = -1;
                curPlayer = null;
                return;
            }
        }
    }

    public void SwappedFoxes()
    {
        if (curTurnIndex > -1)
        {
            curPlayer = PlayerScripts[curTurnIndex];
            curPlayer.ToggleIndicator(true);
            curPlayer.CatchTheBall();
            GiveTurn();
        }
    }

    //Lil' helper getter setter stuff
    public int GetNumPlayers()
    {
        return numPlayers;
    }

    public FoxCharacter GetPlayerScript(int idx)
    {
        return PlayerScripts[idx];
    }

    public GameObject[] GetPlayers()
    {
        return PlayerGroup;
    }

    //Updates the UI text "total Moves"
    public void UpdateMoveCount()
    {
        moveCountUIText.text = totalMoveCount.ToString();
        undoCountUIText.text = undoCount.ToString();
    }

    //These are just wrapper functions to set/reset an "isAnimating" flag.
    // The animations themselves call these functions using animation events.
    // While the flag is set, user input is not accepted.
    public void beginAnimation()
    {
        //Debug.Log("begin anim");
        isAnimating = true;
    }
    public void completeAnimation()
    {
        //Debug.Log("complete anim");
        isAnimating = false;
    }

    public void TogglePauseMenu()
    {
        pauseLock = !pauseLock;
    }

    public void ToggleCameraMode()
    {
        cameraLock = !cameraLock;
    }
}
