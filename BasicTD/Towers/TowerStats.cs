using System.Collections.Generic;

namespace BasicTD.Towers;

public static class TowerStats
{
    public static readonly Dictionary<string, float> BasicTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 5.0f },
        { "AttackSpeed", 1.0f }
    };

    public static readonly Dictionary<string, float> SniperTowerStats = new Dictionary<string, float>
    {
        { "Damage", 2f },
        { "Range", 8.0f },
        { "AttackSpeed", 0.5f }
    };

    public static readonly Dictionary<string, float> SplashTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 3.0f },
        { "AttackSpeed", 1.0f }
    };

    public static readonly Dictionary<TowerType, Dictionary<string, float>> AllTowerStats =
        new Dictionary<TowerType, Dictionary<string, float>>
        {
            { TowerType.Basic, BasicTowerStats },
            { TowerType.Sniper, SniperTowerStats },
            { TowerType.Splash, SplashTowerStats }
        };
}
