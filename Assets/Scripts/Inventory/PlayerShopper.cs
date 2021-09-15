using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using GameDevTV.Saving;

namespace RPG.Shops
{
    public class PlayerShopper : MonoBehaviour, ISaveable
    {
        public int playerWallet = 100; // TODO saving system
        Shop currentShop = null;
        bool isShopActive = false;
        bool isConfirming = false;
        ShopItem selectedShopItem = null;
        Inventory playerInventory = null;

        public event Action onUpdateUI;

        private void Awake() 
        {
            playerInventory = GetComponent<Inventory>();
        }

        public void Setup(Shop shop)
        {
            currentShop = shop;
            isShopActive = true;
            onUpdateUI();
        }

        public void SelectItem(ShopItem shopItem)
        {
            // Bring up sub menu for quantity, price and confirmation
            selectedShopItem = shopItem;
            isConfirming = true;
            onUpdateUI();
        }

        public bool IsShopActive()
        {
            return isShopActive;
        }

        public bool GetIsConfirming()
        {
            return isConfirming;
        }

        public bool HasRoomInInventory(InventoryItem item)
        {
            return playerInventory.HasSpaceFor(item);
        }

        public void AddToInventory(InventoryItem item, int number)
        {
            playerInventory.AddToFirstEmptySlot(item, number);
        }

        public void RemoveFromInventory(Inventory item, int number)
        {
           // playerInventory.RemoveFromSlot()
        }

        public void CloseConfirmationWindow()
        {
            isConfirming = false;
            selectedShopItem = null;
            onUpdateUI();
        }

        public ShopItem GetSelectedShopItem() 
        {
            if (selectedShopItem != null)
            {
                return selectedShopItem;
            }
            return null;
        }

        public string GetCurrentShopTitle()
        {
            return currentShop.GetShopTitle();
        }

        public string GetCurrentShopFlavourText()
        {
            return currentShop.GetShopFlavourText();
        }

        public void Quit()
        {
            currentShop = null;
            isShopActive = false;
            onUpdateUI();
        }

        public IEnumerable<ShopItem> GetShopItems()
        {
            return currentShop.GetShopItems();
        }

        public object CaptureState()
        {
            return playerWallet;
        }

        public void RestoreState(object state)
        {
            playerWallet = (int)state;
            onUpdateUI();
        }
    }

}