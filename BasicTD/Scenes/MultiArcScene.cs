using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Input;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using System.Collections.Generic;
using MonoGameLibrary.Paths;

namespace BasicTD.Scenes;

public class MultiArcScene : BaseScene
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
    private LinkedPath PathCollection;

    // Creeps
    private List<Creep> TorchCreepList;
    private float CreepSpeed = 400f; // pixels per second

    // Towers
    private List<Vector2> Towers;
    
    public MultiArcScene() : base()
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
        PathCollection = LinkedPath.FromFile(Core.Content, "paths/multi-path.xml");

        // Create the creeps
        TorchCreepList = new List<Creep>();
        foreach (var path in PathCollection.Paths)
        {
            TorchCreepList.Add(new Creep(path, CreepSpeed, Torch));
            path.LoadSprites(Atlas);
        }

        // Towers
        Towers = new List<Vector2>();

        // Scene management
        NextScene = new LinkedArcScene();
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
            // Update the creeps
            foreach (var TorchCreep in TorchCreepList)
            {
                TorchCreep.Update(gameTime);
            }
        }
        
        if (PlacingTower && Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left))
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
            Towers.Add(mousePos);
            PlacingTower = false;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: Grayscale);

        foreach (var path in PathCollection.Paths)
        {
            path.Draw(Core.SpriteBatch, WhitePixel);

            Vector2 StartingPosition = path.StartingPoint;
            Vector2 EndingPosition = path.EndingPoint;
            List<Vector2> ControlPoints = path.ControlPoints;

            StartMarker.Draw(Core.SpriteBatch, StartingPosition);
            EndMarker.Draw(Core.SpriteBatch, EndingPosition);
            foreach (var controlPoint in ControlPoints)
            {
                ControlPointMarker.Draw(Core.SpriteBatch, controlPoint);
            }
        }

        foreach (var TorchCreep in TorchCreepList)
        {
            TorchCreep.Draw(Core.SpriteBatch, WhitePixel, DebugDraw);
        }

        Core.SpriteBatch.End();
 
        if (PlacingTower)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();

            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Tower.Draw(Core.SpriteBatch, mousePos, new Color(0, 255, 0, 128), 0f);
            Core.SpriteBatch.End();
            DrawCircleIndicator();
        }
        
        foreach (var towerPosition in Towers)
        {
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Tower.Draw(Core.SpriteBatch, towerPosition);
            Core.SpriteBatch.End();
        }
    }
}