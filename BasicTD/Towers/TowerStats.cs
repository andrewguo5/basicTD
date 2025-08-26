using System.Collections.Generic;

namespace BasicTD.Towers;

public static class TowerStats
{
    public static readonly Dictionary<string, float> LightTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 3.0f },
        { "AttackSpeed", 1.0f }
    };

    public static readonly Dictionary<string, float> HeavyTowerStats = new Dictionary<string, float>
    {
        { "Damage", 2f },
        { "Range", 3.0f },
        { "AttackSpeed", 0.7f }
    };

    public static readonly Dictionary<string, float> PulseTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 3.0f },
        { "AttackSpeed", 1.0f }
    };
    public static readonly Dictionary<string, float> ShockwaveTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 3.0f },
        { "AttackSpeed", 1.0f }
    };
    public static readonly Dictionary<string, float> VulnTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 3.0f },
        { "AttackSpeed", 1.0f }
    };
    public static readonly Dictionary<string, float> BeaconTowerStats = new Dictionary<string, float>
    {
        { "Damage", 1f },
        { "Range", 3.0f },
        { "AttackSpeed", 1.0f }
    };

    public static readonly Dictionary<TowerType, Dictionary<string, float>> AllTowerStats =
        new Dictionary<TowerType, Dictionary<string, float>>
        {
            { TowerType.Light, LightTowerStats },
            { TowerType.Heavy, HeavyTowerStats },
            { TowerType.Pulse, PulseTowerStats },
            { TowerType.Shockwave, ShockwaveTowerStats },
            { TowerType.Vuln, VulnTowerStats },
            { TowerType.Beacon, BeaconTowerStats }
        };
}
