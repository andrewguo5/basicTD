using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace BasicTD.Towers;

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
        public int Damage { get; set; }
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


    public TowerStatLine GetTowerStat(TowerType towerType, int level)
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
        return GetTowerStat(towerId).GetStatsForLevel(level);
    }
}

