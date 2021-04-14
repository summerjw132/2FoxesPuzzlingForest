using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager for shading the image of the leaf for each button
/// </summary>
public class LeafManager : MonoBehaviour
{
    private bool isHighlighted = false;
    private static Color normal = new Color(1f, 1f, 1f, 1f);
    private static Color highlight = new Color(1f, .7f, 0f, 1f);
    private static Color inactive = new Color(1f, 1f, 1f, 0.6f);

    private Image image;
    private Button button;

    void Awake()
    {
        image = GetComponentInChildren<Image>();
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleLeafHighlight()
    {
        if (!button.interactable)
            image.color = inactive;
        else if (isHighlighted)
            image.color = normal;
        else
            image.color = highlight;

        isHighlighted = !isHighlighted;
    }

    public void ShadeLeafActive()
    {
        if (button.interactable)
        {
            image.color = normal;
        }

        else
        {
            image.color = inactive;
        }
    }
}
