using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

[RequireComponent(typeof(InputEvent))]
public class InputEvent_HoldDrawer : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        fillImage.fillAmount = 0f;
        InputAction lInputAction = GetComponent<InputEvent>().Action;
        lInputAction.started += InputAction_started;
        lInputAction.canceled += InputAction_canceled;
    }

    private void InputAction_started(InputAction.CallbackContext obj)
    {
        HoldInteraction lHoldInteraction = obj.interaction as HoldInteraction;

        if (lHoldInteraction == null)
            Debug.LogError("HoldDrawer's attached InputEvent does not have a HoldInteraction!");
        else
            fillImage.DOFillAmount(1f, lHoldInteraction.duration).SetEase(Ease.Linear);
    }

    private void InputAction_canceled(InputAction.CallbackContext obj)
    {
        fillImage.DOKill();
        fillImage.fillAmount = 0f;
    }
}