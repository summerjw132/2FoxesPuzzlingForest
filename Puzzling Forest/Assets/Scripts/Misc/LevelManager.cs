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
        PopulateLevels();
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
}
