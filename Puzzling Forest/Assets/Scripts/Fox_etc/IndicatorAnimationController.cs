using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAnimationController : MonoBehaviour
{
    foxAnimationStateController foxAnim;
    FoxCharacter fox;
    TurnManager turnManager;


    static int dropCount;
    const float prob = 0.1f;
    int targetDrop;
    int flatDrop = 10;      // Used for flat number of drop
    int minDrop = 5, maxDrop = 10;  //Used for random number of drop
    const int MaxDrop = 400;
    bool isPlayDead = false;

    int algo = 3; 
    /// Algorithm
    /// Swap algo to corresponding algo number
    /// 0) Flat rate, animation will play per "flatDrop" times
    /// 1) Random range, animation will play once after "targetDrop" times, which is within "minDrop" and "maxDrop"
    /// 2) Random chance per time, animation will have "prob" chance to play the animation each time
    /// 3) Random chance which raise per time, each time the animation will have "dropCount"/"MaxDrop" of chance to play. After the animation, dropCount will reset.
 
    // Start is called before the first frame update
    void Start()
    {
        foxAnim = transform.parent.GetComponent<foxAnimationStateController>();
        fox = transform.parent.GetComponent<FoxCharacter>();
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
        switch (algo)
        {
            case 0:
                targetDrop = flatDrop;
                break;
            case 1:
                targetDrop = Random.Range(minDrop, maxDrop);
                break;
            default:
                break;
        }
    }
    public void OnBallHit()
    {
        switch (algo)
        {
            case 0:
            case 1:
                isPlayDead = dropCount > targetDrop;
                break;
            case 2:
                {
                    float rand = Random.value;
                    isPlayDead = prob > rand;
                    break;
                }
            case 3:
                {
                    float chance = (float)dropCount / MaxDrop;
                    float rand = Random.value;
                    isPlayDead = chance > rand;
                }
                break;
            default:
                break;
        }
        if (isPlayDead)
        {
            //If have achievement system, add"Don't hurt your companion"
            foxAnim.startCatchTheBall();
            dropCount = 0;
            switch (algo)
            {
                case 1:
                    targetDrop = Random.Range(minDrop, maxDrop);
                    break;
                default:
                    break;
            }
        }
        else
            dropCount++;
    }

    public void OnBallRaised()
    {
        //Ask Fox B's indicator to drop the ball
        fox.ToggleIndicator(false);
        turnManager.SwappedFoxes();
    }

    public void OnBallDropped()
    {
        if (!isPlayDead)
        {
            fox.completeAnimation();
        }
    }
}
/// Whole Swap fox animation order :
/// 1) TurnManager.Swap Foxes()         //Start the animation of Fox A
/// 2) OnBallRaised()                   //Fox A animation finish, start to drop Fox B's ball
/// 3) OnBallHit()                      //Fox B's ball dropped on Fox B's head, play get hit animation
/// 4) FoxCharacter.completeAnimation() //Whole animation is done, turn manager will unlock for input.
/// 


