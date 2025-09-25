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
    public int Level { get; set; }

    // Why here? Perhaps certain relics can interact with it.
    public TowerInfo TowerInfo { get; set; }

    public Player()
    {
        Reset();
        Level = 0;
        TowerInfo = TowerInfo.FromJsonFile(Core.Content, "stats/tower-info.json");
    }

    public void SetInventory(Inventory inventory)
    {
        // Add a refernece to the inventory component so we can update it when we purchase cards.
        Inventory = inventory;
    }

    public bool PurchaseCard(Card card)
    {
        if (Inventory == null)
        {
            throw new InvalidOperationException("Inventory not set for player.");
        }

        if (Gold >= card.Cost)
        {
            if (card.Rarity == CardRarity.Null)
            {
                return false;
            }

            if (Inventory.Cards.Count >= 5)
            {
                System.Console.WriteLine("Inventory full. Cannot purchase more cards.");
                return false;
            }
            Inventory.AddCard(card);
            Gold -= card.Cost;
            System.Console.WriteLine($"Added {card.Rarity} {card.TowerType} tower to inventory.");
            return true;
        }
        else
        {
            System.Console.WriteLine($"Not enough gold to purchase {card.Rarity} {card.TowerType} tower.");
            return false;
        }
    }

    public void Reset()
    {
        Gold = 5;
        Health = 3;
    }
}
