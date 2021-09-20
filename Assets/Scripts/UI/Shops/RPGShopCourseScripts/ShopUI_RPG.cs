using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Shops_RPG;
using TMPro;
using RPG.Inventories;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI_RPG : MonoBehaviour
    {
        PlayerShopper_RPG playerShopper;
        Shop_RPG currentShop;
        [SerializeField] GameObject uiParent;
        [SerializeField] TextMeshProUGUI titleText;
        //[SerializeField] TextMeshProUGUI moneyText;
        [SerializeField] TextMeshProUGUI flavourText;
        [SerializeField] Transform shopContentsParent;
        [SerializeField] ShopRowUI_RPG shopRowUiPrefab;
        [SerializeField] TextMeshProUGUI totalTransactionText;
        [SerializeField] TextMeshProUGUI moneyText;
        [SerializeField] Button confirmButton;
        [SerializeField] Button switchButton;
        [SerializeField] TextMeshProUGUI buyButtonText;
        [SerializeField] TextMeshProUGUI switchButtonText;
        [SerializeField] Color insufficientFundsTextColour;
        Color originalTotalTextColour;
        Wallet playerWallet;


        void Awake()
        {
            playerShopper = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShopper_RPG>();
            playerShopper.activeShopChanged += OnShopChanged;

            playerWallet = playerShopper.GetComponent<Wallet>();
            playerWallet.onMoneyUpdated += UpdateMoneyUI;

            confirmButton.onClick.AddListener(ConfirmPurchase);
            switchButton.onClick.AddListener(SwitchMode);
        }

        void Start()
        {
            originalTotalTextColour = totalTransactionText.color;
            OnShopChanged();

        }

        private void OnShopChanged()
        {
            currentShop = playerShopper.GetActiveShop();
            if (currentShop != null)
            {
                currentShop.onUpdateUI -= RefreshUIContents;
            }
            
            // TODO make the shop open from a dialogue trigger and fix the cursor
            // conflict within the aiConversant
            uiParent.SetActive(currentShop != null);

            if (currentShop == null)
            {                             
                return;
            }

            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(currentShop);
                
            }

            currentShop.onUpdateUI += RefreshUIContents;

            SetUpShopName();
            RefreshUIContents();
        }

        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuying());
        }

        private void SetUpShopName()
        {
            titleText.text = currentShop.GetShopName();
            flavourText.text = currentShop.GetShopFlavourText();

        }


        private void RefreshUIContents()
        {
            // remove all rows in contents first
            foreach (Transform row in shopContentsParent)
            {
                Destroy(row.gameObject);
            }

            foreach(ShopItem item in currentShop.GetFilteredItems())
            {
                ShopRowUI_RPG rowUI = Instantiate(shopRowUiPrefab, shopContentsParent);
                rowUI.SetShopItem(item, currentShop);
            }

            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }

            totalTransactionText.text = $"{currentShop.GetTransactionTotal():N0}";
            totalTransactionText.color = currentShop.HasSufficientFunds() ? originalTotalTextColour : insufficientFundsTextColour;
            confirmButton.interactable = currentShop.CanTransact();

            buyButtonText.text = currentShop.IsBuying() ? "Buy" : "Sell";
            switchButtonText.text = currentShop.IsBuying() ? "Switch To Selling" : "Switch To Buying";
        }

        public void ConfirmPurchase()
        {
            currentShop.ConfirmPurchase();
        }

        public void CloseShop()
        {
            playerShopper.SetCurrentShop(null);

        }

        private void UpdateMoneyUI()
        {
            moneyText.text = $"{playerWallet.GetMoneyAmount():N0}";
        }
    }

}
