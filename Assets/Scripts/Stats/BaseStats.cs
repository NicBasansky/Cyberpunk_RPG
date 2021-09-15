using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 25)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticles = null;
        [SerializeField] bool shouldUseModifiers = false;

        Experience experience = null;
        LazyValue<int> currentLevel;

        public event Action onLevelUp;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                SpawnLevelUpParticles();
                onLevelUp();
            }
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXp = GetComponent<Experience>().GetExperiencePoints();
            int numLevels = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= numLevels; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (currentXp < xpToLevelUp)
                {
                    return level;
                }
            }
            return numLevels + 1; // if current xp exceeds the last xp requirement then it is on level higher
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, CalculateLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float result = 0;
            foreach (IModifier provider in GetComponents<IModifier>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    result += modifier;
                }
            }
            return result;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifier provider in GetComponents<IModifier>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private void SpawnLevelUpParticles()
        {
            if (levelUpParticles == null) return;

            GameObject fx = Instantiate(levelUpParticles, this.transform);
            Destroy(fx, 3f);
        }
    }
}
