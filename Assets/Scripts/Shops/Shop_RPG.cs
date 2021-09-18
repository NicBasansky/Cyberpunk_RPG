using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using System;
using UnityEngine;
using RPG.Control;
using RPG.Inventories;

// classes with _RPG are from the course
namespace RPG.Shops_RPG
{
    public class Shop_RPG : MonoBehaviour, IRaycastable
    {
        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock = 1;
            [Range(0, 100.0f)]
            public float buyingDiscount = 0;
        }

        [SerializeField] string shopName = "";
        [SerializeField] string flavourText = "";
        [SerializeField] StockItemConfig[] stockItemConfigs;
        private ItemCategory currentCategory = ItemCategory.None;
        private bool buyMode = true;
        PlayerShopper_RPG playerShopper = null;
        Wallet playerWallet;

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();

        public event Action onUpdateUI;

        private void Awake() 
        {
            foreach(StockItemConfig config in stockItemConfigs)
            {
                stock[config.item] = config.initialStock;
            }    
        }

        public void SetPlayerShopper(PlayerShopper_RPG shopper)
        {
            playerShopper = shopper;
        }

        public IEnumerable GetAllItems()
        {
            foreach (StockItemConfig config in stockItemConfigs)
            {
                // the transaction quantity in the ShopItem gets updated here
                int transactionQuantity = 0;
                transaction.TryGetValue(config.item, out transactionQuantity);
                int currentStock = stock[config.item];
                yield return new ShopItem(config.item, currentStock,
                                                        CalculatePrice(config), transactionQuantity);
            }
        }

        public IEnumerable GetFilteredItems()
        {
            return GetAllItems();
        }

        private int CalculatePrice(StockItemConfig config)
        {
            if (config.buyingDiscount == 0)
            {
                return config.item.GetBasePrice();
            }

            int basePrice = config.item.GetBasePrice();
            float priceWithDiscout = basePrice * (1 - (config.buyingDiscount / 100));
            return Mathf.RoundToInt(priceWithDiscout);
        }

        public void SelectFilter(ItemCategory category)
        {

        }

        public ItemCategory GetFilter()
        {
            return ItemCategory.None;
        }

        public void SelectMode(bool isBuying)
        {
            buyMode = isBuying;
        }

        public bool IsBuyingMode()
        {
            return buyMode;
        }

        public void AdjustQuantityOfTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0; // this is adding it to the dictionary
            }

            // adjust stock
            if (transaction[item] + quantity > stock[item])
            {
                transaction[item] = stock[item];            
            }
            else
            {
                transaction[item] += quantity;
            }

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            if (onUpdateUI != null)
            {
                onUpdateUI();
            }
         
        }

        public void AdjustStock(InventoryItem item, int changeAmount)
        {
            stock[item] += changeAmount;
            
        }
        
        public bool CanTransact()
        {
            if (!HasTransaction()) return false;
            if (!HasSufficientFunds()) return false;        

            // not enough inventory space
            // Inventory inventory = playerWallet.GetComponent<Inventory>();
            // foreach(InventoryItem item in transaction.Keys)
            // {
            //     if (!inventory.HasSpaceFor(item))
            //     {
            //         return false;
            //     } 
            // }

            return true;
        }

        public bool HasSufficientFunds()
        {
            playerWallet = playerShopper.GetComponent<Wallet>();
            if (playerWallet == null)
            {
                print("player wallet is null");
                return false;
            }
            return playerWallet.GetMoneyAmount() >= GetTransactionTotal();
        }

        private bool HasTransaction()
        {
            return transaction.Count > 0;
        }

        public int GetTransactionTotal()
        {
            int total = 0;
            foreach(ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityOfTransaction();
            }
            return total;
        }

        public void ConfirmPurchase()
        {
            Inventory inventory = playerShopper.GetComponent<Inventory>();
            if (inventory == null || playerWallet == null) return;

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityOfTransaction();
                int price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (price > playerWallet.GetMoneyAmount())
                    {
                        // TODO message prompt for not enough money
                        break; // if not enough money, go on to the next item
                    }
                    bool success = inventory.AddToFirstEmptySlot(item, 1);
                    if (success)
                    {
                        AdjustQuantityOfTransaction(item, -1);
                        AdjustStock(item, -1);
                        playerWallet.UpdateMoneyAmount(-price);
                    }
                }                 
            }
            if (onUpdateUI != null)
            {
                onUpdateUI();
            }
        }

        public string GetShopName()
        {
            return shopName;
        }

        public string GetShopFlavourText() 
        {
            return flavourText;
        }

        public CursorType GetCursorType()
        {
            // TODO conflicts with AIconversants, dialogue cursor
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerShopper_RPG>().SetCurrentShop(this);
                playerWallet = callingController.GetComponent<Wallet>();
            }
            return true;
        }


    }

}
