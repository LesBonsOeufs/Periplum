using System;
using UnityEngine;

namespace Periplum
{
    public class TimedStepsTrackTest : MonoBehaviour
    {
        public void Execute()
        {
            Pedometer.Instance.StartStepsTracker(5, DateTime.Now.AddMinutes(15));
        }
    }
}