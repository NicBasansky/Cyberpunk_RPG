using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "RPG/Abilities/Effect/DelayComposite", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] float delay = 0f;
        [SerializeField] EffectStrategy[] effectStrategies;

        public override void ApplyEffects(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffect(data, finished));
             
        }

        IEnumerator DelayedEffect(AbilityData data, Action finished)
        {
            if (data.IsCancelled()) yield break;
            
            yield return new WaitForSeconds(delay);

            foreach(EffectStrategy effect in effectStrategies)
            {
                effect.ApplyEffects(data, finished);
            }
        }
    }
}
