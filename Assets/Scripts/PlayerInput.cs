using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Lean.Touch;
using System;

public class PlayerInput : MonoBehaviour
{
    public float speed = 1f;
    private Camera maincam;

    private MoveableObject CurrentObject;
    private Vector3 FingerStartPoint;
    public float FingerDragDist = 0.1f;
    RaycastHit hit;
    private void Start()
    {
        maincam = Camera.main;
    }


    void OnEnable()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
    }

    

    void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;

    }
    private void OnFingerDown(LeanFinger finger)
    {
        Ray ray = maincam.ScreenPointToRay(finger.ScreenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            FingerStartPoint = hit.point;
            var hittarget = hit.transform.GetComponent<MoveableObject>();
            if (hittarget != null)
            {
                //Debug.Log("Hit a movable object and its name is - " + hit.transform.name);
                CurrentObject = hittarget;
            }
            else
            {
                //Debug.Log("Thats Not A Movable Object ");
            }
        }
    }
    private void OnFingerUpdate(LeanFinger finger)
    {
        if (CurrentObject != null)
        {
            Ray ray = maincam.ScreenPointToRay(finger.ScreenPosition);
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Fingie Coords are - " + hit.point);
            }
            if (Vector3.Distance(hit.point,FingerStartPoint)>FingerDragDist) //when the finger is far enough from start point .
            {
                var signedDirection =Vector3.Dot((hit.point - FingerStartPoint),CurrentObject.transform.forward); // get dotproduct to find out if the finger was swiped the same way or backwards
                                                                                                                  // in comparison to the forward of our object without the Y coords for better accuracy
                //Debug.Log("SignedDirection Was " + signedDirection);
                if (signedDirection>0)
                {
                    CurrentObject.Move(true);
                    //Debug.Log("WasDraggedTowardsFrontOfCar");
                }
                else
                {
                    CurrentObject.Move(false);
                    //Debug.Log("WasDraggedTowardsBackOfCar");

                }
                CurrentObject = null;
            }
        }
        

    }
    private void OnDrawGizmos()
    {
        if (hit.point != null)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.05f);
        }
    }
}
