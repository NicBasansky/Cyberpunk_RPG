using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using RPG.Abilities.Filters;
using RPG.Abilities.Effects;
using RPG.Attributes;
using RPG.Core;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilteringStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float coolDownSeconds = 0f;
        [SerializeField] float manaCost = 0f;
        bool cooldown = false;


        // TODO have some sort of player feedback if mana is too low for ability use
        public override void Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if (mana.GetMana() < manaCost) return;

            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            
            if (cooldownStore.GetTimeRemaining(this) > 0) return;
  
            AbilityData data = new AbilityData(user);

            ActionScheduler scheduler = data.GetUser().GetComponent<ActionScheduler>();          
            scheduler.StartAction(data);
            

            targetingStrategy.StartTargeting(data,
                    () => TargetAcquired(data, mana));
            
        }
        
        private void TargetAcquired(AbilityData data, Mana mana)
        {
            if (data.IsCancelled()) return;
            
            if(!mana.UseMana(manaCost)) return;

            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, coolDownSeconds);

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
