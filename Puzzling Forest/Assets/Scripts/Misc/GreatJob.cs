using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is attached to a game object that has a sphere collider. When a block hits this trigger,
// the fairy will say "Great Job".
public class GreatJob : MonoBehaviour
{
    private TurnManager turnManager;
    private AudioSource typingNoise;
    [Tooltip("What message should Summer say when a block enters this trigger? 'Great job.' and 'Well done.' use a small size, other messages will appear on a larger canvas.")]
    [SerializeField] private string message = "Great job.";

    // Start is called before the first frame update
    void Awake()
    {
        turnManager = this.transform.parent.GetComponent<TurnManager>();
        typingNoise = this.transform.Find("TypingNoise").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        else
        {
            Debug.LogFormat("{0} just triggered the 'Great Job' trigger", other.name);
            turnManager.Say("Great job.", typingNoise);
        }
    }
}
