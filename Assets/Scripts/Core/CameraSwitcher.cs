using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Cinematics
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] Cinemachine.CinemachineVirtualCamera cameraToSwitchTo;
        [SerializeField] int cameraPriority = 10;

        public event Action onPriorityUpdated;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                cameraToSwitchTo.Priority = cameraPriority;
                onPriorityUpdated();
            }
        }

    }

}
