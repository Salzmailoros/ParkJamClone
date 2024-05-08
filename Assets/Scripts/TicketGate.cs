using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketGate : MonoBehaviour
{
    public Transform rotatingPart;
    public float rotationSpeed = 5f;
    public float ClosedRotationX;
    public float OpenRotationX;

    private bool isTriggered = false;

    void FixedUpdate()
    {
        if (isTriggered)
        {
            RotateGate(new Vector3(OpenRotationX, 0, 0));
        }
        else
        {
            RotateGate(new Vector3(ClosedRotationX, 0, 0));
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            isTriggered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }

    void RotateGate(Vector3 targetRotationEuler)
    {
        Quaternion targetRotationQuat = Quaternion.Euler(targetRotationEuler);
        rotatingPart.rotation = Quaternion.Lerp(rotatingPart.rotation, targetRotationQuat, rotationSpeed * Time.deltaTime);
    }
}