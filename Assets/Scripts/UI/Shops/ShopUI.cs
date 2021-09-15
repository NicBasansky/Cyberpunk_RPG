using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Shops;
using TMPro;

namespace RPG.UI
{
    public class ShopUI : MonoBehaviour
    {
        PlayerShopper playerShopper;
        [SerializeField] GameObject shopItemPrefab;
        [SerializeField] Transform shopSelectionsTransform;
        [SerializeField] Button exitButton;
        [SerializeField] TextMeshProUGUI shopTitle;
        [SerializeField] TextMeshProUGUI shopFlavourText;
        [SerializeField] TextMeshProUGUI playerWalletText;

        void Start()
        {
            playerShopper = GameObject.FindWithTag("Player").GetComponent<PlayerShopper>();
            playerShopper.onUpdateUI += UpdateShopUI;
            exitButton.onClick.AddListener(() => playerShopper.Quit());

            UpdateShopUI();
        }

        private void UpdateShopUI()
        {
            gameObject.SetActive(playerShopper.IsShopActive());
            if (!playerShopper.IsShopActive())
            {
                return;
            }
            shopTitle.text = playerShopper.GetCurrentShopTitle();
            shopFlavourText.text = playerShopper.GetCurrentShopFlavourText();
            playerWalletText.text = string.Format("{0:N0}", playerShopper.playerWallet);
            foreach (Transform child in shopSelectionsTransform)
            {
                Destroy(child.gameObject);
            }
            foreach (ShopItem item in playerShopper.GetShopItems())
            {
                GameObject shopItemInstance = Instantiate(shopItemPrefab, shopSelectionsTransform);
                StoreItemUI storeUI = shopItemInstance.GetComponent<StoreItemUI>();
                storeUI.Setup(item, playerShopper);
            }
        }
    }
}
