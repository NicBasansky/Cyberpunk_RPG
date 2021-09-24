using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "RPG/Abilities/Effect/SpawnTargetPrefab", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject prefabToSpawn;
        [SerializeField] float destroyDelay = -1.0f;

        public override void ApplyEffects(AbilityData data, Action finished)
        {   
            data.StartCoroutine(Effect(data, finished));
        }

        IEnumerator Effect(AbilityData data, Action finished)
        {

            Vector3 targetLocation = data.GetTargetedPoint();
            GameObject fx = Instantiate(prefabToSpawn, targetLocation, Quaternion.identity);
           
            if (destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(fx);
            }

            finished();
        }

    }
}
