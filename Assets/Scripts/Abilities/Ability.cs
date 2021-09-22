using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using RPG.Abilities.Filters;
using RPG.Abilities.Effects;


namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilteringStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;

        public override void Use(GameObject user)
        {
            AbilityData data = new AbilityData(user);
            targetingStrategy.StartTargeting(data, 
                    () => TargetAcquired(data));

        }
        
        private void TargetAcquired(AbilityData data)
        {
            foreach(var filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.FilterTargets(data.GetTargets()));
            }

            foreach(var effect in effectStrategies)
            {
                effect.ApplyEffects(data, OnEffectFinished);
            }      
        }

        private void OnEffectFinished()
        {

        }
    }
}
