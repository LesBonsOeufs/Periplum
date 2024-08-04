using UnityEngine;

public class GameEventSetter : MonoBehaviour
{
    [SerializeField] private EGameEvent gameEvent;

    public void IntCastedSet(int value) => Set(value != 0);
    public void Set(bool value)
    {
        LocalDataSaver<LocalGameEventsData>.CurrentData.Set(gameEvent, value);
        LocalDataSaver<LocalGameEventsData>.SaveCurrentData();
    }
}