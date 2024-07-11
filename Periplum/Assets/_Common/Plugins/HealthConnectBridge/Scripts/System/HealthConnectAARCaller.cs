using System;
using UnityEngine;

public class HealthConnectAARCaller : AndroidAARCaller
{
#if UNITY_EDITOR
    [SerializeField] private int editorTestSteps = 132;
#endif

    private Action<int> todayStepsReceivedCallback;

    /// <summary>
    /// Do not change name or signature: used by .AAR
    /// </summary>
    /// <param name="stepsCount"></param>
    private void ReceiveTodayStepsCount(string stepsCount)
    {
        todayStepsReceivedCallback?.Invoke(int.Parse(stepsCount));
        todayStepsReceivedCallback = null;
    }

    public void GetTodayStepsCount(Action<int> callback)
    {
        todayStepsReceivedCallback = callback;

#if UNITY_ANDROID && !UNITY_EDITOR
        Call("getTodayStepsCount");
#else
        todayStepsReceivedCallback?.Invoke(editorTestSteps);
#endif
    }
}