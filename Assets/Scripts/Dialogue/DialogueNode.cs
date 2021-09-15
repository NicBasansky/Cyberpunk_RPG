using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using RPG.Core;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [TextArea(3, 6)] // min and max lines are the arguments
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 115);
        [SerializeField] bool isPlayerSpeaking = false;
        [Tooltip("If player is speaking, as in making a choice, this is the simplified that appears above dialogue options")]
        [SerializeField] string simpleQuestion = "";
        [SerializeField] string onEnterAction = "";
        [SerializeField] string onExitAction = "";
        [SerializeField] Condition condition;
        [SerializeField] bool requiresFriendshipCheck = false;
        [SerializeField] bool requiresFriendship = false;
        [Tooltip("Select this to make the dialogue show a next button to open the shop")]
        [SerializeField] bool isOpeningShop = false;
        DialogueNode parentNode = null;

        public bool GetShouldCheckFriendship()
        {
            return requiresFriendshipCheck;
        }

        public bool GetIsOpeningShop()
        {
            return isOpeningShop;
        }

        public bool GetRequiresFriendship()
        {
            return requiresFriendship;
        }

        public string GetText()
        {
            return text;
        }

        public string GetSimpleQuestion()
        {
            return simpleQuestion;
        }        

        public List<string> GetChildren()
        {
            return children;
        }

        public DialogueNode GetParentNode()
        {
            return parentNode;
        }

        public bool GetIsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public Rect GetNodeRect()
        {
            return rect;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }

        public bool HasCondition()
        {
            return condition.HasCondition();
        }

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

#if UNITY_EDITOR
        public void SetText(string newText)
        {
            if (text != newText)
            {
                Undo.RecordObject(this, "Set Dialogue Text");
                text = newText;
            }
        }

        public void SetSimpleQuestion(string newQuestion)
        {
            if (simpleQuestion != newQuestion)
            {
                Undo.RecordObject(this, "Added Simple Question");
                simpleQuestion = newQuestion;
            }
        }

        public void AddToChildren(string child)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(child);
        }

        public void RemoveFromChildren(string child)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(child);
        }

        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Set Node Position");
            rect.position = newPosition;
        }

        public void SetIsPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Set IsPlayerSpeaking");
            isPlayerSpeaking = newIsPlayerSpeaking;
        }

        public void SetParentNode(DialogueNode parent)
        {
            parentNode = parent;
        }

        

#endif
    }
}
