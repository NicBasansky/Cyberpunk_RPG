using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetingStrategy : ScriptableObject
{
    public abstract void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished);

}