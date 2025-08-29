using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

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

public class TowerInfo
{
    public static TowerInfo FromJson(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<TowerInfo>(json);
    }

    public static TowerInfo FromJsonFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);
        string json = File.ReadAllText(filePath);
        return FromJson(json);
    }

    public List<TowerStat> Towers { get; set; }

    public class TowerStat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nick { get; set; }
        public string Desc { get; set; }
        public List<TowerStatLine> Stats { get; set; }

        public TowerStatLine GetStatsForLevel(int level)
        {
            return Stats.Find(s => s.Level == level);
        }
    }

    public class TowerStatLine
    {
        public int Level { get; set; }
        public float Damage { get; set; }
        public float Range { get; set; }
        public float Speed { get; set; }
    }

    public TowerStat GetTowerStat(int id)
    {
        return Towers.Find(t => t.Id == id);
    }

    public TowerStat GetTowerStat(TowerType towerType)
    {
        int towerId = towerType switch
        {
            TowerType.Light => 0,
            TowerType.Heavy => 1,
            TowerType.Pulse => 2,
            TowerType.Shockwave => 3,
            TowerType.Beacon => 4,
            TowerType.Vuln => 5,
            _ => -1
        };
        return GetTowerStat(towerId);
    }
}

