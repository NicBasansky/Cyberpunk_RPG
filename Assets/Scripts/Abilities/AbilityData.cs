using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        IEnumerable<GameObject> targets;
        GameObject user;
        Vector3 targetedPoint;
        bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public GameObject GetUser()
        {
            return user;
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return targets;
        }

        public Vector3 GetTargetedPoint()
        {
            return targetedPoint;
        }

        public void SetTargetedPoint(Vector3 point)
        {
            targetedPoint = point;
        }

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public bool IsCancelled ()
        {
            return cancelled;
        }

        public void Cancel()
        {
            cancelled = true;
        }
    }
}
