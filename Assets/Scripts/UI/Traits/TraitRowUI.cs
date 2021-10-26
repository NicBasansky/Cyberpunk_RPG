using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPG.Stats;
using UnityEngine.Events;
using System;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] Trait trait;
        [SerializeField] TextMeshProUGUI traitValueText;
        [SerializeField] Button minusButton;
        [SerializeField] Button plusButton;

        TraitStore playerTraitStore;

        void Start()
        {
            playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            minusButton.onClick.AddListener(() => Allocate(-1));
            plusButton.onClick.AddListener(() => Allocate(1));
            // TODO need a lazy value to set the trait value and points remaining
        }

        public void Allocate(int points)
        {
            playerTraitStore.AssignPoints(trait, points);
            
        }

        private void Update() 
        {
           
            minusButton.interactable = playerTraitStore.CanAssignPoints(trait, - 1);
            plusButton.interactable = playerTraitStore.CanAssignPoints(trait, + 1);

            traitValueText.text = (playerTraitStore.GetProposedPoints(trait)).ToString();
            
        }

    }

}
