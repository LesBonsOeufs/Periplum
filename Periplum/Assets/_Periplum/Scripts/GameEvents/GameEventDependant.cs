using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Periplum
{
    public class GameEventDependant : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> onValueChange;
        [SerializeField] private UnityEvent onUnlockedAtAwake;
        [SerializeField] private UnityEvent onUnlocked;
        [SerializeField] private EGameEvent linkedEvent;

        [Foldout("Advanced"), SerializeField] private UnityEvent<bool> onValueChange_Inverted;

        private bool isUnlocked;

        private void Awake()
        {
            LocalGameEventsData.OnEventSet += LocalGameEventsData_OnEventSet;
            isUnlocked = LocalDataSaver<LocalGameEventsData>.CurrentData.Get(linkedEvent);

            if (isUnlocked)
                onUnlockedAtAwake?.Invoke();

            onValueChange?.Invoke(isUnlocked);
            onValueChange_Inverted?.Invoke(!isUnlocked);
        }

        private void LocalGameEventsData_OnEventSet(EGameEvent gameEvent, bool value)
        {
            if (gameEvent != linkedEvent)
                return;

            bool lPreviousValue = isUnlocked;
            isUnlocked = value;

            onValueChange?.Invoke(isUnlocked);
            onValueChange_Inverted?.Invoke(!isUnlocked);

            if (isUnlocked && !lPreviousValue)
                onUnlocked?.Invoke();
        }
    }
}