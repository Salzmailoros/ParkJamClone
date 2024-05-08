using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameAndLevelControl : MonoBehaviour
{
    [SerializeField] GameObject[] Levels;
    private int CurrLevel;
    private GameObject lastlvl;
    [SerializeField] ParticleSystem WinLevelParticle;
    [SerializeField] float levelendSeqLength = 1.5f;
    [SerializeField] GameObject NextLevelButton;
    private void Start()
    {
        Application.targetFrameRate = 120;
        CurrLevel = -1;
        NextLevelSequence();
    }
    public void RestartLevel()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void NextLevel()
    {
        WinLevelParticle.Play();
        
        DOTween.To(() => 0f, x => { }, 1, levelendSeqLength).OnComplete(() => NextLevelButton.SetActive(true) );
    }
    public void NextLevelSequence()
    {
        WinLevelParticle.Clear();
        NextLevelButton.SetActive(false);
        if (lastlvl != null) { Destroy(lastlvl); }
        if (CurrLevel + 1 < Levels.Length)
        {
            CurrLevel++;
        }
        else
        {
            CurrLevel = 0;
        }
        Debug.Log("LoadingLevel Named : " + Levels[CurrLevel]);
        lastlvl = Instantiate(Levels[CurrLevel]);
    }
}
