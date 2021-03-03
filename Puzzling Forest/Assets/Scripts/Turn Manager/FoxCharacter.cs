using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxCharacter : TurnBasedCharacter
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This function is part of what will be called when the player swaps foxes.
    ///  I'm envisioning this as just handling the animation stuff, I will have a second function
    ///  in the TurnMaanager script for actually changing whose turn it is.
    /// </summary>
    private void PassTheBall()
    {

    }

    public override void SpecialAction()
    {
        throw new System.NotImplementedException();
    }
}
