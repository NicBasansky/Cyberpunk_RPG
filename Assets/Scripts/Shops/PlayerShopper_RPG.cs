using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace RPG.Shops_RPG
{
    public class PlayerShopper_RPG : MonoBehaviour
    {
        Shop_RPG currentShop = null;
        public event Action activeShopChanged;

        public void SetCurrentShop(Shop_RPG shop)
        {
            // if current shop is previously not null then this method is now called to unset
            // the current shop, meaning we should unset the player shopper 
            if (currentShop != null)
            {
                currentShop.SetPlayerShopper(null);
            }
            currentShop = shop;
            

            // by now the shop should have been set to something, in which case, set the player shopper
            if (currentShop != null)
            {
                currentShop.SetPlayerShopper(this);
            }
            
            if (activeShopChanged != null)
            {
                activeShopChanged();
            }
        }

        public Shop_RPG GetActiveShop()
        {
            return currentShop;
        }

        public void CloseShop()
        {
            currentShop = null;
        }
    }

}
