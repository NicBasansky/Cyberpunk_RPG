using UnityEngine;
using UnityEngine.UI;
using RPG.Shops_RPG;
using System;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        Button button;
        Shop_RPG currentShop;
        [SerializeField] ItemCategory itemCategory = ItemCategory.None;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop_RPG currentShop)
        {
            this.currentShop = currentShop;
        }

        private void SelectFilter()
        {
            currentShop.SelectFilter(itemCategory);           
    
        }

        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != itemCategory;
        }

    }
}
