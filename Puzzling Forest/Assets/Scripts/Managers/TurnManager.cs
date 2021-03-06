﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
//For TEST LOGS
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles input and controls characters' turns
/// </summary>
public class TurnManager : MonoBehaviour
{
    //Turn/Which Fox Can Move Stuff
    static private int numPlayers;
    private GameObject[] PlayerGroup;
    private FoxCharacter[] PlayerScripts;
    private FairyController[] FairyScripts;
    private FoxCharacter curPlayer;
    private int curTurnIndex = 0;

    //Scoring stuff
    private LevelSelectManager levelManager;
    [SerializeField] private Text moveCountUIText = null;
    [SerializeField] private Text undoCountUIText = null;
    [HideInInspector] public bool isLevelComplete;
    public int undoCount = 0;
    public int totalMoveCount = 0;
    //For TEST LOGS
    private Timer timer;

    //Pause Menu Stuff
    private PauseMenuManager pauseManager;

    //Undo Stuff
    private UndoManager undoManager;

    //Control Stuff
    private bool isAnimating = false;
    private bool keyJustPressed = true;
    private bool pauseLock = false;
    private bool cameraLock = false;
    private const float keyDelayDuration = 0.2f;
    private WaitForSeconds keyDelay = new WaitForSeconds(keyDelayDuration);
    private Coroutine keyDelayer = null;

    //For continuous walking
    private bool walkQueued = false;
    private Coroutine walkQueuer = null;

    //For passing on Summer's last message
    FairyController.SpeechController.KeepTalkingInfo contInfo;

    private void Awake()
    {
        pauseManager = GameObject.Find("UI Canvas").GetComponent<PauseMenuManager>();
        undoManager = GameObject.Find("GameManager").GetComponent<UndoManager>();

        SetUpPlayerGroup();
    }

