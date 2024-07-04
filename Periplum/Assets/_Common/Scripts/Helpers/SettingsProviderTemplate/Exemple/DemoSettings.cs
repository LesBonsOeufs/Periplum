using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[GameSettings(typeof(DemoSettings))]
public class DemoSettings : SingletonSCO<DemoSettings>
{
    public float test;

    [NaughtyAttributes.Button]
    public void TEST() { }
}
