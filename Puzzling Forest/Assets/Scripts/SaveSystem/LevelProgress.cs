using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One LevelProgress instance contains a list of LevelDatas.
/// Essentially, this means it is just an ordered list of all the information found in each MyLevel.
/// It is this class that is Serialized/Deserialized as a Save File
/// </summary>
[System.Serializable]
public class LevelProgress
{
    public LevelData[] listOfLevelData;

    //Populates listOfLevelData using a List of MyLevels found in the LevelManager script.
    public LevelProgress(List<MyLevel> allLevels)
    {
        listOfLevelData = new LevelData[allLevels.Count];
        for (int i = 0; i < allLevels.Count; i++)
        {
            listOfLevelData[i] = new LevelData(allLevels[i]);
        }
    }

    /// <summary>
    /// Helper method to show the relevant information of the level Progress. Mostly a debugging help.
    /// </summary>
    public void Print()
    {
        LevelData curLevel;
        for (int i = 0; i < listOfLevelData.Length; i++)
        {
            curLevel = listOfLevelData[i];
            Debug.LogFormat("Name: {0} \n\tUnlocked: {1} \n\tComplete: {2} \n\tBestScore: {3}", curLevel.LevelName, curLevel.isUnlocked, curLevel.isLevelComplete, curLevel.BestMoveCount);
        }
    }
}
