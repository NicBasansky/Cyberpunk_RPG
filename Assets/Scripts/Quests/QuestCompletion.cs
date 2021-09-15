using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] string objective;
        [SerializeField] bool completeAllObjectives = false;

        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            if (questList.HasQuest(quest))
            {
                if (completeAllObjectives)
                {
                    questList.CompleteAllObjectives(quest);
                    return;
                }
                questList.CompleteObjective(quest, objective);
            }
        }
    }

}
