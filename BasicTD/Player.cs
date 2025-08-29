using System.Collections.Generic;
using BasicTD.Components;
using BasicTD.Towers;
using System;
using MonoGameLibrary;

namespace BasicTD;

public class Player
{
    public int Gold { get; set; }
    public int Health { get; set; }
    public Inventory Inventory { get; set; }

    // Why here? Perhaps certain relics can interact with it.
    public TowerInfo TowerInfo { get; set; }

    public Player(int startingGold, int startingHealth)
    {
        Gold = startingGold;
        Health = startingHealth;
        TowerInfo = TowerInfo.FromJsonFile(Core.Content, "tower-info.json");
    }

    public void SetInventory(Inventory inventory)
    {
        // Add a refernece to the inventory component so we can update it when we purchase cards.
        Inventory = inventory;
    }

    public void PurchaseCard(Card card)
    {
        if (Inventory == null)
        {
            throw new InvalidOperationException("Inventory not set for player.");
        }

        Console.WriteLine($"Added {card.Rarity} {card.TowerType} tower to inventory.");
        Inventory.AddCard(card);
    }
}
