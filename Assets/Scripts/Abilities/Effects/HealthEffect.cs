using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New Health Effect", menuName = "RPG/Abilities/Effect/HealthEffect", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange = 0;
        [SerializeField] bool isHealing = false;

        public override void ApplyEffects(AbilityData data, Action finished)
        {
            foreach(var target in data.GetTargets())
            {
                Health targetHealth = target.GetComponent<Health>();
                if (targetHealth == null) continue;

                if (!isHealing)
                {
                    targetHealth.TakeDamage(healthChange, data.GetUser());
                }
                else
                {
                    targetHealth.Heal(healthChange);
                }
                finished();
            }
        }
    }

}
