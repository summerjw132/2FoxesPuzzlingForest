using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    private class SingleTurnState
    {
        public List<StateInfo> turnState;

        public void AddState(GameObject incomingGO)
        {
            turnState.Add(new StateInfo(incomingGO));
        }

        public void Reset()
        {
            turnState.Clear();
        }

        public SingleTurnState Copy()
        {
            SingleTurnState retState = new SingleTurnState(turnState);
            return retState;
        }



        //Construction
        public SingleTurnState(List<StateInfo> TurnState)
        {
            turnState = new List<StateInfo>(TurnState);
        }
        public SingleTurnState()
        {
            turnState = new List<StateInfo>();
        }
        //Print
        public override string ToString()
        {
            string retString = "Things that moved this turn and their original position:";
            for (int i = 0; i < turnState.Count; i++)
            {
                retString += turnState[i].ToString();
            }
            return retString;
        }
    }

    public struct StateInfo
    {
        public GameObject GO;
        public Vector3 position;
        public Quaternion rotation;

        public StateInfo(GameObject incomingGO)
        {
            GO = incomingGO;
            position = GO.transform.position;
            if (GO.CompareTag("Player"))
                rotation = GO.transform.Find("Fox").rotation;
            else
                rotation = GO.transform.rotation;
        }

        //Print
        public override string ToString()
        {
            string retString = "\n - GameObject: " + GO.name;
            retString += "\n    - " + position + "\n    - " + rotation;
            return retString;
        }
    }
    private TurnManager turnManager;

    private Stack<SingleTurnState> undoStack = new Stack<SingleTurnState>();
    private SingleTurnState curTurnState = new SingleTurnState();

    void Start()
    {
        turnManager = GameObject.Find("Turn-Based System").GetComponent<TurnManager>();
    }

    public void LogState(GameObject incomingGO)
    {
        //Debug.LogFormat("adding {0} to curTurnState", incomingGO.name);
        curTurnState.AddState(incomingGO);
    }

    public void WriteTurnState()
    {
        undoStack.Push(curTurnState.Copy());
        curTurnState.Reset();
    }

    public void UndoTurn()
    {
        if (undoStack.Count > 0)
        {
            SingleTurnState prevState = undoStack.Pop();

            foreach (StateInfo state in prevState.turnState)
            {
                state.GO.GetComponent<TurnBasedCharacter>().UndoMyTurn(state.position, state.rotation);
            }

            turnManager.totalMoveCount--;
            turnManager.undoCount++;
            turnManager.UpdateMoveCount();
        }
        else
        {
            //do nothing, someone hit undo before a move was made
            Debug.Log("Can't undo, no move is on stack");
        }
    }
}
