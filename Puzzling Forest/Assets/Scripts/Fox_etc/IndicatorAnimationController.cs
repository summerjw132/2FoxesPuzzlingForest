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
    /// 

    //random drift/float effect
    [SerializeField] private bool isTutFairy = false;
    private float rand_x;
    private float rand_y;
    private float rand_z;
    private bool isDrifting = false;
    private float driftSpeed = 0.5f;
    private float range = 0.2f;
    private Transform driftAnchor;
    private Vector3 centerPos;
    private Vector3 randPos;
    private bool goInactive = false;
    private bool isFinding = false;

    //speech stuff
    private Vector3 rightSide = new Vector3(-3.87f, 1.06f, 1.29f);
    private Vector3 leftSide = new Vector3(1.16f, 1.06f, -3.74f);
    private GameObject speechCanvas;
    private UnityEngine.UI.Text fairyText;

    private readonly float typingSpeed = 0.05f;
    private readonly float puncSpeed = 0.5f;
    private readonly float speechPauseDuration = 3f;
    private WaitForSeconds typingPause;
    private WaitForSeconds puncPause;
    private WaitForSeconds speechPause;

    void Awake()
    {
        foxAnim = transform.parent.GetComponent<foxAnimationStateController>();
        fox = transform.parent.GetComponent<FoxCharacter>();
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
        if (isTutFairy)
            driftAnchor = this.gameObject.transform.parent.transform.parent.transform.Find("DriftAnchor");
        else
        {
            speechCanvas = this.gameObject.transform.GetChild(0).gameObject;
            fairyText = speechCanvas.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
        }
        typingPause = new WaitForSeconds(typingSpeed);
        puncPause = new WaitForSeconds(puncSpeed);
        speechPause = new WaitForSeconds(speechPauseDuration);
    }

    void Start()
    {
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

        if (isTutFairy)
            StartCoroutine(DriftAround());
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

    private IEnumerator DriftAround()
    {
        while (true)
        {
            DriftAroundFunc();
            yield return null; //waits one frame
        }
    }

    private void DriftAroundFunc()
    {
        if (isDrifting)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, randPos, driftSpeed * Time.deltaTime);
            if (this.transform.position == randPos)
            {
                isDrifting = false;
                if (goInactive)
                    this.gameObject.transform.parent.gameObject.SetActive(false);
            }
                
        }
        else
        {
            StartCoroutine(DelayFindRandomPosition());
        }
    }

    private IEnumerator DelayFindRandomPosition()
    {
        if (isFinding)
        {
            yield break;
        }
        isFinding = true;
        yield return new WaitForSeconds(0.1f);
        FindRandomPosition();
    }

    private void FindRandomPosition()
    {
        centerPos = driftAnchor.position;

        rand_x = centerPos.x + Random.Range(-range, range);
        rand_y = centerPos.y + Random.Range(-range, range);
        rand_z = centerPos.z + Random.Range(-range, range);

        randPos = new Vector3(rand_x, rand_y, rand_z);

        isDrifting = true;
        driftSpeed = 0.5f;

        isFinding = false;
    }

    public void FlyOff()
    {
        randPos = new Vector3(driftAnchor.position.x, driftAnchor.position.y + 12, driftAnchor.position.z);
        driftSpeed = 15f;
        isDrifting = true;
        StopCoroutine(DelayFindRandomPosition());
        goInactive = true;
    }

    public void FlyDown()
    {
        goInactive = false;
        centerPos = driftAnchor.position;
        this.transform.position = new Vector3(centerPos.x, centerPos.y + 12, centerPos.z);
        randPos = centerPos;
        isDrifting = true;
        driftSpeed = 15f;
    }

    public float Say(string msg)
    {
        if (isTutFairy)
            return -1f;

        SetSide();
        StartCoroutine(Type(msg));
        float duration = speechPauseDuration;
        for (int i = 0; i < msg.Length; i++)
        {
            if (msg[i] == '.' || msg[i] == '!')
                duration += puncSpeed;
            else
                duration += typingSpeed;
        }
        return duration;
    }

    public float Say(string msg, AudioSource clip)
    {
        if (isTutFairy)
            return -1f;

        SetSide();
        StartCoroutine(Type(msg, clip));
        float duration = speechPauseDuration;
        for (int i = 0; i < msg.Length; i++)
        {
            if (msg[i] == '.' || msg[i] == '!')
                duration += puncSpeed;
            else
                duration += typingSpeed;
        }
        return duration;
    }

    private IEnumerator Type(string msg)
    {
        speechCanvas.SetActive(true);
        fairyText.text = "";
        string current = "";

        for (int i = 0; i < msg.Length; i++)
        {
            current += msg[i];
            fairyText.text = current;
            if (msg[i] == '.' || msg[i] == '!')
                yield return puncPause;
            else
                yield return typingPause;
        }

        yield return speechPause;
        speechCanvas.SetActive(false);
    }

    private IEnumerator Type(string msg, AudioSource clip)
    {
        if (clip.volume > 0.05f)
            clip.volume = 0.05f;

        speechCanvas.SetActive(true);
        fairyText.text = "";
        string current = "";

        for (int i = 0; i < msg.Length; i++)
        {
            current += msg[i];
            fairyText.text = current;
            clip.Play();
            if (msg[i] == '.' || msg[i] == '!')
                yield return puncPause;
            else
                yield return typingPause;
        }

        yield return speechPause;
        speechCanvas.SetActive(false);
    }

    public void StopTalking()
    {
        speechCanvas.SetActive(false);
    }

    private void SetSide()
    {
        string side = fox.LeftOrRight();
        switch (side)
        {
            case "left":
                speechCanvas.transform.localPosition = leftSide;
                break;

            case "right":
                speechCanvas.transform.localPosition = rightSide;
                break;

            default:
                break;
        }
    }

    public void resizeCanvas(float width, float height)
    {
        speechCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }

    public void updatePosition(Vector3 newLeftSide, Vector3 newRightSide)
    {
        leftSide = newLeftSide;
        rightSide = newRightSide;
    }
}
       
/// Whole Swap fox animation order :
/// 1) TurnManager.Swap Foxes()         //Start the animation of Fox A
/// 2) OnBallRaised()                   //Fox A animation finish, start to drop Fox B's ball
/// 3) OnBallHit()                      //Fox B's ball dropped on Fox B's head, play get hit animation
/// 4) FoxCharacter.completeAnimation() //Whole animation is done, turn manager will unlock for input.
/// 


