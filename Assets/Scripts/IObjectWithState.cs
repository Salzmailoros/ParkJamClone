using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectWithState
{
    CurrentState GetState();
    void ChangeState(CurrentState newState);
}

public enum CurrentState
{
    Still,
    Moving,
    Solved,
    PathEntrance,
    Static
}