using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Button SetButtonLevel = null;
    private static Button buttonLevel = null;

    [SerializeField]
    private GameObject SetScrollViewContainer = null;
    private static GameObject scrollViewContainer = null;

    private static List<MyLevel> allLevels = new List<MyLevel>();

    private static GameObject instance;

    //TEMPORARY LOAD FROM WHERE STUFF
    GameObject LevelSelectMenu;
    GameObject LoadFromWhereMenu;
    Button LoadFileButton;
    Button LoadSOButton;

    private void Awake()
    {
        //So that the instance of this class persists between scenes
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this.gameObject;
        else
            Destroy(this.gameObject);

        if (buttonLevel == null)
            buttonLevel = SetButtonLevel;
        if (scrollViewContainer == null)
            scrollViewContainer = SetScrollViewContainer;

        LevelSelectMenu = GameObject.Find("LevelSelectMenu");
        LoadFromWhereMenu = GameObject.Find("LoadFromWhereMenu");

        //TEMPORARY LOAD FROM WHERE STUFF
        if (allLevels.Count == 0)
        {
            LoadFileButton = GameObject.Find("LoadFromFile").GetComponent<Button>();
            LoadSOButton = GameObject.Find("LoadFromSOs").GetComponent<Button>();

            LevelSelectMenu.SetActive(false);
            LoadFromWhereMenu.SetActive(true);

            LoadFileButton.onClick.AddListener(LoadFromFile);
            LoadFileButton.onClick.AddListener(GoToMenu);
            LoadSOButton.onClick.AddListener(LoadFromSciptableObjects);
            LoadSOButton.onClick.AddListener(GoToMenu);
        }
        else
            GoToMenu();
        
    }

    //TEMPORARY LOAD FROM WHERE STUFF
    public void GoToMenu()
    {
        LevelSelectMenu.SetActive(true);
        LoadFromWhereMenu.SetActive(false);

        PopulateButtons();
        SetLevelPermission();
    }

    /// <summary>
    /// This function populates the allLevels list using the SO's that are in the game.
    /// </summary>
    void PopulateAllLevels()
    {
        MyLevel[] levels = Resources.LoadAll<MyLevel>("Data/ScriptableObject");
        allLevels.Clear();

        //This for loop is iterating through all SO's.
        for (int i = 0; i < levels.Length; i++)
        {
            allLevels.Add(levels[i]);
        }
    }

    /// <summary>
    /// This function populates the buttons on the level-select menu using the allLevels List
    /// </summary>
    void PopulateButtons()
    {
        if (allLevels.Count > 0) //allLevels is initialized
        {
            Button newButton = buttonLevel;
            for (int i = 0; i < allLevels.Count; i++)
            {
                Instantiate(newButton, scrollViewContainer.transform);
                newButton.GetComponentInChildren<Text>().text = allLevels[i].name;
                newButton.name = allLevels[i].name;
                newButton.GetComponent<NextLevel>().nextLevel = allLevels[i].LevelName;
            }
            //This is to get the last level that keeps getting instatiated first removes it from the content viewer and then reparents it so that it will display as the last option available.
            GameObject lastButton = scrollViewContainer.transform.GetChild(0).gameObject;
            lastButton.transform.SetParent(null);
            lastButton.transform.SetParent(scrollViewContainer.transform);
        }
    }

    /// <summary>
    /// This method takes a LevelProgress and updates the SOs found in allLevels to reflect the
    ///  data in the parameter. So, after loading the save file as "progress," this method updates
    ///  allLevels to use that information
    /// </summary>
    private void UpdateScriptableObjects(LevelProgress progress)
    {
        //these better be the same (# of levels)
        if (progress.listOfLevelData.Length == allLevels.Count)
        {
            for (int i = 0; i < allLevels.Count; i++)
            {
                LevelData curLevel = progress.listOfLevelData[i];
                allLevels[i].LevelName = curLevel.LevelName;
                allLevels[i].BestMoveCount = curLevel.BestMoveCount;
                allLevels[i].UndoMoveCount = curLevel.BestUndoCount;
                allLevels[i].isLevelComplete = curLevel.isLevelComplete;
                allLevels[i].isUnlocked = curLevel.isUnlocked;
            }
        }
        else
        {
            Debug.LogError("Tried to update SOs using the save file but they were of uneven length...");
        }
    }

    /// <summary>
    /// This method sets the buttons to active or inactive depending on whether or not the corresponding Scriptable
    /// Object in the allLevels list indicates the level isUnlocked or not.
    /// </summary>
    private void SetLevelPermission()
    {
        Button curButton;
        for (int i = 0; i < scrollViewContainer.transform.childCount; i++)
        {
            curButton = scrollViewContainer.transform.GetChild(i).GetComponent<Button>();
            if (allLevels[i].isUnlocked)
            {
                curButton.interactable = true;
                if (allLevels[i].isLevelComplete)
                    curButton.GetComponentInChildren<Text>().text = allLevels[i].name + "\nMoves: " + allLevels[i].BestMoveCount + "\nUndos: " + allLevels[i].UndoMoveCount;
            }
            else
            {
                curButton.interactable = false;
            }
        }
    }

    //This is just a getter fxn for the private list allLevels
    public List<MyLevel> giveList()
    {
        return allLevels;
    }

    //This method is a wrapper. It just calls SaveLevelProgress from the SaveSystem class,
    // but is nice because it automatically passes it allLevels.
    public void Save()
    {
        SaveSystem.SaveLevelProgress(new LevelProgress(allLevels));
    }

    /// <summary>
    /// Both of these load functions populate AllLevels List with the appropriate data.
    ///  This one pulls it from a save file. The other uses the SO's that are set in game
    /// </summary>
    public void LoadFromFile()
    {
        PopulateAllLevels();

        LevelProgress progress = SaveSystem.LoadLevelProgress();
        if (progress == null)
        {
            Debug.Log("No Save File");
            return;
        }
        //Debug.Log("Loaded successfully");
        //progress.Print();
        UpdateScriptableObjects(progress);
    }

    /// <summary>
    /// This load function populates AllLevels using the SO's that are set in game editor.
    /// </summary>
    public void LoadFromSciptableObjects()
    {
        PopulateAllLevels();
    }

    //This method takes a string "name" (which is the name of the level being referred to
    // and sets that level to unlocked in allLevels. Does not update buttons
    public void unlockLevel(string name)
    {
        for (int i = 0; i < allLevels.Count; i++)
        {
            if (allLevels[i].LevelName == name)
            {
                allLevels[i].isUnlocked = true;
            }
        }
    }

    //Overloaded method from above. Uses an index instead of a name
    public void unlockLevel(int index)
    {
        if (index < allLevels.Count)
            allLevels[index].isUnlocked = true;
        else
            Debug.LogError("Tried to unlock a level with an index out of bounds for the list allLevels");
    }

    //Sets the level given by name to complete, and handle bestScore
    // Also unlocks the next level
    public void completeLevel(string name, int moveCount, int undoCount)
    {
        int curScore = 0;

        int curUndoScore = 0;

        int curLevelIndex = -1;
        int nextLevelIndex;
        for (int i = 0; i < allLevels.Count; i++)
        {
            if (allLevels[i].LevelName == name)
            {
                allLevels[i].isLevelComplete = true;
                int.TryParse(allLevels[i].BestMoveCount, out curScore);
                if (moveCount < curScore || curScore == 0)
                {
                    allLevels[i].BestMoveCount = moveCount.ToString();
                }

                int.TryParse(allLevels[i].UndoMoveCount, out curUndoScore);
                if (undoCount < curUndoScore || curUndoScore == 0)
                {
                    allLevels[i].UndoMoveCount = undoCount.ToString();
                }

                curLevelIndex = i;
            }
        }
        nextLevelIndex = curLevelIndex + 1;
        unlockLevel(nextLevelIndex);

        Save();
    }

    //This method resets the SOs in allLevels to their defaults...
    // ie only level one is unlocked and nothing is completed.
    public void resetLevelProgress()
    {
        allLevels[0].isLevelComplete = false;
        allLevels[0].isUnlocked = true;
        allLevels[0].BestMoveCount = "";

        for (int i = 1; i < allLevels.Count; i++)
        {
            allLevels[i].isLevelComplete = false;
            allLevels[i].isUnlocked = false;
            allLevels[i].BestMoveCount = "";
        }

        Save();
        LoadFromFile();
        SetLevelPermission();
    }

    //This unlocks all levels for demo/testing/whatever purposes
    public void unlockAllLevels()
    {
        for (int i = 0; i < allLevels.Count; i++)
        {
            allLevels[i].isLevelComplete = false;
            allLevels[i].isUnlocked = true;
            allLevels[i].BestMoveCount = "";
        }

        Save();
        LoadFromFile();
        SetLevelPermission();
    }

    /// <summary>
    /// This method will populate levels based on the number of SO levels and then instantiate buttons per SO and write all information on the
    /// buttons to load the proper level.
    /// 
    /// NOTE: This was used originally, but has been split into two functions (above) for greater control especially as we alter the level list
    /// </summary>
    void PopulateLevelsAndButtons()
    {
        //Retrieve the Directory the SO are stored.
        MyLevel[] levels = Resources.LoadAll<MyLevel>("Data/ScriptableObject");
        Button newButton = buttonLevel;
        //This for loop is iterating through all SO in the Directory above.
        for (int i = 0; i < levels.Length; i++)
        {
            Instantiate(newButton, scrollViewContainer.transform);
            newButton.GetComponentInChildren<Text>().text = levels[i].name;
            newButton.name = levels[i].name;
            newButton.GetComponent<NextLevel>().nextLevel = levels[i].LevelName;
            allLevels.Add(levels[i]);
        }
        //This is to get the last level that keeps getting instatiated first removes it from the content viewer and then reparents it so that it will display as the last option available.
        GameObject lastButton = scrollViewContainer.transform.GetChild(0).gameObject;
        lastButton.transform.SetParent(null);
        lastButton.transform.SetParent(scrollViewContainer.transform);
    }
}
