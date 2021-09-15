using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] Disjunction[] and;
        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction disjunction in and)
            {
                if (!disjunction.Check(evaluators))
                {
                    return false;
                }
            }
            return true;
        }

        [System.Serializable]
        class Disjunction
        {
            [SerializeField] Predicate[] or;
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach(Predicate pred in or)
                {
                    if (pred.Check(evaluators))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        [System.Serializable]
        class Predicate
        {
            [SerializeField] string predicate;
            [SerializeField] string[] parameters;
            [SerializeField] bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result == null)
                    {
                        continue;
                    }
                    if (result == negate)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool HasCondition() // TODO check to see if this will work
        {
            //return predicate != "";
            return and.Length > 0;
        }
    }

}