using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Abilities
{
    public class AbilityData
    {
        IEnumerable<GameObject> targets;
        GameObject user;

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

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }
    }
}
