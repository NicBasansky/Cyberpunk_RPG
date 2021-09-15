using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class NPCBasicAIController : MonoBehaviour
    {
        [SerializeField] AnimState animState;
        [SerializeField] float minSecondsToDrink = 2f;
        [SerializeField] float maxSecondsToDrink = 10f;

        enum AnimState
        {
            Standing,
            Sitting,
            Dancing,
            StandDrinking
        };

        Animator animator;

        private void Awake() 
        {
            animator = GetComponent<Animator>();
        }

        private void Start() 
        {
            switch (animState)
            {
                case AnimState.Sitting:
                    animator.SetBool("isSitting", true);
                    break;
                case AnimState.Dancing:
                    animator.SetBool("isDancing", true);
                    break;
                case AnimState.StandDrinking:
                    StartCoroutine(RandomlyDrink());
                    break;
            }
        }

        IEnumerator RandomlyDrink()
        {
            while (animState == AnimState.StandDrinking)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(minSecondsToDrink, maxSecondsToDrink));
                animator.SetTrigger("takeDrink");
            }
            
        }

    }

    
}