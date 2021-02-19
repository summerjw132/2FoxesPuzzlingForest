using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//UNUSED BUT I DON'T HAVE THE AUTHORITY TO DELETE. COMMENTING OUT INCASE YOU WANT IT LATER

public class LevelSelectManager : MonoBehaviour
{
    //Button[] LevelButtons;

    //[SerializeField]
    //private string loadingScene = "level";


    //private void Awake()
    //{
    //    //Use PlayerPrefs to permenantly store Level 1 as unlocked
    //    int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

    //    //If Level is set to 2 or more, set the Highest Reached Level to Level
    //    if(PlayerPrefs.GetInt("Level") >= 2)
    //    {
    //        reachedLevel = PlayerPrefs.GetInt("Level");
    //    }

    //    LevelButtons = new Button[transform.childCount];

    //    //Disables/enables buttons according to reached Level value
    //    //Labels all button by number
    //    for(int i=0; i<LevelButtons.Length; i++)
    //    {
    //        LevelButtons[i] = transform.GetChild(i).GetComponent<Button>();
    //        LevelButtons[i].GetComponentInChildren<Text>().text = (i + 1).ToString();

    //        if(i+1 > reachedLevel)
    //        {
    //            LevelButtons[i].interactable = false;
    //        }
    //    }

    //}

    //public void LoadScene(int Level)
    //{
    //    PlayerPrefs.SetInt("Level", Level);
    //    SceneManager.LoadScene("Loading");
    //}

    //public void nextLevel()
    //{

    //} 
}
