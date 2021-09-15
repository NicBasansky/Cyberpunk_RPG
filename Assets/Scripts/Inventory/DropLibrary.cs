using UnityEngine;
using GameDevTV.Inventories;
using System.Collections.Generic;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/DropLibrary")]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField]
        DropConfig[] potentialDrops;
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] float[] minDrops;
        [SerializeField] float[] maxDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            [Tooltip("Example: if 10, that means the chance of getting this drop is 10 x for every 1 y. This is AFTER the drop chance percentage is calculated")]
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumberOfDrops(int level)
            {
                if (!item.IsStackable())
                    return 1;

                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return (int)Random.Range(min, max + 1);
            }
        }
        

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }


        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break; // if we shouldn't be dropping at all then get out 
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        
        }

        private bool ShouldRandomDrop(int level)
        {
            return (Random.Range(0, 100) < GetByLevel(dropChancePercentage, level));
        }

        private int GetRandomNumberOfDrops(int level)
        {
            return (int)Random.Range(GetByLevel(minDrops, level), GetByLevel(maxDrops, level)); 
        }

        private Dropped GetRandomDrop(int level)
        {
            DropConfig dropConfig = SelectRandomItem(level);
            Dropped result = new Dropped();
            result.item = dropConfig.item;
            result.number = dropConfig.GetRandomNumberOfDrops(level);
            return result;
        }

        private DropConfig SelectRandomItem(int level)
        {
            // firstly, we know that we want to iterate over all the potential drops in order to select one
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance); // see note
            float chanceTotal = 0; 
            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if (randomRoll < chanceTotal) //  chanceTotal > randomRoll
                {
                    return drop;
                }
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var dropConfig in potentialDrops)
            {
                total += GetByLevel(dropConfig.relativeChance, level);
            }
            return total;
        }

        // static helper function where we feed in an array and a value for the level
        // and it return the associated value. static b/c it doesn't need to belong to any instance of the class
        static T GetByLevel<T>(T[] values, int level) // T so it can accept both floats and ints
        {
            if (values.Length == 0)
            {
                return default;
            }
            if (level > values.Length)
            {
                return values[values.Length - 1]; // return the last value of the values array
            }
            if (level <= 0)
            {
                return default;
            }
            return values[level - 1];
        }
    }
}

// min drops
// max drops
// potential drops
// - Relative Chance
// - Min items for stackable items
// - Max items

// note
/* The relative chance differs from totalChance. Each drop will have their own relative chance.
    Say a rel. chance for one drop is 10 and another is 1. 10 is ten times more likely than the one.
    The total chance is 10 plus 1 = 11. So essentially, totalChance represents an
    eleven-sided die.

    So on an 11-sided dice if you threw it and you wanted a hat to be dropped 10 of the 
    11 times and 1 of the 11 to be a sword, you'd have two options.
        You could generate a random number between 0 and 11 and anything <=1 would signify dropping
    a sword otherwise drop a hat. You could also say that if a randomRoll is <= 10
    then drop a hat and any number greater is the sword. 
        This is what we are trying to achieve with SelectRandomItem(). Another way of wording this is 
    say we have a hat with a relative chance of 10, sword 1, and a fireball 5. First we roll the die which has as many faces
    as the total relative chances of all drops (in this case, we're roll a 10 + 1 + 5 = 16 sided die). 
    chanceTotal is our running tally. We compare if the roll is less than the relative chance value. For example, we roll a 13 and the 
    relative chance of the hat is 10. Since our roll is greater than 10, we move on. Increase the tally to include the second drop so chanceTotal
    now 10 + 1 = 11. Is our roll of 13 less than this new tally? No, we move on. Increase the tally to include the fireball drop and 
    chanceTotal becomes 16. 13 is less than 16 so we return this particular drop. 
*/


