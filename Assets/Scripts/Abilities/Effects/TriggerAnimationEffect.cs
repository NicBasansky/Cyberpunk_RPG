using System;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "RPG/Abilities/Effect/TriggerAnimEffect", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string animationTriggerString;

        public override void ApplyEffects(AbilityData data, Action finished)
        {          
            Animator anim = data.GetUser().GetComponent<Animator>();
            if (anim)
            {
                anim.SetTrigger(animationTriggerString);
            }
            finished();
        }
    }
}
