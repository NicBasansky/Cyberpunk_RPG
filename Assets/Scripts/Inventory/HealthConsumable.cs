using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using RPG.Attributes;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "HealthConsumable", menuName = "GameDevTV/GameDevTV.UI.InventorySystem/Action Item/HealthConsumable", order = 0)]
    public class HealthConsumable : ActionItem 
    {
        [SerializeField] int addToHealthAmount = 0;

        public override void Use(GameObject user)
        {
            //base.Use(user);
            user.GetComponent<Health>().Heal(addToHealthAmount);
        }

    }
}