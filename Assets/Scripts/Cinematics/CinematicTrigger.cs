using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GameDevTV.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!hasTriggered && other.gameObject.tag == "Player")
            {
                hasTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }

        public object CaptureState()
        {
            return hasTriggered;
        }

        public void RestoreState(object state)
        {
            hasTriggered = (bool)state;
        }


    }

}