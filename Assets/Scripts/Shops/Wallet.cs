using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameDevTV.Saving;

namespace RPG.Inventories
{
    public class Wallet : MonoBehaviour, ISaveable
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

        public object CaptureState()
        {
            return GetMoneyAmount();
        }

        public void RestoreState(object state)
        {
            money = (int)state;
            
        }
    }
}
