using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingObject : MonoBehaviour, IObjectWithState
{
    public CurrentState state = CurrentState.Static;
    [SerializeField] private Animator animator;

    CurrentState IObjectWithState.GetState()
    {
        return state;
    }

    void IObjectWithState.ChangeState(CurrentState newState)
    {
        Debug.LogError("Trying To change State of a BlockingObject which should never change");
    }
    public void Hit()
    {
        animator.SetTrigger("GetHit");
    }
}
