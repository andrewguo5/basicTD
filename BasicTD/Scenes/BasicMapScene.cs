using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using System.Collections.Generic;
using MonoGameLibrary.Paths;
using System;

namespace BasicTD.Scenes;

public class BasicMapScene : BaseScene
{
    // Sprites
    private Sprite StartMarker;
    private Sprite EndMarker;
    private Sprite ControlPointMarker;
    private AnimatedSprite Torch;
    private Sprite Tower;
    private List<Sprite> SpriteManager;
    private Vector2 SpriteScale = new Vector2(3f, 3f);

    // Path
    // private LinkedPath Path;

    // Creeps
    private Creep TorchCreep;
    private float CreepSpeed = 400f; // pixels per second

    // Towers
    private List<Vector2> Towers;
    private bool TowerPlacementValid;
    
    public BasicMapScene() : base()
    {

    }

    public override void Initialize()
    {
        // NOTE: Above this line, content has not been loaded yet.
        base.Initialize();
        // NOTE: Content has been loaded after this line

        // Collect all of the sprites into a list for easy management
        SpriteManager = new List<Sprite>()
        {
            StartMarker,
            EndMarker,
            ControlPointMarker,
            Torch,
            Tower
        };

        // Scale and center the sprites
        foreach (var sprite in SpriteManager)
        {
            sprite.CenterOrigin();
            sprite.Scale = SpriteScale;
        }

        // Load the path from the XML file
        Path = LinkedPath.FromFile(Core.Content, "paths/beginner-map.xml");
        Path.LoadSprites(Atlas);

        // Create the creeps
        TorchCreep = new Creep(Path, CreepSpeed, Torch);

        // Towers
        Towers = new List<Vector2>();

        // Scene management
        NextScene = new LineScene();
    }

    public override void Reset()
    {
        // Initialize();
        Towers = new List<Vector2>();
        PlacingTower = false;
    }

    public override void LoadContent()
    {
        base.LoadContent();

        // Sprites for a starting point, an ending point, and a blue torch
        StartMarker = Atlas.CreateSprite("lever-blue");
        EndMarker = Atlas.CreateSprite("lever-red");
        ControlPointMarker = Atlas.CreateSprite("lever-yellow");
        Torch = Atlas.CreateAnimatedSprite("torch-blue-animation");
        Tower = Atlas.CreateSprite("lever-green");
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        if (!Paused)
        {
            // Update the creep
            TorchCreep.Update(gameTime);
        }
        
        if (PlacingTower) 
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
            Hitbox TowerBox = new Hitbox(
                mousePos, (int)(Tower.Width * 0.5f)
            );
            TowerPlacementValid = !Path.HasCollided(TowerBox);
            
            if (Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left) && TowerPlacementValid)
            {
                Towers.Add(mousePos);
                PlacingTower = false;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);

        DrawPath(gameTime);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: Grayscale);

        Vector2 StartingPosition = Path.StartingPoint;
        Vector2 EndingPosition = Path.EndingPoint;
        List<Vector2> ControlPoints = Path.ControlPoints;

        StartMarker.Draw(Core.SpriteBatch, StartingPosition);
        EndMarker.Draw(Core.SpriteBatch, EndingPosition);
        foreach (var controlPoint in ControlPoints)
        {
            ControlPointMarker.Draw(Core.SpriteBatch, controlPoint);
        }

        TorchCreep.Draw(Core.SpriteBatch, WhitePixel, DebugDraw);

        Core.SpriteBatch.End();
        
        foreach (var towerPosition in Towers)
        {
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Tower.Draw(Core.SpriteBatch, towerPosition);
            Core.SpriteBatch.End();
        }

        if (PlacingTower)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();

            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Color SemiTransparentGreen = new Color(0, 255, 0, 128);
            Color SemiTransparentRed = new Color(255, 0, 0, 128);

            if (TowerPlacementValid)
                Tower.Draw(Core.SpriteBatch, mousePos, SemiTransparentGreen, 0f);
            else
                Tower.Draw(Core.SpriteBatch, mousePos, SemiTransparentRed, 0f);
            
            Core.SpriteBatch.End();
            DrawCircleIndicator();
        }
        
    }
}