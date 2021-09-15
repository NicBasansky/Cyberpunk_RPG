using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;
using TMPro;
using System;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardText;
        // [SerializeField] string rewardText;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.name;
            foreach(Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach(Quest.Objective objective in quest.GetObjectives())
            {
                GameObject objPrefab = objectiveIncompletePrefab;
                if (status.IsObjectiveComplete(objective.reference))
                {
                    objPrefab = objectivePrefab;
                }
                GameObject prefab = Instantiate(objPrefab, objectiveContainer);
                TextMeshProUGUI objectiveText = prefab.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }
            rewardText.text = GenerateRewardText(quest);
            
        }

        private string GenerateRewardText(Quest quest)
        {
            string displayedRewards = "";
            foreach (var reward in quest.GetRewards())
            {
                if (displayedRewards != "")
                {
                    displayedRewards += ", ";
                }

                if (reward.number > 1)
                {
                    displayedRewards += "(" + reward.number + ") ";
                }
                displayedRewards += reward.item.GetDisplayName();
            }
            if (displayedRewards == "")
            {
                displayedRewards = "No rewards mentioned";
            }
            return displayedRewards;
        }

    }

}
