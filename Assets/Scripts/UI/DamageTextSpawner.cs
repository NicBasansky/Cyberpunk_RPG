using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        private void Start()
        {
            //Spawn(99f);
        }

        public void Spawn(float damage)
        {
            DamageText damageTextInstance = Instantiate<DamageText>(damageTextPrefab, transform);
            damageTextInstance.SetValue(damage);
        }

    }

}