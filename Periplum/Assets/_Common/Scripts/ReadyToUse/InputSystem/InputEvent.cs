using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputEvent : MonoBehaviour
{
    [SerializeField] private InputAction action;
    [SerializeField] private UnityEvent _event;

    public InputAction Action => action;

    public event UnityAction Event
    {
        add => _event.AddListener(value);
        remove => _event.RemoveListener(value);
    }

    private void Awake()
    {
        action.performed += Action_Performed;
    }

    private void Action_Performed(InputAction.CallbackContext obj)
    {
        _event?.Invoke();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }
}