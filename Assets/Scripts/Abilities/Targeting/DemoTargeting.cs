using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DemoTargeting", menuName = "RPG/Abilities/Targeting/Demo", order = 0)]
public class DemoTargeting : TargetingStrategy
{
    public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished)
    {
        Debug.Log("Demo targeting strategy has begun");
        finished(null);

    }
}