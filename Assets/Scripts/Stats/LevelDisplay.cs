using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] Text levelText;
        BaseStats playerBaseStats;

        private void Awake()
        {
            playerBaseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();

        }

        void Update()
        {
            levelText.text = String.Format("{0:0}", playerBaseStats.CalculateLevel());
        }
    }

}