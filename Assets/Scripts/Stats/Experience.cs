using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        public event Action onExperienceGained;

        float experiencePoints = 0;

        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                GainExperience(Time.deltaTime * 1000);
            }
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }

}