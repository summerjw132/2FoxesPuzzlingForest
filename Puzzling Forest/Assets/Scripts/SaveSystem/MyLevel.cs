using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyLevel")]
public class MyLevel : ScriptableObject
{
    [Tooltip("Name of the Level you want to load")]
    public string LevelName;

    [Tooltip("Most optimal move count for completion")]
    public string BestMoveCount;

    [Tooltip("Most optimal undo count for completion")]
    public string UndoMoveCount;

    [Tooltip("Most optimal Time for completion")]
    public string BestTime;

    [Tooltip("Best move count for achieved")]
    public string OptimalMoveCount;

    [Tooltip("Best undo count for achieved")]
    public string OptimalUndoCount;

    [Tooltip("Best Time for achieved")]
    public string OptimalTime;

    [Tooltip("Boolean to check if the level has been completed")]
    public bool isLevelComplete;

    [Tooltip("Determines if the level is unlocked")]
    public bool isUnlocked;
}
