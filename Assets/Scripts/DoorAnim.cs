using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{

    public enum DoorStates
    {
        IdleState,
        DoorOpen,
        DoorClose
    }

    void setCurrentState(DoorStates state)
    {
        currentState = state;
        LastStateChange = Time.time;
    }

    public LayerMask Player;
    public DoorStates currentState;


    Vector3 StartPos;
    Vector3 EndPos;
    Vector3 EndPos2;

    public float Distance = 0.5f;
    private float Distance2 = 2f;
    public float Speed = 2f;
    public float currentSpeed = 0f;

    private float LastStateChange = 0.0f;

    void Start()
    {
        setCurrentState(DoorStates.IdleState);
        StartPos = transform.position;
        EndPos = transform.position + Vector3.up * Distance;
        EndPos2 = transform.position + Vector3.down * Distance2;
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case DoorStates.IdleState:
                break;
            case DoorStates.DoorOpen:
                DoorOpen();
                break;
            case DoorStates.DoorClose:
                DoorClose();
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            setCurrentState(DoorStates.DoorOpen);
        }
        if(other.gameObject.layer == 9)
        {
            setCurrentState(DoorStates.DoorOpen);
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (transform.position == EndPos)
            {
                setCurrentState(DoorStates.IdleState);
            }
        }
        if (other.gameObject.layer == 9)
        {
            if (transform.position == EndPos)
            {
                setCurrentState(DoorStates.IdleState);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if(transform.position != EndPos)
            {
                currentSpeed = Speed;
            }
            setCurrentState(DoorStates.DoorClose);
        }
        if (other.gameObject.layer == 9)
        {
            setCurrentState(DoorStates.DoorClose);
        }

    }

    public void DoorOpen()
    {
        currentSpeed += Time.deltaTime * 2.5f;
        if (currentSpeed >= Speed)
        {
            currentSpeed = Speed;
        }

        transform.position = Vector3.Lerp(StartPos, EndPos, currentSpeed);

        if(transform.position == EndPos)
        {
            currentSpeed = 0;
            setCurrentState(DoorStates.IdleState);
        }
    }

    public void DoorClose()
    {
        currentSpeed += Time.deltaTime * 2.5f;
        if (currentSpeed >= Speed)
        {
            currentSpeed = Speed;
        }

        transform.position = Vector3.Lerp(EndPos, StartPos, currentSpeed);

        if (transform.position == StartPos)
        {
            currentSpeed = 0;
            setCurrentState(DoorStates.IdleState);
        }
    }
}

