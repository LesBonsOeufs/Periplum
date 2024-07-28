using NaughtyAttributes;
using System;
using UnityEngine;

public delegate void PedometerEventHandler(Pedometer sender);
public class Pedometer : Singleton<Pedometer>
{
    public const float STEPS_PER_METERS = 1.31f;

    [SerializeField] private HealthConnectAARCaller healthConnect = default;
    [SerializeField] private bool runtimeUpdate = false;
    [SerializeField, ShowIf(nameof(runtimeUpdate))] private float updateFrequency = 10f;

    private float counter = 0f;

    public int StepsCountSinceLast { get; private set; }
    public int TodayStepsCount { get; private set; }

    public event PedometerEventHandler OnStepsUpdate;

    private void Start()
    {
        RefreshStepsCount();
    }

    private void Update()
    {
        if (!runtimeUpdate)
            return;

        if (counter >= updateFrequency)
        {
            RefreshStepsCount();
            counter = 0f;
        }

        counter += Time.deltaTime;
    }

    public void RefreshStepsCount() => healthConnect.GetTodayStepsCount(OnStepsCountReceived);

    /// <summary>
    /// Starts the StepsTracker, which updates the user on his progress towards target steps count, even if the app is closed, via a notification.
    /// </summary>
    public void StartStepsTracker(int targetSteps) => healthConnect.StartStepsTracker(targetSteps);
    public void StopStepsTracker() => healthConnect.StopStepsTracker();

    private void OnStepsCountReceived(int nSteps)
    {
        PedometerData lLocalData = LocalDataSaver<PedometerData>.CurrentData;

        if (lLocalData.lastUseDay < DateTime.Today)
        {
            lLocalData.lastUseDay = DateTime.Today;
            lLocalData.nLastTodaySteps = 0;
        }

        StepsCountSinceLast = nSteps - lLocalData.nLastTodaySteps;
        TodayStepsCount = nSteps;

        lLocalData.nLastTodaySteps = nSteps;
        LocalDataSaver<PedometerData>.SaveCurrentData();

        OnStepsUpdate?.Invoke(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnStepsUpdate = null;
    }
}