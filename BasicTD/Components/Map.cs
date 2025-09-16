using BasicTD.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using BasicTD.Towers;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Paths;
using MonoGameLibrary.Input;
using System.Collections.Generic;
using MonoGameLibrary.Creeps;

namespace BasicTD.Components;

public class Battlefield : GComponent
{
    // Props
    private TextureAtlas Atlas => Props["Atlas"];
    private SpriteFont GameFont => Props["GameFont"];
    private Rectangle MapBounds => Props["MapBounds"];
    private Vector2 SpriteScale => Props["SpriteScale"];
    private int SideBuffer => Props["SideBuffer"];

    // Component-specific Content
    private Tilemap PlatformTilemap;
    private Path BattlePath;
    private Sprite TowerSprite;
    private TowerFactory TowerFactory => ((GameScene)ParentScene).TowerFactory;

    // Component-specific data structures
    private List<Creep> SpawnedCreepList;
    private float CreepSpeed = 150f;
    private List<Tower> PlacedTowersList;
    public int CurrentWave = 0;
    private Queue<CreepSpawner> SpawnerQueue;

    // Tower placement variables
    private TowerType PlacingTowerType;
    private int PlacingTowerLevel;
    private bool PlacingTower;
    private Card PlacingTowerCard;
    private bool TowerPlacementValid;
    private bool SelectingTower;
    private Tower SelectedTower;

    // Game state variables
    public bool AttackPhase;
    public bool BuildPhase;

    // Game States
    private bool Paused => ((GameScene)ParentScene).Paused;
    private bool DebugDraw => ((GameScene)ParentScene).DebugDraw;
    private Player Player => ((GameScene)ParentScene).Player;

    public Battlefield(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
    }

    protected override void InitializeSelf()
    {
        SpawnedCreepList = new List<Creep> { };
        PlacedTowersList = new();

        TowerSprite = ((GameScene)ParentScene).SpriteDictionary["TowerSprite"];
        SpawnerQueue = new Queue<CreepSpawner>();

        // hook
        ((GameScene)ParentScene).Battlefield = this;

        AttackPhase = false;
        BuildPhase = true;
    }

