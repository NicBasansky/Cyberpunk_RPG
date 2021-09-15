using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV;
using GameDevTV.Saving;
using System;

namespace RPG.Cinematics
{
    public class CameraPrioritySaver : MonoBehaviour, ISaveable
    {
        Cinemachine.CinemachineVirtualCamera cam;
        [SerializeField] CameraSwitcher InTrigger;
        [SerializeField] CameraSwitcher SecondInTrigger = null;
        [SerializeField] CameraSwitcher OutTrigger;
        [SerializeField] CameraSwitcher SecondOutTrigger = null;

        int currentPriority = 0;

        private void Awake()
        {
            cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            InTrigger.onPriorityUpdated += CameraPriorityUpdated;
            OutTrigger.onPriorityUpdated += CameraPriorityUpdated;

            if (SecondInTrigger != null && SecondOutTrigger != null)
            {
                SecondInTrigger.onPriorityUpdated += CameraPriorityUpdated;
                SecondOutTrigger.onPriorityUpdated += CameraPriorityUpdated;
            }

        }

        private void OnDisable()
        {
            InTrigger.onPriorityUpdated -= CameraPriorityUpdated;
            OutTrigger.onPriorityUpdated -= CameraPriorityUpdated;

            if (SecondInTrigger != null && SecondOutTrigger != null)
            {
                SecondInTrigger.onPriorityUpdated += CameraPriorityUpdated;
                SecondOutTrigger.onPriorityUpdated -= CameraPriorityUpdated;
            }
        }

        private void CameraPriorityUpdated()
        {
            currentPriority = cam.Priority;
        }

        public object CaptureState()
        {
            return currentPriority;
        }

        public void RestoreState(object state)
        {
            currentPriority = (int)state;
            cam.Priority = currentPriority;
        }
    }
}
