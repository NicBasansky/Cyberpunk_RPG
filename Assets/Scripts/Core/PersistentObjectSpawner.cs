using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject PersistentObjectsPrefab;
        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned) return;
            SpawnPersistentObjects();
            hasSpawned = true;
        }

        public void SpawnPersistentObjects()
        {
            GameObject persistentObjects = Instantiate(PersistentObjectsPrefab);
            DontDestroyOnLoad(persistentObjects);
        }

    }

}