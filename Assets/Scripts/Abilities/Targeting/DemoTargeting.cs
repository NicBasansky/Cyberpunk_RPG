using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "DemoTargeting", menuName = "RPG/Abilities/Targeting/Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            Debug.Log("Demo targeting strategy has begun");
            finished();

        }
    }

}
