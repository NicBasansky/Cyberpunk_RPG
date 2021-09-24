using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Stats;
using GameDevTV.Utils;
using GameDevTV.Saving;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            // empty class that inherits from the event
        }

        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDeath;

        [SerializeField] float regenerationPercentage = 70.0f;
        float startingHealth = 100f;
        LazyValue<float> healthPoints;
        bool isDead = false;
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            baseStats.onLevelUp -= RegenerateHealth;
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * regenerationPercentage / 100;
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public float GetHealthPercentage()
        {
            return healthPoints.value / baseStats.GetStat(Stat.Health) * 100;
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetHealthPointFraction()
        {
            return healthPoints.value / baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
            if (isDead) return;

            takeDamage.Invoke(damage);

            healthPoints.value -= damage;
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value <= 0)
            {
                AwardExperience(instigator);
                onDeath.Invoke();
                Die();

            }

        }

        public void Heal(float amount)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + amount, GetMaxHealthPoints());
        }

        private void AwardExperience(GameObject instigator)
        {
            if (instigator != null && instigator.GetComponent<Experience>() != null)
            {
                instigator.GetComponent<Experience>().GainExperience(baseStats.GetStat(Stat.ExperienceReward));
            }
        }

        private void Die()
        {
            isDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Animator>().SetTrigger("Die");

            // Prevents the player from falling through the floor on death for some reason
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            if (!gameObject.CompareTag("Player"))
            {
                GetComponent<CapsuleCollider>().isTrigger = true;
            }
        }

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}
