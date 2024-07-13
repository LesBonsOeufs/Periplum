using NaughtyAttributes;
using System;
using UnityEngine;

public delegate void PodometerEventHandler(Pedometer sender);
public class Pedometer : Singleton<Pedometer>
{
    [SerializeField] private HealthConnectAARCaller healthConnect = default;
    [SerializeField] private bool runtimeUpdate = false;
    [SerializeField, ShowIf(nameof(runtimeUpdate))] private float updateFrequency = 10f;

    private float counter = 0f;

    public int StepsCountSinceLast { get; private set; }
    public int TodayStepsCount { get; private set; }

    public event PodometerEventHandler OnStepsUpdate;

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