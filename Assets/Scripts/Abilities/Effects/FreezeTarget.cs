using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Freeze Target Effect", menuName = "RPG/Abilities/Effect/FreezeTarget", order = 0)]
    public class FreezeTarget : EffectStrategy
    {
        public override void ApplyEffects(AbilityData data, Action finished)
        {
            foreach(GameObject target in data.GetTargets())
            {
                if (target == data.GetUser()) continue;
                Animator anim = target.GetComponent<Animator>();
                if (anim == null) continue;
                anim.speed = 0;
                finished();

            }
            
        }
    }

}
