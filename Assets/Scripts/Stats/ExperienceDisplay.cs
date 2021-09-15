using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        [SerializeField] Text experienceText;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            experienceText.text = String.Format("{0:0}", experience.GetExperiencePoints());
        }
    }

}