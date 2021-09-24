using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        Mana mana;
        [SerializeField] Text manaValue;

        private void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            manaValue.text = String.Format("{0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }

    }

}