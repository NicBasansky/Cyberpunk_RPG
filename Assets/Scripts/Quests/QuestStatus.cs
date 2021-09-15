using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        [SerializeField] Quest quest;
        [SerializeField] List<string> completedObjectives = new List<string>();

        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object obj)
        {
            QuestStatusRecord record = (QuestStatusRecord)obj;
            this.quest = Quest.GetQuestByName(record.questName);
            completedObjectives = record.completedObjectives;
        }


        public Quest GetQuest()
        {
            return quest;
        }

        public int GetNumberOfCompletedObjectives()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }
        }

        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }


        public object CaptureState()
        {
            QuestStatusRecord record = new QuestStatusRecord();
            record.questName = quest.name;
            record.completedObjectives = completedObjectives;
            return record;
        }


        // public IEnumerable<string> GetCompletedObjectives()
        // {
        //     return completedObjectives;
        // }

    }
}