    private void Start()
    {
        Time.maximumDeltaTime = keyDelayDuration;
        for (int i = 0; i < numPlayers; i++)
        {
            FairyScripts[i].gameObject.SetActive(false);
        }

        GiveTurn();
        keyJustPressed = false;

        //For displaying level name... kind of tricky since its index is unrelated
        try
        {
            levelManager = GameObject.Find("LevelSelectManager").GetComponent<LevelSelectManager>();
            transform.Find("UI Canvas/Panel_Info/LevelIndicator").GetComponent<Text>().text = "Level " + (levelManager.GetLevelIndex(SceneManager.GetActiveScene().name)+1);
        }
        catch
        {
            //Do nothing
        }

        //For TEST LOGS
        timer = this.gameObject.GetComponent<Timer>();
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
                if (curPlayer && !curPlayer.GetIsMoving() && curPlayer.CheckIfTakingTurns() && !isAnimating && !cameraLock && !keyJustPressed)
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PressKey();
                        SwapFoxes(); 
                    }
                    else if (Input.GetKeyDown(KeyCode.U))
                    {
                        PressKey();
                        undoManager.UndoTurn();
                    }
                    else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    {
                        PressKey();
                        WASDPressed(KeyCode.W);
                    }
                    else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        PressKey();
                        WASDPressed(KeyCode.A);
                    }
                    else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    {
                        PressKey();
                        WASDPressed(KeyCode.D);
                    }
                    else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    {
                        PressKey();
                        WASDPressed(KeyCode.S);
                    }
            }
            UpdateMoveCount();
        }
    }

    /// <summary>
    /// Allowing the player to hold buttons was too fast and led to the animations restarting too soon.
    ///  This Coroutine just handles that to only re-read a held key every 0.25 seconds. (way shorter than any animation)
    /// </summary>
    private void WASDPressed(KeyCode code)
    {
        
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

            case KeyCode.S:
                curPlayer.Turn("around");
                break;

            default:
                break;
        }
    }

    public void StartWalkingQueuer()
    {
        walkQueued = false;
        if (walkQueuer != null)
        {
            Debug.Log("tried to start walkQueuer while one existed!");
            StopCoroutine(walkQueuer);
            walkQueuer = StartCoroutine(WalkQueuer());
        }
        else
        {
            walkQueuer = StartCoroutine(WalkQueuer());
        }
    }
    public void StopWalkingQueuer()
    {
        if (walkQueuer == null)
        {
            return;
        }

        StopCoroutine(walkQueuer);
        walkQueuer = null;

        if (walkQueued)
        {
            curPlayer.TryKeepWalking();
        }
        else
        {
            curPlayer.StopWalking();
        }
    }
    private IEnumerator WalkQueuer()
    {
        while (true)
        {
            if (Input.anyKey)
            {
                if (!pauseLock)
                {
                    if (curPlayer && !cameraLock && !keyJustPressed)
                    {
                        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                        {
                            PressKey();
                            walkQueued = true;
                            StopWalkingQueuer();
                        }
                    }
                }
            }
            yield return null;
        }
    }

    public void PressKey()
    {
        if (keyDelayer != null)
        {
            StopCoroutine(keyDelayer);
            keyJustPressed = true;
            keyDelayer = StartCoroutine(KeyPressDelay());
        }
        else
        {
            keyJustPressed = true;
            keyDelayer = StartCoroutine(KeyPressDelay());
        }
    }

    private IEnumerator KeyPressDelay()
    {
        yield return keyDelay;
        keyJustPressed = false;
    }

    //Sets up the PlayerGroup by finding all game objects with the "Player" tag in the scene.
    private void SetUpPlayerGroup()
    {
        PlayerGroup = GameObject.FindGameObjectsWithTag("Player");
        numPlayers = PlayerGroup.Length;
        PlayerScripts = new FoxCharacter[numPlayers];
        FairyScripts = new FairyController[numPlayers];

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerScripts[i] = PlayerGroup[i].GetComponent<FoxCharacter>();
            FairyScripts[i] = PlayerGroup[i].transform.Find("turnIndicator").GetComponent<FairyController>();
            //FairyScripts[i].gameObject.SetActive(false);
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
        contInfo = FairyScripts[curTurnIndex].GetSpeechInfo();
        FairyScripts[curTurnIndex].ShutUp();
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

    //This is called by the SwapFox animation when the ball goes off the screen for the first one.
    // It then handles activating the turn and playing the "catch" animation for the second one.
    public void SwappedFoxes()
    {
        if (curTurnIndex > -1)
        {
            curPlayer = PlayerScripts[curTurnIndex];
            curPlayer.ToggleIndicator(true);
            curPlayer.CatchTheBall();
            GiveTurn();

            FairyScripts[curTurnIndex].KeepSaying(contInfo);
        }
    }

    //These two functions are only used in tutorial levels to arrest control long enough for exposition.
    public void StealControl()
    {
        curPlayer.SetFairyActive(false);
        curPlayer.SetTurnActive(false);
        curPlayer = null;
    }

    public void ResumeControl()
    {
        curPlayer = PlayerScripts[curTurnIndex];
        curPlayer.SetFairyActive(true);
        curPlayer.SetTurnActive(true);
        curPlayer.CatchTheBall();
    }


    //The following is stuff for the Summer/Fairy dialogue system. Each fox has its own Summer since they weren't
    // originally intended to be personified. As such, the turn manager is needed in order to control WHICH Summer
    // is being told to do what at a given time and for ensuring all of the Summers are on the same page.

    public float Say(string msg)
    {
        if (curPlayer)
        {
            return FairyScripts[curTurnIndex].Say(msg);
        }
        else
            return -1f;
    }

    public void ShutUp()
    {
        if (curPlayer)
        {
            FairyScripts[curTurnIndex].ShutUp();
        }
        return;
    }

    //These are the functions that an individual Summer will call when she wants it to apply to all
    // all Summers or just the Summers that are relevant. TurnManager (this) script then handels
    // the logic for which Summers are actually relevant.
    public void ResetFairySpeechProgress()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            FairyScripts[i].resetMyProgress();
        }
    }

    public void IncrementFairySpeechProgress()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            FairyScripts[i].incrementMyProgress();
        }
    }

    public void StopTalking()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            FairyScripts[i].stopTalking();
        }
    }

    public void ClearDialogue()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            FairyScripts[i].clearDialogue();
        }
    }

    //Lil' helper getter setter stuff
    public int GetNumPlayers()
    {
        return numPlayers;
    }

    public bool GetKeyJustPressed()
    {
        return keyJustPressed;
    }

    public FoxCharacter GetPlayerScript(int idx)
    {
        return PlayerScripts[idx];
    }

    public GameObject[] GetPlayers()
    {
        return PlayerGroup;
    }

    public GameObject GetCurrentFairy()
    {
        return curPlayer.transform.Find("turnIndicator").gameObject;
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
        Debug.Log("begin anim at: " + Time.time);
        isAnimating = true;
    }
    public void completeAnimation()
    {
        Debug.Log("complete anim at: " + Time.time);
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

    public float GetKeyDelayDuration()
    {
        return keyDelayDuration;
    }

    //For TEST LOGS
    public void LogUserTest()
    {
        int randID;
        if (PlayerPrefs.HasKey("randomID"))
            randID = PlayerPrefs.GetInt("randomID");
        else
        {
            PlayerPrefs.SetInt("randomID", Random.Range(10000, 99999));
            randID = PlayerPrefs.GetInt("randomID");
        }
        string fileName = "/PuzzlingForestLog_" + randID + ".csv" ;

        System.DateTime localDateTime = System.DateTime.Now;
        string msg;
        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            msg = "DATE,NAME,COMPLETE/RESET,TIME,MOVES,UNDOS,MODE";
            File.AppendAllText(Application.persistentDataPath + fileName, msg);

            //System Info
            string sysInfo;
            sysInfo = ",,,deviceModel,graphicsID,operatingSystem";
            sysInfo += "\n,,,,,,,,";
            sysInfo += "," + SystemInfo.deviceModel.Replace(",", ".");
            sysInfo += "," + SystemInfo.graphicsDeviceName.Replace(",", ".");
            sysInfo += "," + SystemInfo.operatingSystem.Replace(",", ".");
            File.AppendAllText(Application.persistentDataPath + fileName, sysInfo);
        }

        msg = "\n" + localDateTime.ToString();
        msg += "," + SceneManager.GetActiveScene().name;
        msg += "," + "reset";
        msg += "," + timer.GetTime();
        msg += "," + totalMoveCount;
        msg += "," + undoCount;
        msg += "," + PlayerPrefs.GetString("Speed");


        File.AppendAllText(Application.persistentDataPath + fileName, msg);
    }

    //For TEST LOGS
    // Note: this is hooked up to the level complete condition, the one above is hooked up to the rest button,
    // and this one is now also hooked up to the exit button in the pause menu
    public void LogUserTest(bool complete)
    {
        int randID;
        if (PlayerPrefs.HasKey("randomID"))
            randID = PlayerPrefs.GetInt("randomID");
        else
        {
            PlayerPrefs.SetInt("randomID", Random.Range(10000, 99999));
            randID = PlayerPrefs.GetInt("randomID");
        }
        string fileName = "/PuzzlingForestLog_" + randID + ".csv";

        string completionStatus;
        if (!complete)
            completionStatus = "exited";
        else
            completionStatus = "completed";

        System.DateTime localDateTime = System.DateTime.Now;
        string msg;
        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            msg = "DATE,NAME,COMPLETE/RESET,TIME,MOVES,UNDOS,MODE";
            File.AppendAllText(Application.persistentDataPath + fileName, msg);

            //System Info
            string sysInfo;
            sysInfo = ",,,deviceModel,graphicsID,operatingSystem";
            sysInfo += "\n,,,,,,,,";
            sysInfo += "," + SystemInfo.deviceModel.Replace(",", ".");
            sysInfo += "," + SystemInfo.graphicsDeviceName.Replace(",",".");
            sysInfo += "," + SystemInfo.operatingSystem.Replace(",", ".");
            File.AppendAllText(Application.persistentDataPath + fileName, sysInfo);
        }

        msg = "\n" + localDateTime.ToString();
        msg += "," + SceneManager.GetActiveScene().name;
        msg += "," + completionStatus;
        msg += "," + timer.GetTime();
        msg += "," + totalMoveCount;
        msg += "," + undoCount;
        msg += "," + PlayerPrefs.GetString("Speed");

        File.AppendAllText(Application.persistentDataPath + fileName, msg);
    }
}
