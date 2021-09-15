using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPG.Shops;

namespace RPG.UI
{
    public class ShopQuantityUI : MonoBehaviour
    {
        PlayerShopper shopper;
        [SerializeField] TextMeshProUGUI wallet;
        [SerializeField] Button minusButton;
        [SerializeField] Button addButton;
        [SerializeField] TextMeshProUGUI quantity;
        [SerializeField] TextMeshProUGUI totalCost;
        [SerializeField] Button okButton;
        [SerializeField] Button cancelButton;
        [SerializeField] Image icon;
        int desiredQuantity = 1;
        ShopItem shopItem = null;
        MessageHandler messageHandler = null;


        void Start() 
        {
            shopper = GameObject.FindWithTag("Player").GetComponent<PlayerShopper>();
            shopper.onUpdateUI += UpdateUI;
            AddListeners();
            UpdateUI();

            messageHandler = GetComponent<MessageHandler>();
        }

        private void AddListeners()
        {
            minusButton.onClick.AddListener(() =>
                {
                    desiredQuantity--;
                    desiredQuantity = GetDesiredQuantity();
                    UpdateUI();
                });
            addButton.onClick.AddListener(() =>
                {
                    desiredQuantity++;
                    desiredQuantity = GetDesiredQuantity();
                    UpdateUI();
                });
            okButton.onClick.AddListener(ConfirmPurchase);
            cancelButton.onClick.AddListener(Quit);
        }

        private void CheckIfShopItemChanged()
        {
            if (shopItem != shopper.GetSelectedShopItem())
            {
                shopItem = shopper.GetSelectedShopItem();
                desiredQuantity = 1;
            }
        }

        private void UpdateUI()
        {
            gameObject.SetActive(shopper.GetIsConfirming());
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            CheckIfShopItemChanged();
            if (shopItem == null) return;

            wallet.text = string.Format("{0:N0}", shopper.playerWallet);
            quantity.text = GetDesiredQuantity().ToString();
            totalCost.text = string.Format("{0:N0}", desiredQuantity * shopItem.pricePerUnit);
            icon.sprite = shopItem.item.GetIcon();
        }
        
        private int GetDesiredQuantity() 
        {
            if (desiredQuantity < 1)
            {
                desiredQuantity = shopItem.stockQuantity;
                return desiredQuantity;
            }
            else if (desiredQuantity > shopItem.stockQuantity)
            {
                desiredQuantity = 1;
                return desiredQuantity;
            }
            else
            {
                return desiredQuantity;
            }
        }

        private int GetTotalCost()
        {
            return desiredQuantity * shopItem.pricePerUnit;
        }

        private void ConfirmPurchase()
        {
            if (shopper.playerWallet >= GetTotalCost())
            {
                bool addedToInventoryMessageSent = false;
                bool inventoryFullMessageSent = false;
                for (int i = 0; i < desiredQuantity; i++)
                {         
                    if (shopper.HasRoomInInventory(shopItem.item))
                    {
                        shopper.AddToInventory(shopItem.item, 1);
                        shopper.playerWallet -= shopItem.pricePerUnit;
                        shopItem.stockQuantity -= 1;

                        if (!addedToInventoryMessageSent) 
                        {
                            messageHandler.ShowMessage(MessageType.AddedToInventory);
                            addedToInventoryMessageSent = true;
                        }                     
                    }
                    else
                    {
                        if (!inventoryFullMessageSent)
                        {
                            messageHandler.ShowMessage(MessageType.InventoryFull);
                            inventoryFullMessageSent = true;
                        }                      
                    }
                }
                Quit();
            }
        }

        private void Quit()
        {
            desiredQuantity = 1;
            shopper.CloseConfirmationWindow();
        }

    }
}
