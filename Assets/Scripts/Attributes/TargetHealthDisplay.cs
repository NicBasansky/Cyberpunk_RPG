using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Combat;

namespace RPG.Attributes
{
    public class TargetHealthDisplay : MonoBehaviour
    {
        Text healthText;
        GameObject player;
        Fighter fighter;
        Health target = null;


        private void Awake()
        {
            healthText = GetComponent<Text>();
            player = GameObject.FindWithTag("Player");
            fighter = player.GetComponent<Fighter>();
        }

        void Update()
        {
            Health target = fighter.GetTarget();
            if (target != null)
            {

                healthText.text = String.Format("{0:0}/{1:0}", target.GetHealthPoints(), target.GetMaxHealthPoints());
            }
            else
            {
                healthText.text = "N/A";
            }
        }
    }

}