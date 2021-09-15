using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG/Quest", order = 0)]
    public class Quest : ScriptableObject 
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>(); 

        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

        [System.Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public InventoryItem item;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveRef)
        {
            foreach (Objective obj in objectives)
            {
                if (obj.reference == objectiveRef)
                {
                    return true;
                }
            }
            return false;
            //return objectives.Contains(objective);
        }

        public static Quest GetQuestByName(string questName)
        {
            foreach(Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}