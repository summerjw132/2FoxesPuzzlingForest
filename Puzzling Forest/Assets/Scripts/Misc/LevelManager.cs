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

    void PopulateLevels()
    {
        MyLevel[] levels = Resources.LoadAll<MyLevel>("Data/ScriptableObject");
        Button newButton = buttonLevel;
        for (int i = 0; i < levels.Length; i++)
        {
            Instantiate(newButton, scrollViewContainer.transform);
            newButton.GetComponentInChildren<Text>().text = levels[i].name;
            newButton.name = levels[i].name;
            newButton.GetComponent<NextLevel>().nextLevel = levels[i].LevelName;
            allLevels.Add(levels[i]);
        }
        GameObject lastButton = scrollViewContainer.transform.GetChild(0).gameObject;
        lastButton.transform.SetParent(null);
        lastButton.transform.SetParent(scrollViewContainer.transform);
    }
}
