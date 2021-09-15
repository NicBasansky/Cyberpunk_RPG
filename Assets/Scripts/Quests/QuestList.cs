using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Core;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action onUpdate;

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            
            QuestStatus status = new QuestStatus(quest);
            statuses.Add(status);
            if (onUpdate != null)
            {
                onUpdate();
            }
            
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective); 
            
            if (status.IsComplete())
            {
                GiveRewards(quest);
            }

            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        public void CompleteAllObjectives(Quest quest)
        {
            foreach(var objective in quest.GetObjectives())
            {
                if (GetQuestStatus(quest).IsObjectiveComplete(objective.reference))
                {
                    continue;
                }
                GetQuestStatus(quest).CompleteObjective(objective.reference);
            }

            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        private void GiveRewards(Quest quest)
        {
            // foreach (Quest.Reward ...)
            foreach(var reward in quest.GetRewards())
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }
            return null;
        }

        // temp
        public void RemoveQuest(Quest quest)
        {
            foreach(QuestStatus questStatus in statuses)
            {
                if (questStatus.GetQuest().name == quest.name)
                {
                    statuses.Remove(questStatus);
                    break;
                }
            }
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;

            statuses.Clear();
            foreach(object objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                return HasQuest(Quest.GetQuestByName(parameters[0]));
                case "QuestCompleted":
                return HasQuest(Quest.GetQuestByName(parameters[0])) &&
                        GetQuestStatus(Quest.GetQuestByName(parameters[0])).IsComplete();
                case "ObjectiveCompleted":
                    return HasQuest(Quest.GetQuestByName(parameters[0])) &&
                            GetQuestStatus(Quest.GetQuestByName(parameters[0])).IsObjectiveComplete(parameters[1]);
            }

            return null;

        }
    }

}
