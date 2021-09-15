using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public enum MessageType
    {
        AddedToInventory,
        InventoryFull,
        RemoveFromInventory
    };

    public class MessageHandler : MonoBehaviour
    {
        [SerializeField] GameObject inventoryFullPrefab;
        [SerializeField] GameObject addedToInventoryPrefab;
        [SerializeField] GameObject removedFromInventoryPrefab;
        [SerializeField] Transform messageTransform;

        // TODO add sound

        public void ShowMessage(MessageType type)
        {
            GameObject prefab = GetPrefab(type);
            GameObject prompt = Instantiate(prefab);
            prompt.transform.SetParent(messageTransform, false);
            Destroy(prompt, 3f);
        }

        private GameObject GetPrefab(MessageType type)
        {
            switch (type)
            {
                case MessageType.AddedToInventory:
                    return addedToInventoryPrefab;
                case MessageType.InventoryFull:
                    return inventoryFullPrefab;
                case MessageType.RemoveFromInventory:
                    return removedFromInventoryPrefab;
            }
            return null;
        }
    }

}
