using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using RPG.UI;

namespace RPG.Inventories
{
    public class ItemTaker : MonoBehaviour
    {
        [SerializeField] InventoryItem itemToTake;
        [SerializeField] int number = 1;

        public void RemoveItemFromPlayerInventory()
        {
            Inventory inventory = Inventory.GetPlayerInventory();
            if (!itemToTake.IsStackable())
            {
                RemoveNonStackableItems(inventory);
                return;
            }

            int slotIndex;
            if (inventory.HasItem(itemToTake, out slotIndex))
            {
                if (inventory.GetNumberInSlot(slotIndex) >= number)
                {
                    inventory.RemoveFromSlot(slotIndex, number);
                    inventory.GetComponent<MessageHandler>().ShowMessage(MessageType.RemoveFromInventory);
                }
            }
        }

        private void RemoveNonStackableItems(Inventory inventory)
        {
            int numItemsInInventory = inventory.HowManyNonStackableItems(itemToTake);
            //Debug.Log("Want to remove " + numItemsInInventory + " of " + itemToTake.name);
            if (number <= numItemsInInventory)
            {
                for (int i = 0; i < number; i++)
                {
                    int index;
                    if (inventory.HasItem(itemToTake, out index))
                    {
                        inventory.RemoveFromSlot(index, 1);
                        inventory.GetComponent<MessageHandler>().ShowMessage(MessageType.RemoveFromInventory);
                    }
                    
                }
            }
            else
            {
                // TODO fill this out when it comes time for the NPC to want to remove more that one item
                // Will need to also adjust the implementation of IPredicateEvaluator in Inventory
                // not enough items in inventory
            }
        }
    }
}
