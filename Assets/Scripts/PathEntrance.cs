using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathEntrance : MonoBehaviour, IObjectWithState
{
    public CurrentState state = CurrentState.PathEntrance;
    [SerializeField] Transform[] RestOfPath;

    void IObjectWithState.ChangeState(CurrentState newState)
    {
        Debug.LogError("Trying To change State of a PathEntrance which should never change");
    }

    CurrentState IObjectWithState.GetState()
    {
        return state;
    }

    public Transform[] returnPath()
    {
        return RestOfPath;
    }
}
