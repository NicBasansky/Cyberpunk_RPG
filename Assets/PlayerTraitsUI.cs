using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Stats;
using UnityEngine.UI;

namespace RPG.UI
{
    public class PlayerTraitsUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI unassignedPointsText;
        [SerializeField] Button commitButton;

        TraitStore traitStore;

        void Start()
        {      
            traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            commitButton.onClick.AddListener(traitStore.Commit);
        }

        private void Update()
        {
            unassignedPointsText.text = string.Format("{0:N0}", traitStore.GetUnassignedPoints());
            
        }
    }

}