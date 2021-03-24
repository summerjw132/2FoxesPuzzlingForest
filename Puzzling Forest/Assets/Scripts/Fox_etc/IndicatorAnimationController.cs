using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isFlyingOff = false;
    private bool isDrifting = false;
    private float driftSpeed = 0.5f;
    private float range = 0.2f;
    private Transform driftAnchor;
    private Vector3 centerPos;
    private Vector3 randPos;
    private bool goInactive = false;
    private bool isFinding = false;
    private Coroutine driftingRoutine = null;

    //speech stuff
    private GameObject speechCanvas;
    private Text fairyText;
    private Coroutine typer = null;
    private SpeechController speechController;
    private Camera cam;

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
            fairyText = speechCanvas.transform.Find("Background/Text").GetComponent<Text>();
            cam = GameObject.Find("GameManager/CameraControls/Camera").GetComponent<Camera>();

            speechController = new SpeechController(this.gameObject, speechCanvas, fairyText, cam);
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
            driftingRoutine = StartCoroutine(DriftAround());
    }

    void Update()
    {
        if (!isTutFairy && speechCanvas.activeInHierarchy)
        {
            speechController.UpdatePosition();
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
        if (isFlyingOff)
            return;
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
        isFlyingOff = true;
        randPos = new Vector3(driftAnchor.position.x, driftAnchor.position.y + 12, driftAnchor.position.z);
        driftSpeed = 15f;
        isDrifting = true;
        StopCoroutine(DelayFindRandomPosition());
        goInactive = true;
    }

    public void FlyDown()
    {
        isFlyingOff = false;
        goInactive = false;
        centerPos = driftAnchor.position;
        this.transform.position = new Vector3(centerPos.x, centerPos.y + 12, centerPos.z);
        randPos = centerPos;
        isDrifting = true;
        driftSpeed = 15f;
    }

    public float Say(string msg, AudioSource clip)
    {
        speechCanvas.SetActive(true);
        if (typer != null)
        {
            StopCoroutine(typer);
            typer = null;
        }   
        AutoResizeCanvas(msg);
        speechController.Clear();

        typer = StartCoroutine(Type(msg, clip));
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

    private IEnumerator Type(string msg, AudioSource clip)
    {
        for (int i = 0; i < msg.Length; i++)
        {
            speechController.AddChar(msg[i]);
            clip.Play();
            if (msg[i] == '.' || msg[i] == '!')
                yield return puncPause;
            else
                yield return typingPause;
        }

        yield return speechPause;
        speechController.StopTalking();
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

    public void ResizeCanvas(float width, float height)
    {
        speechController.resizeCanvas(width, height);
    }

    public void AutoResizeCanvas(string msg)
    {
        speechController.autoResizeCanvas(msg);
    }

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

        public SpeechController(GameObject _fairy, GameObject _canvas, Text _text, Camera _cam)
        {
            canvas = _canvas;
            text = _text;
            fairy = _fairy;
            cam = _cam;

            background = canvas.transform.Find("Background").GetComponent<RectTransform>();
            defaultColor = text.color;
        }

        public void Clear()
        {
            text.text = "";
            text.color = yellow;
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

        public void autoResizeCanvas(string msg)
        {
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
        }

        public void UpdatePosition()
        {
            Vector2 fairyScreenPos = cam.WorldToScreenPoint(fairy.transform.position);
            float newX = 0f;
            float newY = 0f;
            //fairy on right, so show text on left side
            if (fairyScreenPos.x > (cam.pixelWidth / 2f))
            {
                newX = fairyScreenPos.x - (background.rect.width / 1.9f);
                newY = fairyScreenPos.y + (background.rect.height / 1.8f);
            }
            //fairy on left, so show text on right side
            else
            {
                newX = fairyScreenPos.x + (background.rect.width / 1.9f);
                newY = fairyScreenPos.y + (background.rect.height / 1.8f);
            }
            background.position = new Vector3(newX, newY, -1f);

        }

        public struct SpeechInfo
        {
            public string msg;
            public float width;
            public float height;
            public SpeechInfo(string _msg, float _width, float _height)
            {
                msg = _msg;
                width = _width;
                height = _height;
            }
        }
    }
}
       
/// Whole Swap fox animation order :
/// 1) TurnManager.Swap Foxes()         //Start the animation of Fox A
/// 2) OnBallRaised()                   //Fox A animation finish, start to drop Fox B's ball
/// 3) OnBallHit()                      //Fox B's ball dropped on Fox B's head, play get hit animation
/// 4) FoxCharacter.completeAnimation() //Whole animation is done, turn manager will unlock for input.
/// 


