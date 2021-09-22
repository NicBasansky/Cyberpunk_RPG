using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    public abstract class FilteringStrategy : ScriptableObject
    {
        public abstract IEnumerable<GameObject> FilterTargets(IEnumerable<GameObject> targets);
    }

}
