using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Shops_RPG;
using UnityEngine.UI;
using TMPro;

namespace RPG.UI.Shops
{
    public class ShopRowUI_RPG : MonoBehaviour
    {
        ShopItem shopItem = null;
        Shop_RPG currentShop = null;
        [SerializeField] Image iconImage;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI stockText;
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] TextMeshProUGUI quantityText;

        public void SetShopItem(ShopItem item, Shop_RPG shop)
        {
            shopItem = item;
            currentShop = shop;
            SetUpUI();
    
        }

        private void SetUpUI()
        {
            if (shopItem == null) return;

            iconImage.sprite = shopItem.GetItemIcon();
            nameText.text = shopItem.GetName();
            stockText.text = $"{shopItem.GetInitialStock()}"; //shopItem.GetStock().ToString();
            priceText.text = $"{shopItem.GetPrice():N0}";   //$"{shopItem.GetPrice()}"; 
            quantityText.text = $"{shopItem.GetQuantityOfTransaction()}";

        }

        public void Add()
        {
            currentShop.AdjustQuantityOfTransaction(shopItem.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            currentShop.AdjustQuantityOfTransaction(shopItem.GetInventoryItem(), -1);
        }
    }
}
