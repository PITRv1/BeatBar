using Godot;
using System;

public partial class ShopKeeper : BaseNpc
{
    bool canUpgradeHealth = false;
    bool canUpgradeDamageMult = false;


    public void UpgradePlayerHealth()
    {
        canUpgradeHealth = false;

        int playerMoney = Global.Instance.player.money;
        if (playerMoney >= 20)
        {
            canUpgradeHealth = true;

            Global.Instance.player.beaterDataComponent.beaterData.health += 5;
            Global.Instance.player.TakeMoney(20);
        }

        
    }
    
    public void UpgradePlayerDamageMult()
    {
        canUpgradeDamageMult = false;

        int playerMoney = Global.Instance.player.money;
        if (playerMoney >= 25)
        {
            canUpgradeDamageMult = true;

            Global.Instance.player.beaterDataComponent.beaterData.damageMultiplier += .2f;
            Global.Instance.player.TakeMoney(25);
        }
    }
}
