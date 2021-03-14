using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAnimationController : MonoBehaviour
{
    foxAnimationStateController foxAnim;
    FoxCharacter fox;
    TurnManager turnManager;
    // Start is called before the first frame update
    void Start()
    {
        foxAnim = transform.parent.GetComponent<foxAnimationStateController>();
        fox = transform.parent.GetComponent<FoxCharacter>();
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBallHit()
    {
        //Ask Fox B to play hit animation
        foxAnim.startCatchTheBall();
    }

    public void OnBallRaised()
    {
        //Ask Fox B's indicator to drop the ball
        fox.ToggleIndicator(false);
        turnManager.SwappedFoxes();
    }
}
/// Whole Swap fox animation order :
/// 1) TurnManager.Swap Foxes()         //Start the animation of Fox A
/// 2) OnBallRaised()                   //Fox A animation finish, start to drop Fox B's ball
/// 3) OnBallHit()                      //Fox B's ball dropped on Fox B's head, play get hit animation
/// 4) FoxCharacter.completeAnimation() //Whole animation is done, turn manager will unlock for input.
