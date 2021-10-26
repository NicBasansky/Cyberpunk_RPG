using System;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "RPG/Abilities/Effect/TriggerAnimEffect", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string animationTriggerString;
        [Tooltip("If checked animation will play regardless if there are any targets or not")]
        [SerializeField] bool alwaysTrigger = true;

        public override void ApplyEffects(AbilityData data, Action finished)
        {         
            // if (!alwaysTrigger)
            // {
            //     int counter = 0;
            //     foreach(var target in data.GetTargets())
            //     {
            //         counter++;
            //     }
            //     if (counter < 1)
            //     {
            //         finished();
            //         return;
            //     }
            // }
            Animator anim = data.GetUser().GetComponent<Animator>();
            if (anim)
            {
                anim.SetTrigger(animationTriggerString);
            }
            finished();
        }
    }
}
