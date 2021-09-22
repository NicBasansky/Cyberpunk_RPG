using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "New Tag Filter", menuName = "RPG/Abilities/Filters/TagFilter", order = 0)]
    public class TagFilter : FilteringStrategy
    {
        [SerializeField] string filterTagString;
        public override IEnumerable<GameObject> FilterTargets(IEnumerable<GameObject> targets)
        {
            foreach(GameObject target in targets)
            {
                if (target.tag == filterTagString)
                {
                    yield return target;
                }
            }
        }
    }

}
