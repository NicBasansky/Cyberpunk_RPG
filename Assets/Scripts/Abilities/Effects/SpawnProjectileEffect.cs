using System;
using System.Collections;
using UnityEngine;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "RPG/Abilities/Effect/SpawnProjectile", order = 0)]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] Projectile projectileToSpawn;
        [SerializeField] float damage = 10.0f;
        [SerializeField] bool useTargetPoint = true;

        public override void ApplyEffects(AbilityData data, Action finished)
        {
            Transform spawnTransform = data.GetUser().GetComponent<Fighter>().GetHandTransform(true);
            if (useTargetPoint)
            {
                SpawnProjectileForTargetPoint(data, spawnTransform);
            }
            else
            {
                SpawnProjectileForTargets(data, spawnTransform);

            }
            finished();
        }

        private void SpawnProjectileForTargetPoint(AbilityData data, Transform spawnTransform)
        {
            Projectile projectile = Instantiate(projectileToSpawn);
            projectile.transform.position = spawnTransform.position;
            projectile.SetTarget(data.GetUser(), damage, data.GetTargetedPoint());
        }

        private void SpawnProjectileForTargets(AbilityData data, Transform spawnTransform)
        {
            foreach (GameObject target in data.GetTargets())
            {
                Health targetHealth = target.GetComponent<Health>();
                if (targetHealth)
                {
                    Projectile projectile = Instantiate(projectileToSpawn);
                    projectile.transform.position = spawnTransform.position;
                    projectile.SetTarget(targetHealth, data.GetUser(), damage);
                }
            }
        }
    }
}
