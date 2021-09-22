using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    public abstract class EffectStrategy : ScriptableObject
    {
        public abstract void ApplyEffects(AbilityData data, Action finished);
    }

}
