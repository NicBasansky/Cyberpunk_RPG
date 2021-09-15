using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifier
    {
        [SerializeField] Modifier[] additiveModifiers;
        [SerializeField] Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }
        
        
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (Modifier mod in additiveModifiers)
            {
                if (mod.stat == stat)
                {
                    yield return mod.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach(Modifier mod in percentageModifiers)
            {
                if (mod.stat == stat)
                {
                    yield return mod.value;
                }
            }
        }

    }
}
