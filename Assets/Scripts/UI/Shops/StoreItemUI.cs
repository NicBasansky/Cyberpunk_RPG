using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Shops;
using UnityEngine.UI;
using TMPro;

namespace RPG.UI
{
    public class StoreItemUI : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI costField;
        [SerializeField] TextMeshProUGUI itemNameField;
        [SerializeField] TextMeshProUGUI itemDescriptionField;
        [SerializeField] TextMeshProUGUI stockQuantityField;
        [SerializeField] Button button;
        PlayerShopper playerShopper;


        public void Setup(ShopItem shopItem, PlayerShopper shopper)
        {
            
            SetupIcon(shopItem);
            costField.text = string.Format("{0:N0}", shopItem.pricePerUnit);
            itemNameField.text = shopItem.item.GetDisplayName();
            itemDescriptionField.text = shopItem.item.GetDescription();
            stockQuantityField.text = shopItem.stockQuantity.ToString();

            playerShopper = shopper;
            button.onClick.AddListener(() => playerShopper.SelectItem(shopItem));

        }

        private void SetupIcon(ShopItem shopItem)
        {
            var iconImage = icon.GetComponent<Image>();
            if (shopItem == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = shopItem.item.GetIcon();
            }
        }


    }
}
