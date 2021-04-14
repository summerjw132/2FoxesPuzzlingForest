using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyLevel")]
public class MyLevel : ScriptableObject
{
    [Tooltip("Name of the level")]
    public string LevelName;

    [Tooltip("Lowest move count the player has beaten the level with")]
    public string BestMoveCount;

    [Tooltip("Lowest undo count the player has beaten the level with")]
    public string UndoMoveCount;

    [Tooltip("Boolean to check if the level has been completed")]
    public bool isLevelComplete;

    [Tooltip("Determines if the level is unlocked")]
    public bool isUnlocked;
}
