using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{

    public bool one, two, three, four, five;
    public GameObject pan1, pan2, pan3, pan4, pan5;

    // Start is called before the first frame update
    void Start()
    {

        one = true;

    }

    // Update is called once per frame
    void Update()
    {

        
        
    }

    public void MovePanelRight()
    {

        if (one == false && five == true)
        {

            pan5.gameObject.SetActive(false);
            pan1.gameObject.SetActive(true);
            five = false;
            one = true;

        }
        if (one == true && two == false)
        {

            pan1.gameObject.SetActive(false);
            pan2.gameObject.SetActive(true);
            one = false;
            two = true;

        }

        if (two == true && three == false)
        {

            pan2.gameObject.SetActive(false);
            pan3.gameObject.SetActive(true);
            two = false;
            three = true;

        }

        if (three == true && four == false)
        {

            pan3.gameObject.SetActive(false);
            pan4.gameObject.SetActive(true);
            three = false;
            four = true;

        }

        if (four == true && five == false)
        {

            pan4.gameObject.SetActive(false);
            pan5.gameObject.SetActive(true);
            four = false;
            five = true;

        }




    }


}
