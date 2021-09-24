using System;
using System.Collections;
using UnityEngine;
using RPG.Control;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Orient To Target Effect", menuName = "RPG/Abilities/Effect/OrientToTarget", order = 0)]
    public class OrientToTargetEffect : EffectStrategy
    {
        [SerializeField] float rotationSpeed = 0.5f;

        public override void ApplyEffects(AbilityData data, Action finished)
        {
            //data.GetUser().transform.LookAt(data.GetTargetedPoint());
          
            data.StartCoroutine(RotateToFace(data, data.GetTargetedPoint(), finished));
            //finished();
        }

        IEnumerator RotateToFace(AbilityData data, Vector3 target, Action finished)
        {
            Transform userTransform = data.GetUser().transform;
            Vector3 targetDirection = target - userTransform.position;
            float timeElapsed = 0;
            while (timeElapsed <= 1.5f)
            {
                userTransform.rotation = Quaternion.Slerp(userTransform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up),
                                                   rotationSpeed * Time.deltaTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            userTransform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            finished();
        }
    }
}
