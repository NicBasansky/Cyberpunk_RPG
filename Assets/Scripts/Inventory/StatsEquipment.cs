using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifier
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            // return a list of equipment locations
            foreach (var slot in GetAllPopulatedSlots())
            {
                // not all things that are equipable modify stats
                // so we cast to see if it does and it doesn't, then we continue
                var item = GetItemInSlot(slot) as IModifier;
                if (item == null) continue;

                foreach (float modifier in item.GetAdditiveModifiers(stat))
                { 
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            // return a list of equipment locations
            foreach (var slot in GetAllPopulatedSlots())
            {
                // not all things that are equipable modify stats
                // so we cast to see if it does and it doesn't, then we continue
                var item = GetItemInSlot(slot) as IModifier;
                if (item == null) continue;

                foreach (float modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }

}