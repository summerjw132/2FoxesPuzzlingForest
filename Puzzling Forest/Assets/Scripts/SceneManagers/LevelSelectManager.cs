using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    private static LevelSelectManager instance;

    [SerializeField] private GameObject branchPagePrefab = null;
    private GameObject[] branchPages;
    private GameObject[] leaves;

    private int curBranch = 0;
    private int numBranches;
    private const int leavesPerBranch = 15;

    private static List<MyLevel> allLevels = new List<MyLevel>();

    private void Awake()
    {
        //So that the instance of this class persists between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += InitMenu;
        }
        else
            Destroy(this.gameObject);
    }

    //DoNotDestroy() does not call Awake() so we have to make sure this happens
    // every time manually.
    void InitMenu(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LevelSelectMenu")
        {
            curBranch = 0;
            LoadFromFile();
            SetUpMenu();
            SetLevelPermission();
        }
    }

    /// <summary>
    /// This function creates as many levelselect pages as needed based on the leaves per branch
    ///  and the total number of levels. It then goes through each page and sets up the individual
    ///  leaves to have the correct text and the correct level linked to the OnClick. Sets unused leaves
    ///  to blank text and inactive.
    /// </summary>
    public void SetUpMenu()
    {
        GameObject curBranch;
        GameObject curButton;
        bool isValidButton = true;

        int totalCount;

        numBranches = Mathf.CeilToInt((float)allLevels.Count / (float)leavesPerBranch);
        branchPages = new GameObject[numBranches];
        leaves = new GameObject[numBranches * leavesPerBranch];

        for (int i = 0; i < numBranches; i++)
        {
            branchPages[i] = Instantiate(branchPagePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            branchPages[i].name = branchPages[i].name + "_" + i;
        }
        
        for (int i = 0; i < numBranches; i++)
        {
            curBranch = branchPages[i];
            for (int j = 0; j < leavesPerBranch; j++)
            {
                totalCount = (i * leavesPerBranch + j);
                if (totalCount >= allLevels.Count)
                    isValidButton = false;

                curButton = GameObject.Find(curBranch.name + "/Canvas/Branch/LeafButton (" + (j + 1) + ")");
                leaves[totalCount] = curButton;
                if (isValidButton)
                {
                    curButton.GetComponentInChildren<Text>().text = "" + (totalCount + 1);
                    curButton.GetComponent<NextLevel>().nextLevel = allLevels[totalCount].LevelName;
                }
                else
                {
                    curButton.GetComponentInChildren<Text>().text = "";
                    curButton.GetComponent<Button>().interactable = false;
                }
            }

            if (i == 0)
                GameObject.Find(branchPages[i].name + "/Canvas/PageSelect/Backward").GetComponent<Button>().interactable = false;
            if (i+1 == numBranches)
                GameObject.Find(branchPages[i].name + "/Canvas/PageSelect/Forward").GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < numBranches; i++)
        {
            GameObject.Find(branchPages[i].name + "/Canvas/PageSelect/Forward").GetComponent<Button>().onClick.AddListener(IncrementBranchPage);
            GameObject.Find(branchPages[i].name + "/Canvas/PageSelect/Backward").GetComponent<Button>().onClick.AddListener(DecrementBranchPage);

            if (i > 0)
                branchPages[i].SetActive(false);
        }

        Invoke("SetLevelPermission", 0.01f);
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

    public int GetLevelIndex(string levelName)
    {
        for (int i = 0; i < allLevels.Count; i++)
        {
            if (allLevels[i].LevelName == levelName)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// This method sets the buttons to active or inactive depending on whether or not the corresponding Scriptable
    /// Object in the allLevels list indicates the level isUnlocked or not. Also tells the leaf managers
    ///  to shade the leaves accordingly.
    /// </summary>
    private void SetLevelPermission()
    {
        GameObject curLeaf;
        for (int i = 0; i < leaves.Length; i++)
        {
            curLeaf = leaves[i];
            if (i < allLevels.Count && allLevels[i].isUnlocked)
            {
                curLeaf.GetComponent<Button>().interactable = true;
            }
            else
            {
                curLeaf.GetComponent<Button>().interactable = false;
            }

            curLeaf.GetComponent<LeafManager>().ShadeLeafActive();
        }

        UpdateText();
    }

    public void IncrementBranchPage()
    {
        if (curBranch < numBranches-1)
        {
            branchPages[curBranch].SetActive(false);
            curBranch++;
            branchPages[curBranch].SetActive(true);
        }

        UpdateText();
    }

    public void DecrementBranchPage()
    {
        if (curBranch > 0)
        {
            branchPages[curBranch].SetActive(false);
            curBranch--;
            branchPages[curBranch].SetActive(true);
        }

        UpdateText();
    }

    private void UpdateText()
    {
        GameObject.Find(branchPages[curBranch].name + "/Canvas/PageSelect/Text").GetComponent<Text>().text = (curBranch + 1) + " / " + numBranches;
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
}
