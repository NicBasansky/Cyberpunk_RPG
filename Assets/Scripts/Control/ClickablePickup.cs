using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using UnityEngine.Events;

namespace RPG.Control
{

    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;
        public UnityEvent onPickedUp;

        void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pickup.PickupItem();
                onPickedUp.Invoke();
            }
            return true;
        }
    }
}
