using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueFriendCondition : MonoBehaviour, ISaveable
    {
        [SerializeField] bool befriended = false;
        [SerializeField] GameObject otherFriend = null;

        public void Befriend()
        {
            befriended = true;
        }

        public bool GetIsBefriended()
        {
            return befriended;
        }

        public void BefriendOtherGameObject()
        {
            if (otherFriend == null) return;

            var other = otherFriend.GetComponent<DialogueFriendCondition>();
            if (other != null)
            {
                other.Befriend();
            }
        }

        public object CaptureState()
        {
            return befriended;
        }

        public void RestoreState(object state)
        {
            befriended = (bool)state;
        }
    }

}