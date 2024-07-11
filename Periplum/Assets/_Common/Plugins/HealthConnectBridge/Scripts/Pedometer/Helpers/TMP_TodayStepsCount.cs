using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_TodayStepsCount : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        Pedometer.Instance.OnStepsUpdate += Pedometer_OnStepsUpdate;
    }

    private void Pedometer_OnStepsUpdate(Pedometer sender)
    {
        tmp.text = $"{sender.TodayStepsCount} steps today";
    }
}