using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;

        public override void Use(GameObject user)
        {
            targetingStrategy.StartTargeting(user, TargetAcquired); 
        }

        
        private void TargetAcquired(IEnumerable<GameObject> targets)
        {
            foreach (var target in targets)
            {
                Debug.Log("Target " + target.name + " acquired");

            }
        }
    }
}
