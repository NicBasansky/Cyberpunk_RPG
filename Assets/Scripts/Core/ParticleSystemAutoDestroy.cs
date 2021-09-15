using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ParticleSystemAutoDestroy : MonoBehaviour
    {
        ParticleSystem ps;
        [SerializeField] GameObject targetToDestroy = null;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }


        void Update()
        {
            if (ps != null && !ps.IsAlive())
            {
                if (targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

    }
}
