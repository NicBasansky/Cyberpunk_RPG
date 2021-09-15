using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest quest;
    

        public void GiveQuest()
        {
            QuestList playerQuestList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            playerQuestList.AddQuest(quest);
        }
    }

}