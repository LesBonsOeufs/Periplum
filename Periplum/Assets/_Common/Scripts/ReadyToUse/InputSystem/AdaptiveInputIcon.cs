using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AdaptiveInputIcon : MonoBehaviour
{
    [SerializeField] private GameObject gamepadIcon;
    [SerializeField] private GameObject keyboardIcon;

    private IDisposable anyButtonListener;

    private void Awake()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        gamepadIcon.SetActive(true);
        keyboardIcon.SetActive(false);
#else
        bool lContainsGamepad = false;

        foreach (InputDevice lDevice in InputSystem.devices) 
        { 
            if (lDevice is Gamepad)
            {
                lContainsGamepad = true;
                break;
            }
        }

        gamepadIcon.SetActive(lContainsGamepad);
        keyboardIcon.SetActive(!lContainsGamepad);
#endif
    }

#if !UNITY_SWITCH || UNITY_EDITOR
    private void OnEnable()
    {
        anyButtonListener = InputSystem.onAnyButtonPress.Call(InputSystem_OnAnyButtonPress);
    }

    private void OnDisable()
    {
        anyButtonListener.Dispose();
    }

    private void InputSystem_OnAnyButtonPress(InputControl inputControl)
    {
        if (inputControl.device is Mouse)
            return;

        bool lIsGamepad = inputControl.device is not Keyboard;
        gamepadIcon.SetActive(lIsGamepad);
        keyboardIcon.SetActive(!lIsGamepad);
    }
#endif
}