using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class MoveableObject : MonoBehaviour, IObjectWithState
{
    public CurrentState objectState;
    public float Speed;
    public float checkDist;

    private int fwdMultiplier = 1;
    private RaycastHit hit;

    public Animator animator;
    [SerializeField] private bool forward; // if going forward set this true. if going backwards set it false. tickmark means true. empty mark is false
    [SerializeField] private ParticleSystem happyParticle;
    [SerializeField] private ParticleSystem madParticle;

    // ------------------- INTERFACE FUNCTIONS ------------------- \\ 
    CurrentState IObjectWithState.GetState()
    {
        return objectState;
    }

    void IObjectWithState.ChangeState(CurrentState newState)
    {
        object state = newState;
    }

    // ------------------- PUBLIC FUNCTIONS ------------------- \\ 
    public void Move()
    {
        objectState = CurrentState.Moving;
        if (forward ) { fwdMultiplier = 1; }
        else { fwdMultiplier = -1; }
        LogicCheck();// to make sure its not right up against an object so it moves again.
        //Debug.Log(this.name + "is moving" + fwdMultiplier);
    }

    // ------------------- PRIVATE FUNCTIONS & MAIN CAR LOGIC ------------------- \\ 
    private void FixedUpdate()
    {
        if (objectState == CurrentState.Moving)
        {
            LogicCheck();
            transform.Translate(Vector3.forward * Time.deltaTime*fwdMultiplier * Speed);
        }
    }


    private void LogicCheck() 
    {
        
        //Ray objectray = new Ray(transform.position + Vector3.up * 0.15f, transform.forward * 0.1f * checkDist * fwdMultiplier);
        //Debug.DrawRay(transform.position + Vector3.up * 0.15f, transform.forward *  checkDist * fwdMultiplier, Color.green);
        
        bool boxhit = Physics.BoxCast(transform.position + Vector3.up * 0.15f, Vector3.one * 0.05f, transform.forward * 0.1f * checkDist * fwdMultiplier, out hit,transform.rotation,checkDist);
        if (boxhit)
        {
            if (hit.collider != null)
            {
                IObjectWithState objstate = hit.transform.gameObject.GetComponent<IObjectWithState>();
                //Debug.Log(hit.transform.gameObject.name);

                switch (objstate.GetState())
                {
                    case CurrentState.Moving:   // hitting a moving car thats not on its way out of the level
                        Debug.Log("Object in front of me - " + hit.transform.gameObject.name + " - is Moving. ");
                        Debug.LogError("Two Objects Shouldnt be moving at the same time - completed cars should be in state -Solved- not -Moving- And " +
                            "Input Should be Locked while a car is moving already");
                        // can be a future feature to have some interesting tech if this were ok with being a complicated game.
                        break;

                    case CurrentState.Solved:   // hitting a moving car that did complete its puzzle and is leaving level
                        //Debug.Log("Ive hit a car on its Way out I must wait. - " + name);
                        // Wait for the guy and try to get on the road again
                        objectState = CurrentState.Still;
                        madParticle.Play();
                        DOTween.To(() => 0f, x => { }, 1, 0.25f).OnComplete(() => CheckAgain());
                        break;

                    case CurrentState.Static:
                        //Debug.Log("-" + name + " - I hit a Static Object and stopped");
                        HitBlockingObject();
                        
                        break;

                    case CurrentState.Still:
                        //Debug.Log("-" + name + " - I hit a non Moving But movable Object and Ive stopped");
                        Stop();
                        if (fwdMultiplier>0)
                        {
                            animator.SetTrigger("HitFront");

                        }
                        else
                        {
                            animator.SetTrigger("HitBack");
                        }
                        madParticle.Play();
                        hit.transform.GetComponent<Animator>().SetTrigger("Shake");
                        break;
                    case CurrentState.PathEntrance:
                        //Debug.Log("WOO I FOUND AN EXIT - " + name);
                        FollowPath(hit.transform.GetComponent<PathEntrance>().returnPath());
                        //pull into path and leave
                        happyParticle.Play();
                        objectState = CurrentState.Solved;
                        break;

                    default:
                        Debug.LogError("-" + name + " - Has hit something thats not an object with state");

                        break;
                }

            }
        }
    }

    private void CheckAgain()
    {
        LogicCheck();
        objectState = CurrentState.Moving;
        //Debug.Log("CheckedAndMovingAgain.");
    }
    private void Stop()
    {
        objectState = CurrentState.Still;
    }
    private void HitBlockingObject()
    {
        if (fwdMultiplier>0)
        {
            animator.SetTrigger("HitFront");
            madParticle.Play();
        }
        else
        {
            animator.SetTrigger("HitBack");
            madParticle.Play();
        }
        Stop();
        hit.transform.GetComponent<BlockingObject>().Hit();
    }
    private void FollowPath(Transform[] restofPoints)
    {
        Vector3[] pathArray;
        if (fwdMultiplier>0)
        {
            // pull in motion points generation.
            Vector3[] pulloutPath = new Vector3[1];
            pulloutPath[0] = transform.position;
            //pulloutPath[1] = transform.position + transform.forward * 0.3f * checkDist * 10;
            pathArray = new Vector3[restofPoints.Length+1];
            
            pathArray[0] = transform.position + transform.forward * 0.3f * checkDist * 10;
            for (int i = 1; i < pathArray.Length; i++)
            {
                pathArray[i] = restofPoints[i-1].position;
            }
            float pathlength=0f;
            for (int i = 0; i < pathArray.Length-1; i++)
            {
                pathlength += Vector3.Distance(pathArray[i] , pathArray[i + 1]);
            }
            transform.DOPath(pulloutPath, Vector3.Distance(pulloutPath[0],transform.position) / Speed , PathType.Linear).SetEase(Ease.Linear)
                .OnComplete(() => transform.DOPath(pathArray,  pathlength / Speed, PathType.CatmullRom).SetLookAt(0.1f).SetEase(Ease.Linear));
            

        }
        else 
        {
            // Back out Motion points generation.

            Vector3[] pulloutPath = new Vector3[2];
            pulloutPath[0] = transform.position;
            pulloutPath[1] = transform.position + transform.forward * checkDist * -0.6f * 10;
            pathArray = new Vector3[restofPoints.Length + 1];
            pathArray[0] = transform.position + transform.forward * checkDist *- 0.3f*10;
            for (int i = 1; i < pathArray.Length; i++)
            {
                pathArray[i] = restofPoints[i-1].position;
            }
            float pathlength = 0f;
            for (int i = 0; i < pathArray.Length - 1; i++)
            {
                pathlength += Vector3.Distance(pathArray[i], pathArray[i + 1]);
            }
            transform.DOPath(pulloutPath, Vector3.Distance(pulloutPath[0], pulloutPath[1]) / Speed , PathType.Linear).SetEase(Ease.Linear)
                .OnComplete(() => transform.DOPath(pathArray,  pathlength / Speed , PathType.CatmullRom).SetLookAt(0.1f).SetEase(Ease.Linear)); ;

        }
        
    }
    private void OnDrawGizmos()
    {
        if (Physics.BoxCast(transform.position + Vector3.up * 0.15f, Vector3.one * 0.1f, transform.forward * 0.1f * checkDist, out hit,transform.rotation,checkDist))      //for level design. checkdist gizmo front
        {
            Gizmos.color=Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.15f + transform.forward * checkDist*fwdMultiplier*2, Vector3.one * 0.15f);
        }
        if (Physics.BoxCast(transform.position + Vector3.up * 0.15f, Vector3.one * 0.05f, transform.forward* (-1f) * 0.1f * checkDist, out hit, transform.rotation, checkDist))// gizmo for back
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.15f + transform.forward * checkDist  *(-1f) * 2, Vector3.one * 0.15f);
        }

    }
}
