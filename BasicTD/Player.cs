using System.Collections.Generic;
using BasicTD.Towers;

namespace BasicTD;

public class Player
{
    public int Gold { get; set; }
    public int Health { get; set; }
    public List<Tower> Inventory { get; set; }

    public Player(int startingGold, int startingHealth)
    {
        Gold = startingGold;
        Health = startingHealth;
        Inventory = new List<Tower>();
    }
}
