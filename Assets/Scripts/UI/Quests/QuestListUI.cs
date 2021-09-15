using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        
        [SerializeField] QuestItemUI questPrefab;
        QuestList questList;

        private void Start()
        {
            questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        { 
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (QuestStatus questStatus in questList.GetStatuses())
            {
                QuestItemUI newPrefab = Instantiate<QuestItemUI>(questPrefab, transform);
                newPrefab.Setup(questStatus);
            }
        }
    }

}