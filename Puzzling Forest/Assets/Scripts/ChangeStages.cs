using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStages : MonoBehaviour
{
    public bool one, two, three, four, five;
    public GameObject pan1, pan2, pan3, pan4, pan5;

    // Start is called before the first frame update
    void Start()
    {

        one = true;
        two = false;
        three = false;
        four = false;
        five = false;

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
        else if (one == true && two == false)
        {

            pan1.gameObject.SetActive(false);
            pan2.gameObject.SetActive(true);
            one = false;
            two = true;

        }

        else if (two == true && three == false)
        {

            pan2.gameObject.SetActive(false);
            pan3.gameObject.SetActive(true);
            two = false;
            three = true;

        }

        else if (three == true && four == false)
        {

            pan3.gameObject.SetActive(false);
            pan4.gameObject.SetActive(true);
            three = false;
            four = true;

        }

        else if (four == true && five == false)
        {

            pan4.gameObject.SetActive(false);
            pan5.gameObject.SetActive(true);
            four = false;
            five = true;

        }
    }

        public void MovePanelLeft()
        {

            if (five == false && one == true)
            {

                pan1.gameObject.SetActive(false);
                pan5.gameObject.SetActive(true);
                one = false;
                five = true;

            }
            else if (two == true && one == false)
            {

                pan2.gameObject.SetActive(false);
                pan1.gameObject.SetActive(true);
                two = false;
                one = true;

            }

            else if (three == true && two == false)
            {

                pan3.gameObject.SetActive(false);
                pan2.gameObject.SetActive(true);
                three = false;
                two = true;

            }

            else if (four == true && three == false)
            {

                pan4.gameObject.SetActive(false);
                pan3.gameObject.SetActive(true);
               four = false;
                three = true;

            }

            else if (five == true && four == false)
            {

                pan5.gameObject.SetActive(false);
                pan4.gameObject.SetActive(true);
                five = false;
                four = true;

            }



        }


}