    protected override void LoadContentSelf()
    {
        PlatformTilemap = Tilemap.FromFile(Core.Content, "images/tilemap-definition.xml");
        PlatformTilemap.Scale = Props["TilemapScale"];

        // Load the path from the XML file
        BattlePath = LinkedPath.FromFile(
            Core.Content,
            "paths/beginner-map-std.xml",
            MapBounds.Location.ToVector2(),
            1f
        );
        BattlePath.LoadSprites(Atlas);
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        if (((GameScene)ParentScene).Won || ((GameScene)ParentScene).Lost)
        {
            return;
        }

        // Toggle tower placement mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Q))
        {
            StartPlacingTower(TowerType.Light, level: 1);
        }

        // Toggle tower placement mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.W))
        {
            StartPlacingTower(TowerType.Heavy, level: 1);
        }

        // Spawn a creep
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.A))
        {
            SpawnCreep();
        }

        // Enter tower selection mode
        if (Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left))
        {
            Tower selectedTower = SelectTower();
            if (selectedTower != null)
            {
                ClearStates();
                SelectingTower = true;
            }
            else
                SelectingTower = false;
        }

        // Escape from tower placement and tower selection
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            ClearStates();
        }

        // Reset scene
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Reset();
        }
        
        // Move creeps
        UpdateCreep(gameTime);

        // Handle tower placement validity during tower placement mode
        UpdatePlacingTower(gameTime);

        // Towers attack and reload
        UpdateTowers(gameTime);

        // Creeps die and are removed
        UpdateCreepList(gameTime);

        // Spawn creeps from the spawner queue
        UpdateSpawnerQueue(gameTime);

        UpdateBattleState(gameTime);
    }

    public bool StartPlacingTower(TowerType towerType, int level)
    {
        if (!BuildPhase)
            return false;

        ClearStates();
        PlacingTower = true;
        PlacingTowerCard = null;
        PlacingTowerType = towerType;
        PlacingTowerLevel = level;
        return true;
    }

    public bool StartPlacingTower(Card card)
    {
        if (!BuildPhase)
            return false;

        ClearStates();
        PlacingTower = true;
        PlacingTowerCard = card;
        PlacingTowerType = card.TowerType;
        PlacingTowerLevel = card.Level;
        return true;
    }

    private void ClearStates()
    {
        if (PlacingTower)
        {
            // Put it back in the inventory
            if (PlacingTowerCard != null)
            {
                Player.Inventory.AddCard(PlacingTowerCard);
            }
            PlacingTowerCard = null;
        }
        PlacingTower = false;
        SelectingTower = false;
    }

    private void Reset()
    {
        // Clear the placed towers list
        PlacedTowersList = new List<Tower>();
        ClearStates();
    }

    private void SpawnCreep()
    {
        // Todo: Make a CreepManager class that handles all creep logic
        AnimatedSprite CreepSprite = Atlas.CreateAnimatedSprite("torch-red-animation");
        CreepSprite.CenterOrigin();
        CreepSprite.Scale = SpriteScale;
        SpawnedCreepList.Add(
            new Creep(
                BattlePath,
                CreepSpeed,
                CreepSprite
            )
        );
    }

    public Tower SelectTower()
    {
        // Check collision between current mouse position and each placed tower
        Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
        foreach (Tower tower in PlacedTowersList)
        {
            if (tower.MouseCollision(mousePos))
            {
                SelectedTower = tower;
                return tower;
            }
        }
        return null;
    }

    private void UpdateCreep(GameTime gameTime)
    {
        if (!Paused)
        {
            // Update the creep
            foreach (var creep in SpawnedCreepList)
                creep.Update(gameTime);
        }
    }

    private void UpdatePlacingTower(GameTime gameTime)
    {
        if (PlacingTower)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
            Hitbox TowerBox = new Hitbox(
                mousePos, (int)(TowerSprite.Width * 0.5f)
            );
            TowerPlacementValid = !BattlePath.HasCollided(TowerBox) && MapBounds.Contains(TowerBox.rectangleBox);
            if (TowerPlacementValid)
            {
                foreach (Tower tower in PlacedTowersList)
                {
                    if (tower.HasCollided(TowerBox))
                    {
                        TowerPlacementValid = false;
                        break;
                    }
                }
            }

            if (Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left) && TowerPlacementValid)
            {
                PlacedTowersList.Add(TowerFactory.CreateTower(mousePos, PlacingTowerType, PlacingTowerLevel));
                PlacingTower = false;
            }
        }
    }

    private void UpdateTowers(GameTime gameTime)
    {
        if (!Paused)
        {
            foreach (var tower in PlacedTowersList)
            {
                tower.Update(gameTime);
                tower.Attack(SpawnedCreepList);
            }
        }
    }

    private void UpdateCreepList(GameTime gameTime)
    {
        List<Creep> expiredCreeps = new();
        foreach (var creep in SpawnedCreepList)
        {
            if (creep.Expired)
                expiredCreeps.Add(creep);

            if (creep.DamageToDeal > 0)
            {
                Player.Health -= creep.DamageToDeal;
                creep.DamageToDeal = 0;
            }
        }

        foreach (var creep in expiredCreeps)
        {
            Player.Gold += creep.Bounty;
            SpawnedCreepList.Remove(creep);
        }
    }

    private void UpdateSpawnerQueue(GameTime gameTime)
    {
        if (SpawnerQueue != null && SpawnerQueue.Count > 0)
        {
            CreepSpawner spawner = SpawnerQueue.Peek();
            spawner.TimeSinceLastSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawner.TimeSinceLastSpawn >= spawner.SpawnInterval)
            {
                if (spawner.Count <= 0)
                {
                    SpawnerQueue.Dequeue();
                    return;
                }
                SpawnedCreepList.Add(spawner.Creep);
                spawner.Count -= 1;
                spawner.TimeSinceLastSpawn = 0f;
            }
        }
    }

    /// <summary>
    /// Check for win/loss conditions and update game state accordingly.
    /// </summary>
    /// <param name="gameTime"></param>
    private void UpdateBattleState(GameTime gameTime)
    {
        if (AttackPhase)
        {
            if (Player.Health <= 0)
            {
                ((GameScene)ParentScene).Lose();
            }

            if (SpawnedCreepList.Count == 0 && SpawnerQueue.Count == 0 && CurrentWave > 0)
            {
                if (CurrentWave >= 5)
                {
                    ((GameScene)ParentScene).Win();
                }

                AttackPhase = false;
                BuildPhase = true;
                // Reset the shop
                ((GameScene)ParentScene).Shop.GenerateCards(Player.Level);
                // Earn end of round rewards
                // Player.Gold += CurrentWave * 2;
                // Level up player
                Player.Level += 1;
            }
        }
        else if (BuildPhase)
        {
            // Transition to AttackPhase if player presses space
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
            {
                StartNextWave();
            }
        }
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        DrawPlatform();
        DrawPath();
        DrawPlacedTowers();
        DrawPlacingTower();
        DrawSelectedTower();

        if (DebugDraw)
        {
            Core.SpriteBatch.Begin();
            Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, MapBounds, 1);
            Core.SpriteBatch.End();
        }
    }

    private void DrawPlatform()
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        PlatformTilemap.Draw(Core.SpriteBatch, MapBounds.Location.ToVector2());
        Core.SpriteBatch.End();
    }

    private void DrawPath()
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        BattlePath.Draw(Core.SpriteBatch, ((GameScene)ParentScene).WhitePixel);

        foreach (var creep in SpawnedCreepList)
            creep.Draw(Core.SpriteBatch, ((GameScene)ParentScene).WhitePixel, ((GameScene)ParentScene).DebugDraw);

        Core.SpriteBatch.End();
    }
    private void DrawPlacedTowers()
    {
        foreach (var tower in PlacedTowersList)
        {
            Color drawColor;
            if (SelectingTower && tower == SelectedTower)
                drawColor = Color.Cyan;
            else
                drawColor = Color.White;
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            tower.Draw(Core.SpriteBatch, drawColor);
            Core.SpriteBatch.End();
        }
    }
    private void DrawPlacingTower()
    {
        if (PlacingTower)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();

            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Color SemiTransparentGreen = new Color(0, 255, 0, 128);
            Color SemiTransparentRed = new Color(255, 0, 0, 128);

            if (TowerPlacementValid)
                TowerSprite.Draw(Core.SpriteBatch, mousePos, SemiTransparentGreen, 0f);
            else
                TowerSprite.Draw(Core.SpriteBatch, mousePos, SemiTransparentRed, 0f);

            Core.SpriteBatch.End();
            ((GameScene)ParentScene).DrawCircleIndicator(
                circleRadiusPx: Player.TowerInfo.GetTowerStat(PlacingTowerType, PlacingTowerLevel).Range * TDConstants.PixelsPerMeter
            );
        }
    }
    private void DrawSelectedTower()
    {
        if (SelectingTower)
        {
            ((GameScene)ParentScene).DrawCircleIndicator(SelectedTower.Position, SelectedTower.Range);
        }
    }

    public void StartNextWave()
    {
        if (CurrentWave >= 5 || AttackPhase)
            return;

        CurrentWave += 1;

        int SmallCreepToSpawn = CurrentWave switch
        {
            1 => 3,
            2 => 10,
            3 => 3,
            4 => 0,
            5 => 0,
            _ => 0
        };

        int MediumCreepToSpawn = CurrentWave switch
        {
            1 => 0,
            2 => 0,
            3 => 7,
            4 => 3,
            5 => 0,
            _ => 0
        };

        int BigCreepToSpawn = CurrentWave switch
        {
            1 => 0,
            2 => 0,
            3 => 0,
            4 => 7,
            5 => 0,
            _ => 0
        };

        int BossCreepToSpawn = CurrentWave switch
        {
            1 => 0,
            2 => 0,
            3 => 0,
            4 => 0,
            5 => 1,
            _ => 0
        };

        for (int i = 0; i < SmallCreepToSpawn; i++)
        {
            AnimatedSprite CreepSprite = Atlas.CreateAnimatedSprite("torch-red-animation");
            CreepSprite.CenterOrigin();
            CreepSprite.Scale = SpriteScale;
            SpawnerQueue.Enqueue(new CreepSpawner(
                new Creep(
                    BattlePath,
                    CreepSpeed,
                    CreepSprite
                ),
                spawnInterval: 1f / 6f
            ));
        }
        for (int i = 0; i < MediumCreepToSpawn; i++)
        {
            AnimatedSprite CreepSprite = Atlas.CreateAnimatedSprite("torch-blue-animation");
            CreepSprite.CenterOrigin();
            CreepSprite.Scale = SpriteScale;
            SpawnerQueue.Enqueue(new CreepSpawner(
                new MediumCreep(
                    BattlePath,
                    CreepSpeed,
                    CreepSprite
                ),
                spawnInterval: 1f / 6f
            ));
        }
        for (int i = 0; i < BigCreepToSpawn; i++)
        {
            AnimatedSprite CreepSprite = Atlas.CreateAnimatedSprite("torch-yellow-animation");
            CreepSprite.CenterOrigin();
            CreepSprite.Scale = SpriteScale;
            SpawnerQueue.Enqueue(new CreepSpawner(
                new BigCreep(
                    BattlePath,
                    CreepSpeed,
                    CreepSprite
                ),
                spawnInterval: 1f / 6f
            ));
        }
        for (int i = 0; i < BossCreepToSpawn; i++)
        {
            AnimatedSprite CreepSprite = Atlas.CreateAnimatedSprite("torch-green-animation");
            CreepSprite.CenterOrigin();
            CreepSprite.Scale = SpriteScale;
            SpawnerQueue.Enqueue(new CreepSpawner(
                new BossCreep(
                    BattlePath,
                    CreepSpeed,
                    CreepSprite
                ),
                spawnInterval: 1f / 6f
            ));
        }

        AttackPhase = true;
        BuildPhase = false;
    }
}

public class CreepSpawner
{
    public Creep Creep;
    public float SpawnInterval;
    public float TimeSinceLastSpawn = 0f;
    public int Count = 1;

    public CreepSpawner(Creep creep, float spawnInterval)
    {
        Creep = creep;
        SpawnInterval = spawnInterval;
    }
}