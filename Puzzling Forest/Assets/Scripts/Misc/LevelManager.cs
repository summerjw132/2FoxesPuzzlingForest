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


    private void Awake()
    {
        //So that the instance of this class persists between scenes
        DontDestroyOnLoad(this.gameObject);

        PopulateLevels();

        LevelProgress progress = SaveSystem.LoadLevelProgress();
        if (progress == null)
        {
            //new game
            SetLevelPermission();
        }
        else
        {
            //not a new game, has some saved file
            updateScriptableObjects(progress);
            SetLevelPermission();
        }
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
            }
            else
            {
                curButton.interactable = false;
            }
        }
    }

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

    void Test()
    {
        LevelProgress progress = new LevelProgress(allLevels);
        SaveSystem.SaveLevelProgress(progress);
        SaveSystem.LoadLevelProgress().Print();
    }

    public List<MyLevel> giveList()
    {
        return allLevels;
    }

    public void Save()
    {
        SaveSystem.SaveLevelProgress(new LevelProgress(allLevels));
    }
}
