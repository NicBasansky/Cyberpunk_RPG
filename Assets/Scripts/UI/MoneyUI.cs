using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Shops;
using TMPro;

namespace RPG.UI
{
    public class MoneyUI : MonoBehaviour
    {
        PlayerShopper shopper;
        [SerializeField] TextMeshProUGUI moneyText;

        private void Awake() 
        {
            shopper = GameObject.FindWithTag("Player").GetComponent<PlayerShopper>();
            shopper.onUpdateUI += UpdateUI;
        }

        private void UpdateUI()
        {
            moneyText.text = string.Format("{0:N0}", shopper.playerWallet);
        }
    }
}
