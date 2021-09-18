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

            currentShop.onUpdateUI += RefreshUIContents;

            SetUpShopName();
            RefreshUIContents();
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

            totalTransactionText.text = $"{currentShop.GetTransactionTotal():N0}";
            totalTransactionText.color = currentShop.HasSufficientFunds() ? originalTotalTextColour : insufficientFundsTextColour;
            confirmButton.interactable = currentShop.CanTransact();

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
