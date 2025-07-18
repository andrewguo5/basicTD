using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using System.Collections.Generic;
using MonoGameLibrary.Paths;

namespace BasicTD.Scenes;

public class MultiArcScene : Scene
{
    // Sprites
    private Sprite StartMarker;
    private Sprite EndMarker;
    private Sprite ControlPointMarker;
    private AnimatedSprite Torch;
    private List<Sprite> SpriteManager;
    private Vector2 SpriteScale = new Vector2(3f, 3f);
    private Texture2D WhitePixel;

    // Path
    private LinkedPath PathCollection;

    // Creeps
    private List<Creep> TorchCreepList;
    private float CreepSpeed = 400f; // pixels per second

    // Debug Mode
    private bool DebugDraw = false;

    // Scene Manager
    public Scene NextScene { get; set; }
    
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
            Torch
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
        }

        // Scene management
        NextScene = new LineScene();
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");

        // Sprites for a starting point, an ending point, and a blue torch
        StartMarker = atlas.CreateSprite("lever-blue");
        EndMarker = atlas.CreateSprite("lever-red");
        ControlPointMarker = atlas.CreateSprite("lever-yellow");
        Torch = atlas.CreateAnimatedSprite("torch-blue-animation");

        // Create a white pixel texture for debug drawing
        WhitePixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        WhitePixel.SetData(new[] { Color.White });
    }
    public override void UnloadContent()
    {
        base.UnloadContent();
        Core.SpriteBatch.Dispose();
        WhitePixel.Dispose();
    }


    public override void Update(GameTime gameTime)
    {
        // Common keyboard toggles (figure out a way to share this code)
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.D))
        {
            DebugDraw = !DebugDraw;
        }
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            Core.ChangeScene(NextScene);
        }

        // Update the creeps
        foreach (var TorchCreep in TorchCreepList)
        {
            TorchCreep.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        foreach (var path in PathCollection.Paths)
        {
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
    }
}