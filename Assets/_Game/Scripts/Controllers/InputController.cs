using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    public static Action OnDragDown, OnDragUp;

    private const float DistanceTreshold = 10;

    private Vector3 _startPosition;
    private bool _inputEnabled = true;
    private void Awake()
    {
        Instance = this;
    }



    void Update()
    {
        if (_inputEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                onTouchDown();
            }
            if (Input.GetMouseButton(0))
            {
                onTouchStay();
            }
            if (Input.GetMouseButtonUp(0))
            {
                onTouchUp();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnDragDown?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnDragUp?.Invoke();
        }
    }

    void onTouchDown()
    {
        _startPosition = Input.mousePosition;
    }

    void onTouchStay()
    {
        Vector3 move = Input.mousePosition - _startPosition;
        move.z = 0;
        if (Mathf.Abs(move.y) > DistanceTreshold)
        {
            if (move.y > 0)
            {
                OnDragUp?.Invoke();
            }
            else
            {
                OnDragDown?.Invoke();
            }
        }
    }

    void onTouchUp()
    {
        // Vector3 move = Input.mousePosition - _startPosition;
        // move.z = 0;
        // if (move.y > DistanceTreshold)
        // {
        //     if (move.y > 0)
        //     {
        //         OnDragUp?.Invoke();
        //     }
        //     else
        //     {
        //         OnDragDown?.Invoke();
        //     }
        // }
    }

    public void disableInput()
    {

        _inputEnabled = false;
    }

    public void enableInput()
    {
        _inputEnabled = true;
        _startPosition = Input.mousePosition;
    }
}
