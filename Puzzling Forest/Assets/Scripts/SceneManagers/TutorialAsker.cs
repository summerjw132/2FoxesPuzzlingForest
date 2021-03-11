using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAsker : MonoBehaviour
{
    private GameObject level1;
    private LevelManager levelManager;

    // Start is called before the first frame update
    void Awake()
    {
        level1 = this.gameObject.transform.parent.Find("Level").gameObject;

        try
        {
            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        }
        catch
        {
            Debug.Log("levelManager not found. Expected if you loaded a scene/level directly");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoTutorial()
    {
        if (levelManager)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Tutorials/PlayerTutorial");
        }
    }

    public void SkipTutorial()
    {
        level1.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
