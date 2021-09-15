using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] GameObject target;

        void LateUpdate()
        {
            transform.position = target.transform.position;
        }
    }

}