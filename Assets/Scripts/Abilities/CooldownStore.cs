using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialTimeSeconds = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            var keys = new List<InventoryItem>(cooldownTimers.Keys);
            foreach(InventoryItem ability in keys)
            {
                cooldownTimers[ability] -= Time.deltaTime;
                
                if (cooldownTimers[ability] <= 0)
                {
                    cooldownTimers.Remove(ability);
                    initialTimeSeconds.Remove(ability);
                }
            }
        }

        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {    
            cooldownTimers[ability] = cooldownTime;
            initialTimeSeconds[ability] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem ability)
        {
            if (!cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }
            return cooldownTimers[ability];
        }

        public float GetFractionRemaining(InventoryItem ability)
        {
            if (ability == null) return 0;

            if (!cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }

            return GetTimeRemaining(ability) / initialTimeSeconds[ability];

        }
    }
}
