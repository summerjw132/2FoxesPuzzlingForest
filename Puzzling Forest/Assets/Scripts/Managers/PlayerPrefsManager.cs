using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NOTE: Unneeded after the actual settings menu is created and implemented!
using UnityEngine.UI;

public class PlayerPrefsManager: MonoBehaviour
{
    //NOTE: Please use the methods below these ones later on. I'm throwing this together for use on the button
    // on the main menu but the end goal is to have a separate settings page so the extra stuff in this version
    // of the method is undesired.
    void Start()
    {
        UpdateButtonText();
    }
    public void ToggleSpeedSettingONMENU()
    {
        if (PlayerPrefs.HasKey("Speed"))
        {
            if (PlayerPrefs.GetString("Speed") == "Normal")
                PlayerPrefs.SetString("Speed", "Hyper");
            else if (PlayerPrefs.GetString("Speed") == "Hyper")
                PlayerPrefs.SetString("Speed", "Normal");
            else
            {
                Debug.LogError("PlayerPrefs has key \"Speed\" but the value was unexpected.");
                PlayerPrefs.SetString("Speed", "Normal");
            }
        }
        else
        {
            PlayerPrefs.SetString("Speed", "Hyper");
        }

        PlayerPrefs.Save();
        UpdateButtonText();
    }
    private void UpdateButtonText()
    {
        if (PlayerPrefs.HasKey("Speed"))
            this.gameObject.GetComponentInChildren<Text>().text = "Current Speed:\n" + PlayerPrefs.GetString("Speed");
    }


    //KEEP THESE STARTING BELOW
    public void ToggleSpeedSetting()
    {
        if (PlayerPrefs.HasKey("Speed"))
        {
            if (PlayerPrefs.GetString("Speed") == "Normal")
                PlayerPrefs.SetString("Speed", "Hyper");
            else if (PlayerPrefs.GetString("Speed") == "Hyper")
                PlayerPrefs.SetString("Speed", "Normal");
            else
            {
                Debug.LogError("PlayerPrefs has key \"Speed\" but the value was unexpected.");
                PlayerPrefs.SetString("Speed", "Normal");
            }
        }
        else
        {
            PlayerPrefs.SetString("Speed", "Normal");
        }

        PlayerPrefs.Save();
    }

    public void SetSpeedNormal()
    {
        PlayerPrefs.SetString("Speed", "Normal");
        PlayerPrefs.Save();
    }

    public void SetSpeedHyper()
    {
        PlayerPrefs.SetString("Speed", "Hyper");
        PlayerPrefs.Save();
    }
}
