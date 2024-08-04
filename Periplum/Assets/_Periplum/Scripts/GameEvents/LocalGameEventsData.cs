using System;
using System.Collections.Generic;

[Serializable]
public class LocalGameEventsData
{
    private readonly Dictionary<EGameEvent, bool> data = new();

    public static event Action<EGameEvent, bool> OnEventSet;

    public bool Get(EGameEvent gameEvent)
    {
        return data.TryGetValue(gameEvent, out bool lValue) && lValue;
    }

    public void Set(EGameEvent gameEvent, bool value)
    {
        data.TryAdd(gameEvent, value);
        OnEventSet?.Invoke(gameEvent, value);
    }
}