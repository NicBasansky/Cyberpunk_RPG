using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Inventories;

namespace RPG.UI.Shops
{
    // similar to MoneyUI, except this version uses classes made with course
    public class MoneyUI_RPG : MonoBehaviour
    {
        Wallet playerWallet;
        [SerializeField] TextMeshProUGUI moneyText;

        private void Awake()
        {
            playerWallet = GameObject.FindWithTag("Player").GetComponent<Wallet>();
            playerWallet.onMoneyUpdated += UpdateUI;
        }

        private void UpdateUI()
        {
            moneyText.text = string.Format("{0:N0}", playerWallet.GetMoneyAmount());
        }
    }
}