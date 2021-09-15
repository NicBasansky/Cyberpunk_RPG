using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine.AI;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far the drops are scattered from the dropper's position")]
        [SerializeField] float dropDistanceScale = 1;
        [SerializeField] DropLibrary dropLibrary;

        const int ATTEMPTS = 30;

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();

            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * dropDistanceScale;
                // check if it is navigable on the navmesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}
