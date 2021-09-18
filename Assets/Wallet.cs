using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Inventories
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] int money = 100;

        public event Action onMoneyUpdated;

        void Start()
        {
            if (onMoneyUpdated != null)
                onMoneyUpdated();
        }

        public void UpdateMoneyAmount(int amount)
        {
            money += amount;

            if (onMoneyUpdated != null)
            {
                onMoneyUpdated();
            }
        }

        public int GetMoneyAmount()
        {
            return money;
        }
    }
}
