using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private int LevelCarAmnt;
    private int CurrentCarAmnt;
    private GameAndLevelControl levelController;
    private void Start()
    {
         levelController = FindObjectOfType<GameAndLevelControl>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null )
        {
            CurrentState otherState = other.GetComponent<IObjectWithState>().GetState();
            if (otherState == CurrentState.Solved)
            {
                CurrentCarAmnt++;
            }
            if (CurrentCarAmnt >= LevelCarAmnt)
            {
                levelController.NextLevel();
            }
        }
    }
}
