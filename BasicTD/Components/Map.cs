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

    // Component-specific Content
    private Tilemap PlatformTilemap;
    private Path BattlePath;
    private Sprite TowerSprite;
    private TowerFactory TowerFactory;

    // Component-specific data structures
    private List<Creep> SpawnedCreepList;
    private float CreepSpeed = 150f;
    private List<Tower> PlacedTowersList;

    // Tower placement variables
    private TowerType PlacingTowerType;
    private bool PlacingTower;
    private bool TowerPlacementValid;
    private bool SelectingTower;
    private Tower SelectedTower;

    // Game States
    private bool Paused => ((GameScene)ParentScene).Paused;
    private bool DebugDraw => ((GameScene)ParentScene).DebugDraw;

    public Battlefield(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
    }

    protected override void InitializeSelf()
    {
        SpawnedCreepList = new List<Creep> {
            new Creep(
                BattlePath,
                CreepSpeed,
                (AnimatedSprite)((GameScene)ParentScene).SpriteDictionary["TorchSprite"])
        };
        PlacedTowersList = new();

        TowerSprite = ((GameScene)ParentScene).SpriteDictionary["TowerSprite"];
        TowerFactory = new(Atlas, SpriteScale);
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
        // Spawn a creep
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.A))
        {
            SpawnCreep();
        }

        // Toggle tower placement mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Q))
        {
            ClearStates();
            PlacingTower = true;
            PlacingTowerType = TowerType.Light;
        }

        // Toggle tower placement mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.W))
        {
            ClearStates();
            PlacingTower = true;
            PlacingTowerType = TowerType.Heavy;
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
    }


    private void ClearStates()
    {
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
                PlacedTowersList.Add(TowerFactory.CreateTower(mousePos, PlacingTowerType));
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
        }

        foreach (var creep in expiredCreeps)
        {
            SpawnedCreepList.Remove(creep);
        }

        if (SpawnedCreepList.Count == 0)
        {
            // Add a new Creep
            SpawnCreep();
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
                circleRadiusPx: TowerStats.AllTowerStats[PlacingTowerType]["Range"] * TDConstants.PixelsPerMeter
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
}