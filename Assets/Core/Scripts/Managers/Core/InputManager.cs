using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class InputManager : ManagerBase
{
    Camera _mainCamera;
    public Vector3 MouseWorldPosition { get { return _mainCamera.ScreenToWorldPoint(Input.mousePosition); } }

    public Action MouseButtonDown;
    public Action MouseButtonHold;
    public Action MouseButtonUp;


    public Action Num1KeyDown;
    public Action Num2KeyDown;
    public Action Num3KeyDown;
    public Action ReloadKeyDown;
    public Action JumpKeyDown;

    public Action RightArrowPressed;
    public Action LeftArrowPressed;

    [SerializeField]Vector3 _preMousePosition;
    Vector2 _mouseDelta;
    public Vector2 MouseDelta =>_mouseDelta;

    public bool AimTarget { set; get; }
    bool _saveMousePosition;
    public override void Init()
    {
        _mainCamera= Camera.main;
    }

    public override void ManagerUpdate()
    {
        if (AimTarget && !_saveMousePosition)
        {
            _preMousePosition = Input.mousePosition;
            UnityEngine.Cursor.visible = false;
            _saveMousePosition = true;
        }
        if (AimTarget)
        {
            _mouseDelta = _mainCamera.ScreenToWorldPoint(Input.mousePosition) - _mainCamera.ScreenToWorldPoint(_preMousePosition);
            Mouse.current.WarpCursorPosition(_preMousePosition);
        }
        if (!AimTarget && _saveMousePosition)
        {
            _saveMousePosition= false;
            Mouse.current.WarpCursorPosition(_preMousePosition);
            UnityEngine.Cursor.visible = true;

            _preMousePosition = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(0))
            MouseButtonDown?.Invoke();
        if (Input.GetMouseButton(0))
            MouseButtonHold?.Invoke();
        if (Input.GetMouseButtonUp(0))
            MouseButtonUp?.Invoke();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Num1KeyDown?.Invoke();
        if(Input.GetKeyDown(KeyCode.Alpha2))
            Num2KeyDown?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Num3KeyDown?.Invoke();

        if(Input.GetKey(KeyCode.D))
            RightArrowPressed?.Invoke();
        if (Input.GetKey(KeyCode.A))
            LeftArrowPressed?.Invoke();
        if (Input.GetKey(KeyCode.R))
            ReloadKeyDown?.Invoke();
        if(Input.GetKeyDown(KeyCode.Space))
            JumpKeyDown?.Invoke();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            AimTarget = false;
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            AimTarget = false;
        }
    }
}
