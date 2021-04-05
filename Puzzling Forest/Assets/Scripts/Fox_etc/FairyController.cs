using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles a couple of things.
///  1. The animations for the fairy flying up/off screen and back down.
///  2. Summer's "Dialogue" system. (In conjunction with turnManager to know which Fairy should be "talking" when).
///  3. NOTE: Tutorial fairies (Summer when disconnected from a fox) use a child class called TutFairyController.cs
/// </summary>
public class FairyController : MonoBehaviour
{
    //General
    protected foxAnimationStateController foxAnim;
    protected FoxCharacter fox;
    protected TurnManager turnManager;

    //Catch ball and Pass ball animation stuff
    static int dropCount;
    const float prob = 0.1f;
    int targetDrop;
    int flatDrop = 10;      // Used for flat number of drop
    int minDrop = 5, maxDrop = 10;  //Used for random number of drop
    const int MaxDrop = 400;
    bool isPlayDead = false;

    int algo = 3;
    /// Algorithm for determining when a fox should "flop" when Summer returns
    /// Swap algo to corresponding algo number
    /// 0) Flat rate, animation will play per "flatDrop" times
    /// 1) Random range, animation will play once after "targetDrop" times, which is within "minDrop" and "maxDrop"
    /// 2) Random chance per time, animation will have "prob" chance to play the animation each time
    /// 3) Random chance which raise per time, each time the animation will have "dropCount"/"MaxDrop" of chance to play. After the animation, dropCount will reset.
    /// 4) 100% chance of the flop animation playing

    //speech stuff
    protected GameObject speechCanvas;
    protected Text fairyText;
    protected Coroutine typer = null;
    protected SpeechController speechController;
    protected Camera cam;
    protected AudioSource typingNoise;

    protected readonly float typingSpeed = 0.05f;
    protected readonly float puncSpeed = 0.5f;
    protected readonly float speechPauseDuration = 3f;
    protected WaitForSeconds typingPause;
    protected WaitForSeconds puncPause;
    protected WaitForSeconds speechPause;

    protected virtual void Awake()
    {
        foxAnim = transform.parent.GetComponent<foxAnimationStateController>();
        fox = transform.parent.GetComponent<FoxCharacter>();
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();

        speechCanvas = this.gameObject.transform.GetChild(0).gameObject;
        fairyText = speechCanvas.transform.Find("Background/Text").GetComponent<Text>();
        cam = GameObject.Find("GameManager/CameraControls/Camera").GetComponent<Camera>();
        typingNoise = this.transform.Find("typingNoise").GetComponent<AudioSource>();
        speechController = new SpeechController(this.gameObject, speechCanvas, fairyText, cam);

        typingPause = new WaitForSeconds(typingSpeed);
        puncPause = new WaitForSeconds(puncSpeed);
        speechPause = new WaitForSeconds(speechPauseDuration);
    }

