using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        LazyValue<float> mana;
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }
        void Start()
        {
            mana = new LazyValue<float>(GetMaxMana);
        }

        private void Update() 
        {
            if (mana.value < GetMaxMana())
            {
                mana.value += Time.deltaTime * GetRegenRate();
                if (mana.value > GetMaxMana())
                {
                    mana.value = GetMaxMana();
                }
            }
        }

        public float GetMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return baseStats.GetStat(Stat.Mana);
        }

        public float GetRegenRate()
        {
            return baseStats.GetStat(Stat.ManaRegenRate);
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana.value)
            {
                return false;
            }
            mana.value -= manaToUse;
            return true;
        }

        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }
    }
}