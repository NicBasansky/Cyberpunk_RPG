using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Shops_RPG
{
    [System.Serializable]
    public class ShopItem
    {
        InventoryItem item;
        int stock = 5;
        int price = 100;
        int quantityOfTransaction = 0;

        public ShopItem(InventoryItem item, int stock, int price, int quantityOfTransaction)
        {
            this.item = item;
            this.stock = stock;
            this.price = price;
            this.quantityOfTransaction = quantityOfTransaction;
        }

        public string GetName()
        {
            return item.GetDisplayName();
        }

        public Sprite GetItemIcon()
        {
            return item.GetIcon();
        }

        public int GetInitialStock()
        {
            return stock;
        }

        public int GetPrice()
        {
            return price;
        }

        public InventoryItem GetInventoryItem()
        {
            return item;
        }

        public int GetQuantityOfTransaction()
        {
            return quantityOfTransaction;
        }

    }
}
