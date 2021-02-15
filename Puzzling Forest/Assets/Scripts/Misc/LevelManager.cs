using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Button buttonLevel;

    [SerializeField]
    private GameObject scrollViewContainer;

    private List<MyLevel> allLevels = new List<MyLevel>();

    private static GameObject instance;

    //These booleans, editable from editor, can be used to control level progress for demos etc.
    [SerializeField]
    private bool ShouldIUnlockAllLevels;
    [SerializeField]
    private bool ShouldIResetLevelProgress;

    private void Awake()
    {
        //So that the instance of this class persists between scenes
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this.gameObject;
        else
            Destroy(this.gameObject);

        PopulateLevels();

        Load();

        SetLevelPermission();

        if (ShouldIUnlockAllLevels)
            unlockAllLevels();

        if (ShouldIResetLevelProgress)
            resetLevelProgress();
    }

    /// <summary>
    /// This method will populate levels based on the number of SO levels and then instantiate buttons per SO and write all information on the
    /// buttons to load the proper level.
    /// </summary>
    void PopulateLevels()
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
                    curButton.GetComponentInChildren<Text>().text = allLevels[i].name + "\nScore: " + allLevels[i].BestMoveCount;
            }
            else
            {
                curButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// This method takes a LevelProgress and updates the SOs found in allLevels to reflect the
    ///  data in the parameter. So, after loading the save file as "progress," this method updates
    ///  allLevels to use that information
    /// </summary>
    private void updateScriptableObjects(LevelProgress progress)
    {
        //these better be the same (# of levels)
        if (progress.listOfLevelData.Length == allLevels.Count)
        {
            for (int i = 0; i < allLevels.Count; i++)
            {
                allLevels[i].LevelName = progress.listOfLevelData[i].LevelName;
                allLevels[i].BestMoveCount = progress.listOfLevelData[i].BestMoveCount;
                allLevels[i].isLevelComplete = progress.listOfLevelData[i].isLevelComplete;
                allLevels[i].isUnlocked = progress.listOfLevelData[i].isUnlocked;
            }
        }
        else
        {
            Debug.LogError("Tried to update SOs using the save file but they were of uneven length...");
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

    //This method is another wrapper. It does 2 things in order:
    // 1) Loads the save file as "progress"
    // 2) Uses "progress" to update the "allLevels" list in this class
    public void Load()
    {
        LevelProgress progress = SaveSystem.LoadLevelProgress();
        if (progress == null)
        {
            Debug.Log("No Save File");
            return;
        }
        //Debug.Log("Loaded successfully");
        //progress.Print();
        updateScriptableObjects(progress);
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
    public void completeLevel(string name, string moveCount)
    {
        int curScore = 0;
        int newScore = -1;
        int curLevelIndex = -1;
        int nextLevelIndex;
        for (int i = 0; i < allLevels.Count; i++)
        {
            if (allLevels[i].LevelName == name)
            {
                allLevels[i].isLevelComplete = true;
                int.TryParse(allLevels[i].BestMoveCount, out curScore);
                int.TryParse(moveCount, out newScore);
                if (newScore < curScore || curScore == 0)
                {
                    allLevels[i].BestMoveCount = moveCount;
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
        Load();
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
        Load();
        SetLevelPermission();
    }
}
