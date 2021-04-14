using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowHide : MonoBehaviour
{
    [Tooltip("The ordered list of text fields that are cycled by this function.")]
    [SerializeField]
    private List<Text> listOfTextFields = new List<Text>();

    private int currentTextFieldIndex = 0;

    private void Awake()
    {
        InitilizeTextList();
    }

    public void CycleToNextText() // Used to cycle the display of a list of text fields
    {
        if (listOfTextFields.Count > 1) // If there is more than 1 item in the list, we can cycle to the next item
        {
            HideAllTextFields();

            ShowNextTextField();
        }
    }

    private void InitilizeTextList()
    {
        HideAllTextFields();
        if (listOfTextFields.Count > 0)
        {
            listOfTextFields[0].enabled = true;
        }
    }

    private void ShowNextTextField() //Shows the next text field in the list
    {
        if (currentTextFieldIndex >= listOfTextFields.Count - 1) // If we reached the end of the list, cycle to first item 
        {
            currentTextFieldIndex = 0;
        }
        else // else, go to the next item on the list
        {
            currentTextFieldIndex++;
        }

        listOfTextFields[currentTextFieldIndex].enabled = true;
    }

    private void HideAllTextFields() // Hides all the text fields in the list
    {
        foreach (Text field in listOfTextFields)
        {
            field.enabled = false;
        }
    }


}