    protected virtual void Start()
    {
        switch (algo) //see comment on this at top
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

    void Update()
    {
        if (speechCanvas.activeInHierarchy) //keeps the text box attached to Summer and on-screen
        {
            speechController.UpdatePosition();
        }
    }

    /// Whole Swap fox animation order :
    /// 1) TurnManager.Swap Foxes()         //Start the animation of Fox A
    /// 2) OnBallRaised()                   //Fox A animation finish, start to drop Fox B's ball
    /// 3) OnBallHit()                      //Fox B's ball dropped on Fox B's head, play get hit animation
    /// 4) FoxCharacter.completeAnimation() //Whole animation is done, turn manager will unlock for input.
    /// 

    public void OnBallHit()
    {
        //determines whether or not to play the flop animation
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
                    break;
                }
            case 4:
                {
                    isPlayDead = true;
                    break;
                }
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

    //Starting here is stuff for the dialogue system

    //These are wrapper functions for going through the turn manager. This is because these were created as turn indicators originally.
    // Because of that, each fox has its own TurnIndicator/Summer/Fairy object. But they need to act as if there's only one in terms of
    // personification/dialogue. As such, the methods for keeping track of speech progress and clearing the box etc. all need to go
    // through turnManager so that that script can handle updating all Summers together and only telling the active one to talk etc.
    private void ResetSpeechProgress()
    {
        turnManager.ResetFairySpeechProgress();
    }

    private void IncrementSpeechProgress()
    {
        turnManager.IncrementFairySpeechProgress();
    }

    private void StopTalking()
    {
        turnManager.StopTalking();
    }

    private void ClearDialogue()
    {
        turnManager.ClearDialogue();
    }

    //These are the functions that actually affect this particular Summer. TurnManager calls these for the appropriate fairies whenever
    // an individual fairy uses one of the above wrappers to ask the TurnManager to update things accordingly.
    public void resetMyProgress()
    {
        speechController.resetProgress();
    }

    public void incrementMyProgress()
    {
        speechController.incrementProgress();
    }

    public void stopTalking()
    {
        speechController.StopTalking();
    }

    public void clearDialogue()
    {
        speechController.Clear();
    }

    public void ShutUp()
    {
        if (typer != null)
        {
            StopCoroutine(typer);
            typer = null;
        }
        speechController.StopTalking();
    }

    //public method for Summer "saying" a message. Note that when displaying dialogue you should
    // use TurnManager.Say() because that way the turn manager will tell the active fairy to speak.
    public float Say(string msg)
    {
        speechCanvas.SetActive(true);
        if (typer != null)
        {
            StopCoroutine(typer);
            typer = null;
            ResetSpeechProgress();
        }   
        SetUpCanvas(msg);

        typer = StartCoroutine(Type(msg));
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

    //Both of the Type Coroutines that are in this class are for the Typing effect
    // during dialogues. Don't try to use them manually, see the above method.
    private IEnumerator Type(string msg)
    {
        for (int i = 0; i < msg.Length; i++)
        {
            speechController.AddChar(msg[i]);
            IncrementSpeechProgress();
            typingNoise.Play();
            if (msg[i] == '.' || msg[i] == '!')
                yield return puncPause;
            else
                yield return typingPause;
        }

        yield return speechPause;
        ResetSpeechProgress();
        ClearDialogue();
        StopTalking();
    }

    //method for the continuation of a message from Summer. Used
    // if E is pressed while she's talking. Do not manually call this.
    public float KeepSaying(SpeechController.KeepTalkingInfo info)
    {
        if (info.msg == "") //nothing to say.
        {
            return -1f;
        }

        speechCanvas.SetActive(true);
        if (typer != null)
        {
            StopCoroutine(typer);
            typer = null;
        }
        SetUpCanvas(info.msg);

        for (int i = 0; i < info.progress; i++)
        {
            speechController.AddChar(info.msg[i]);
        }

        typer = StartCoroutine(TypeFromI(info.msg, info.progress));
        float duration = speechPauseDuration;
        for (int i = info.progress; i < info.msg.Length; i++)
        {
            if (info.msg[i] == '.' || info.msg[i] == '!')
                duration += puncSpeed;
            else
                duration += typingSpeed;
        }
        return duration;
    }

    private IEnumerator TypeFromI(string msg, int startDex)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = startDex; i < msg.Length; i++)
        {
            speechController.AddChar(msg[i]);
            IncrementSpeechProgress();
            typingNoise.Play();
            if (msg[i] == '.' || msg[i] == '!')
                yield return puncPause;
            else
                yield return typingPause;
        }

        yield return speechPause;
        ResetSpeechProgress();
        ClearDialogue();
        StopTalking();
    }

    //The following are all for formatting/sizing/positioning the text box Summer uses to speak.
    public void ResizeCanvas(float width, float height)
    {
        speechController.resizeCanvas(width, height);
    }

    public void SetUpCanvas(string msg)
    {
        speechController.SetUpCanvas(msg);
    }

    public SpeechController.KeepTalkingInfo GetSpeechInfo()
    {
        return speechController.GetInfo();
    }

