using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Freeze Player Control Effect", menuName = "RPG/Abilities/Effect/FreezePlayer", order = 0)]
    public class FreezePlayerControl : EffectStrategy
    {
        [SerializeField] float playerControlFreezeDuration = 2.0f;
        PlayerController controller;

        public override void ApplyEffects(AbilityData data, Action finished)
        {
            //userTransform.LookAt(data.GetTargetedPoint());

            controller = data.GetUser().GetComponent<PlayerController>();
            if (controller)
            {
                controller.enabled = false;
            }
            controller.StartCoroutine(RestorePlayerControlCoroutine(data, finished));

        }

        IEnumerator RestorePlayerControlCoroutine(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(playerControlFreezeDuration);
            controller.enabled = true;
            finished();
        }

    }
}
