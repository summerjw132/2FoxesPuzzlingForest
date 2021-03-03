using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class exists because, for some reason, buttons are unable to find a script that is attached to an object that
///  doesn't get destroyed between scenes. Specifically, the second+ time a scene is loaded, if the script is in DoNotDestroy area,
///  buttons will fail to find their functions.
///  
/// So, instead, the buttons are linked to a function in this class. This class simply finds the script each time and then calls
///  the corresponding functions.
/// </summary>
public class ButtonClickHandler : MonoBehaviour
{
    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    public void UnlockAllLevels()
    {
        levelManager.unlockAllLevels();
    }

    public void ResetLevelProgress()
    {
        levelManager.resetLevelProgress();
    }
}
