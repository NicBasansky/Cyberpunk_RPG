using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using RPG.Movement;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] string conversantName;
        Mover playerMover;

        private void Awake() 
        {
            playerMover = GameObject.FindWithTag("Player").GetComponent<Mover>();
        }

        public string GetConversantName()
        {
            return conversantName;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!enabled) return false;
            if (dialogue == null || !playerMover.CanMoveTo(playerMover.GetPlayerPosForSpeaking(transform)))
            {
                return false;
            }
            if (Input.GetMouseButtonDown(0)) 
            {
                // maybe wait for the player to be in a certain range and not being attacked
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
            }
            return true;
        }
    }

}