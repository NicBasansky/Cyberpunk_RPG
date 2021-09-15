using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public interface IPredicateEvaluator
    {
        // a nullable bool returns true, false, or null. Yes, no, and I don't know
        bool? Evaluate(string predicate, string[] parameters);
    }
}