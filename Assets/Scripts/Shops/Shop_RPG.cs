using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using System;
using UnityEngine;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using GameDevTV.Saving;

// classes with _RPG are from the course
namespace RPG.Shops_RPG
{
    public class Shop_RPG : MonoBehaviour, IRaycastable, ISaveable
    {
        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock = 1;
            [Range(0, 100.0f)]
            public float buyingDiscount = 0;
            public int levelToUnlock = 0;
        }

        [SerializeField] string shopName = "";
        [SerializeField] string flavourText = "";
        [SerializeField] StockItemConfig[] stockItemConfigs;
        [Tooltip("Percentage of selling price will vendor buy back item")]
        [Range(0, 100)]
        [SerializeField] float sellingDiscount = 60;
        [Tooltip("The maximum discount a shopkeeper will provide from discounts generated from a high Charisma stat")]
        [Range(0, 100)]
        [SerializeField] float maximumBarterDiscount = 80f;
        private ItemCategory currentCategoryFilter = ItemCategory.None;
        private bool isBuying = true;
        PlayerShopper_RPG playerShopper = null;
        Wallet playerWallet;
        Inventory inventory;

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();

        public event Action onUpdateUI;

        private void Awake() 
        {
            inventory = Inventory.GetPlayerInventory();
        }

        public void SetPlayerShopper(PlayerShopper_RPG shopper)
        {
            playerShopper = shopper;
            isBuying = true;
        }

        public IEnumerable GetAllItems()
        {
            //int shopperLevel = GetShopperLevel();
            Dictionary<InventoryItem, int> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();
            foreach (InventoryItem item in availabilities.Keys)
            {
                //if (config.levelToUnlock <= shopperLevel)
                if (availabilities[item] <= 0) continue;

                int price = prices[item];
                // the transaction quantity in the ShopItem gets updated here
                int transactionQuantity = 0;
                transaction.TryGetValue(item, out transactionQuantity);
                
                //int currentStock = stock[config.item];
                int currentStock = availabilities[item];
                
                yield return new ShopItem(item, currentStock,
                                                        price, transactionQuantity);
            
            }
        }     

        public IEnumerable GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if (currentCategoryFilter == ItemCategory.None ||
                                     item.GetItemCategory() == currentCategoryFilter)
                    yield return shopItem;
            }

        }


        private int GetPriceWithBuyingDiscount(StockItemConfig config)
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
            currentCategoryFilter = category;
            if (onUpdateUI != null)
            {
                onUpdateUI();
            }
        }

        public ItemCategory GetFilter()
        {
            return currentCategoryFilter;
        }

        public void SelectMode(bool isBuying)
        {
            this.isBuying = isBuying;
            transaction.Clear();
            if (onUpdateUI != null)
            {
                onUpdateUI(); 
            }
        }

        public bool IsBuying()
        {
            return isBuying;
        }

        public void AdjustQuantityOfTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0; // this is adding it to the dictionary
            }

            // adjust stock
            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;            
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

        public void AddToStockSold(InventoryItem item, int changeAmount)
        {
            if (!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
            }
            stockSold[item] += changeAmount;
        }
        
        public bool CanTransact()
        {
            if (!HasTransaction()) return false;
            if (!HasSufficientFunds()) return false; 
            if (!HasInventorySpace()) return false;

            return true;
        }

        public bool HasSufficientFunds()
        {
            if (!isBuying) return true;
    
            playerWallet = playerShopper.GetComponent<Wallet>();
            if (playerWallet == null)
            {
                print("player wallet is null");
                return false;
            }
            return playerWallet.GetMoneyAmount() >= GetTransactionTotal();
        }

        // TODO show a message prompt that there will not be enough space in the inventory
        private bool HasInventorySpace()
        {
            if (!isBuying) return true;

            if (inventory == null) return false;

            List<InventoryItem> flatList = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityOfTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatList.Add(item);
                }
            }
            if (inventory.HasSpaceFor(flatList))
            {
                return true;
            }
            return false;
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
            if (inventory == null || playerWallet == null) return;

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityOfTransaction();
                int price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuying)
                    {
                        BuyItem(item, price);
                    }
                    else
                    {
                        SellItem(item, price);
                    }
                }                 
            }
            if (onUpdateUI != null)
            {
                onUpdateUI();
            }
        }

        // player buying from the vendor
        private void BuyItem(InventoryItem item, int price)
        {
            if (price > playerWallet.GetMoneyAmount())
            {
                // TODO message prompt for not enough money
                return; // if not enough money, go on to the next item
            }
            bool success = inventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AdjustQuantityOfTransaction(item, -1);
                AddToStockSold(item, 1);
                playerWallet.UpdateMoneyAmount(-price);
            }
        }

        // player selling to vendor
        private void SellItem(InventoryItem item, int price)
        {
            int slot = FindFirstItemSlot(item);
            if (slot == -1) return;

            AdjustQuantityOfTransaction(item, -1);
            inventory.RemoveFromSlot(slot, 1);
            AddToStockSold(item, -1);
            playerWallet.UpdateMoneyAmount(price);
        }

        private Dictionary<InventoryItem, int> GetPrices()
        {
            Dictionary<InventoryItem, int> prices = new Dictionary<InventoryItem, int>();
            foreach(StockItemConfig config in GetAvailableConfigs())
            {       
                if (!prices.ContainsKey(config.item))
                {
                    prices[config.item] = Mathf.FloorToInt(config.item.GetBasePrice() * GetBarterDiscount());
                }
                if (isBuying)
                {
                    prices[config.item] = Mathf.FloorToInt(prices[config.item] * (100 - config.buyingDiscount)/100);
                }
                else
                {
                    prices[config.item] = Mathf.FloorToInt(prices[config.item] * ((100 - sellingDiscount) / 100));
                }
                
            }
            return prices;
        }

        private float GetBarterDiscount()
        {
            BaseStats baseStats = playerShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return (1 - Mathf.Min(discount, maximumBarterDiscount) / 100);
        }

        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();
            
            if (isBuying)
            {
                foreach(StockItemConfig config in GetAvailableConfigs())
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        availabilities[config.item] = 0;
                        int sold = 0;
                        stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }
                    availabilities[config.item] += config.initialStock;
                }
            }
            else // if player is selling back to the vendor
            {
                foreach(StockItemConfig config in GetAvailableConfigs())
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        availabilities[config.item] = 0;
                    }
                    availabilities[config.item] = CountItemsInInventory(config.item);                }
            }
            return availabilities;
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (ReferenceEquals(inventory.GetItemInSlot(i), item))
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }

            return total;
        }

        private IEnumerable GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach(var config in stockItemConfigs)
            {
                if (config.levelToUnlock > shopperLevel) continue;
                yield return config;
            }
        }

        private int FindFirstItemSlot(InventoryItem item)
        {
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (item == inventory.GetItemInSlot(i))
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetShopperLevel()
        {
            BaseStats stats = playerShopper.GetComponent<BaseStats>();
            if (stats == null) return 0;

            return stats.GetLevel();
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

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach(var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = stockSold[pair.Key];
            }
            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            stockSold.Clear();
            foreach(var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
            
        }

    }

}