    //A class that solely tracks info regarding the text box and has useful methods for positioning, updating, etc.
    public class SpeechController
    {
        private GameObject fairy;
        private GameObject canvas;
        private Text text;
        private Camera cam;

        private RectTransform background;

        private static Color defaultColor;
        private static Color yellow = new Color(0.98f, 1f, 0f, 1f);
        private static Color invisible = new Color(1f, 1f, 1f, 0f);
        private static Vector2 defaultWidth = new Vector2(200f, 10f);
        private static Vector2 shortWidth = new Vector3(120f, 10f);
        private static string[] goodJobbers = new string[4] { "great job!", "good job!", "well done!", "nice job!" };
        private bool useShort = false;

        //A custom struct for keeping track of dialogue progress in the event the player swaps foxes during dialogue.
        private KeepTalkingInfo myInfo;

        public SpeechController(GameObject _fairy, GameObject _canvas, Text _text, Camera _cam)
        {
            canvas = _canvas;
            text = _text;
            fairy = _fairy;
            cam = _cam;

            background = canvas.transform.Find("Background").GetComponent<RectTransform>();
            defaultColor = text.color;

            myInfo = new KeepTalkingInfo("", 0);
        }

        public void Clear()
        {
            text.text = "";
            text.color = yellow;

            myInfo.msg = "";
            myInfo.progress = 0;
        }

        public void resetProgress()
        {
            myInfo.progress = 0;
        }

        public void incrementProgress()
        {
            myInfo.progress++;
        }

        public void AddChar(char nextChar)
        {
            text.text += nextChar;
        }

        public void StopTalking()
        {
            canvas.SetActive(false);
        }

        public void resizeCanvas(float width, float height)
        {
            background.sizeDelta = new Vector2(width, height);
            UpdatePosition();
        }

        public void SetUpCanvas(string msg)
        {
            myInfo.msg = msg;

            //Height = fontSize * #rows + 6;
            useShort = false;
            for (int i = 0; i < goodJobbers.Length; i++)
            {
                if (goodJobbers[i] == msg.ToLower())
                {
                    background.sizeDelta = shortWidth;
                    useShort = true;
                }
            }
            if (!useShort)
                background.sizeDelta = defaultWidth;

            text.color = invisible;
            text.text = msg;
            Canvas.ForceUpdateCanvases();
            background.sizeDelta = new Vector2(background.rect.width, (text.cachedTextGenerator.lineCount * text.fontSize) + 6);
            text.text = "";
            text.color = yellow;
        }

        public void UpdatePosition()
        {
            Vector2 fairyScreenPos = cam.WorldToScreenPoint(fairy.transform.position);
            float newX;
            float newY;
            //fairy on right, so show text on left side
            if (fairyScreenPos.x > (cam.pixelWidth / 2f))
            {
                newX = fairyScreenPos.x - (background.rect.width / 1.9f);
            }
            //fairy on left, so show text on right side
            else
            {
                newX = fairyScreenPos.x + (background.rect.width / 1.9f);
            }

            //Strong preference for text above fairy, but display below if necessary
            if (fairyScreenPos.y < (cam.pixelHeight * 0.8f))
            {
                newY = fairyScreenPos.y + (background.rect.height / 1.8f);
            }
            else
            {
                newY = fairyScreenPos.y - (background.rect.height / 1.8f);
            }

            background.position = new Vector3(newX, newY, -1f);
        }

        //Struct for passing the necessary information to the TurnManager in order to
        // have the next Summer pick up from where this one left off if the player swaps
        // foxes or otherwise interrupts Summer.
        public struct KeepTalkingInfo
        {
            public string msg;
            public int progress;

            public KeepTalkingInfo(string _msg, int _progress)
            {
                msg = _msg;
                progress = _progress;
            }

            public override string ToString()
            {
                string retString = "msg: " + msg;
                retString += "\nprogress: " + progress;

                return retString;
            }
        }

        public KeepTalkingInfo GetInfo()
        {
            return myInfo;
        }
    }
}