using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Movement;
//using RPG.Shops;
using UnityEngine;

// TODO find a way to update the simple question above player choices
namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;
        [SerializeField] float maxDistFromSpeakers = 20f;
        Dialogue currentDialogue;
        DialogueNode currentNode;
        AIConversant currentConversant = null;
        bool isChoosing = false;
        Transform speakerTransform = null;

        public event Action onConversationUpdated;


        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            speakerTransform = newConversant.transform;
            WalkTowardsSpeaker(newConversant.transform);

            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            currentConversant = newConversant;
            TriggerEnterAction();
            onConversationUpdated();
            
            
        }

        private void Update()
        {
            if (currentDialogue != null)
            {
                if ((speakerTransform.position - transform.position).magnitude > maxDistFromSpeakers)
                {
                    speakerTransform = null;
                    Quit();
                }
            }
        }

        private void WalkTowardsSpeaker(Transform speakerTransform)
        {
            Mover mover = GetComponent<Mover>();
            mover.MoveToFrontOfSpeaker(speakerTransform, 1);
        }

        public string GetCurrentConversantName()
        {
            if (currentNode == null) return "";
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetConversantName();
            }
        }

        // return filtered on condition?
        public string GetSimplifiedQuestion()
        {
            if (currentNode == null) return "";

            foreach (DialogueNode node in currentDialogue.GetPlayerChildren(currentNode))
            {
               return node.GetSimpleQuestion();
            }
            return "";
        }

        public bool IsDialogueActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }
            return currentNode.GetText();
        }

        public void Next()
        {
            if (currentNode.GetIsOpeningShop())
            {
                Quit(); // still want a next button to close dialogue window if opening a shop
                return;
            }
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            // choose a random response
            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            int randomChildIndex = UnityEngine.Random.Range(0, children.Length);
            TriggerExitAction();
            if (children.Length >= 1)
            {
                currentNode = children[randomChildIndex];
            }
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {
            //return currentNode.GetChildren().Count > 0;
            
            if (currentNode.GetIsOpeningShop())
            {
                return true; // todo still need to quit dialogue when hitting the next button if 
                                // is opening shop
            }
            
            if (currentNode.GetChildren().Count == 0)
            {
                return false;
            }
            foreach(DialogueNode child in currentDialogue.GetAllChildren(currentNode))
            {
                if (!child.HasCondition())
                {
                    return true;
                }
            }
            if (FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)); 
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach(DialogueNode node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    if(!node.GetShouldCheckFriendship())
                    {
                        yield return node;
                    }
                    else
                    {
                        if (FilterOnFriendCheck(node))
                        {
                            yield return node;
                        }
                        
                    }
                }
            }
        }

        private bool FilterOnFriendCheck(DialogueNode node)
        {
            var friendCheck = currentConversant.GetComponent<DialogueFriendCondition>();
            if (friendCheck != null)
            {
                bool isFriendly = friendCheck.GetIsBefriended();

                if (node.GetRequiresFriendship() && isFriendly)
                {
                    return true;
                }
                else if (!node.GetRequiresFriendship() && !isFriendly)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                if (currentConversant != null)
                {
                    TriggerAction(currentNode.GetOnEnterAction());    
                }
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                if (currentConversant != null)
                {
                    TriggerAction(currentNode.GetOnExitAction());
                }
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return; 

            foreach (var trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        public void Quit()
        {
            TriggerExitAction();
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated();
        }

    }

}