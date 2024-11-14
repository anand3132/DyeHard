using RedGaint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    BotInventory inventory;
    BotController BotController;
    private void Start()
    {
        if (inventory == null)
        {
            BugsBunny.LogError("Bot: Bot has no inventory to play");
            //DestroyObject(this);
        }
    }

    public void SetBotInventory(BotInventory inventory)
    { this.inventory = inventory; }


}
