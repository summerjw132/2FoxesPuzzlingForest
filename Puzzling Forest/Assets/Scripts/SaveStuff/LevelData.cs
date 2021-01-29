using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is very similar to the MyLevel scriptable object and should be updated if more is added there.
/// Each LevelData instance is the same as one MyLevel object and is used for convenience with the
/// SaveSystem.
/// 
/// A new instance can be constructed by simply passing it a MyLevel object.
/// </summary>
[System.Serializable]
public class LevelData
{
    public string LevelName;
    public string BestMoveCount;
    public bool isLevelComplete;
    public bool isUnlocked;

    public LevelData (MyLevel level)
    {
        LevelName = level.LevelName;
        BestMoveCount = level.BestMoveCount;
        isLevelComplete = level.isLevelComplete;
        isUnlocked = level.isUnlocked;
    }
}
