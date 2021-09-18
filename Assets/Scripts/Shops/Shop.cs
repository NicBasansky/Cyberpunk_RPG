using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using GameDevTV.Saving;

// TODO make the stock reset at some point with an event
namespace RPG.Shops
{
    [System.Serializable]
    public class ShopItem
    {
        public InventoryItem item;
        public int stockQuantity;
        public int pricePerUnit;
    }
    
    public class Shop : MonoBehaviour, ISaveable
    {
        [SerializeField] string shopTitle;
        [SerializeField] string shopFlavourText;
        [SerializeField] List<ShopItem> shopItems = new List<ShopItem>();

        public void OpenShop() // called by event
        {
           GameObject.FindWithTag("Player").GetComponent<PlayerShopper>().Setup(this);
        }

        public IEnumerable<ShopItem> GetShopItems()
        {
            return shopItems;
        }

        public string GetShopTitle()
        {
            return shopTitle;
        }

        public string GetShopFlavourText()
        {
            return shopFlavourText;
        }

        public object CaptureState()
        {
            Dictionary<string, int> state = new Dictionary<string, int>();
            foreach (var shopItem in shopItems)
            {
                state[shopItem.item.GetItemID()] = shopItem.stockQuantity;
            }
            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> quantities = (Dictionary<string, int>)state;
            foreach (var shopItem in shopItems)
            {
                if (quantities.ContainsKey(shopItem.item.GetItemID()))
                {
                    shopItem.stockQuantity = quantities[shopItem.item.GetItemID()];
                }
            }                       
        }

    }

}
