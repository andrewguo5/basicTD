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

public class BasicMapScene : Scene
{
    // Sprites
    private TextureAtlas Atlas;
    private Sprite StartMarker;
    private Sprite EndMarker;
    private Sprite ControlPointMarker;
    private AnimatedSprite Torch;
    private List<Sprite> SpriteManager;
    private Vector2 SpriteScale = new Vector2(3f, 3f);
    private Texture2D WhitePixel;

    // Path
    private LinkedPath Path;

    // Creeps
    private Creep TorchCreep;
    private float CreepSpeed = 400f; // pixels per second

    // Debug Mode
    private bool DebugDraw = false;

    // Scene Manager
    public Scene NextScene { get; set; }
    
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
            Torch
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

        // Scene management
        NextScene = new LineScene();
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        Atlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");

        // Sprites for a starting point, an ending point, and a blue torch
        StartMarker = Atlas.CreateSprite("lever-blue");
        EndMarker = Atlas.CreateSprite("lever-red");
        ControlPointMarker = Atlas.CreateSprite("lever-yellow");
        Torch = Atlas.CreateAnimatedSprite("torch-blue-animation");

        // Create a white pixel texture for debug drawing
        WhitePixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        WhitePixel.SetData(new[] { Color.White });
    }
    public override void UnloadContent()
    {
        base.UnloadContent();
        
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

        // Update the creep
        TorchCreep.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        foreach (var _path in Path.Paths)
        {
            _path.Draw(Core.SpriteBatch, WhitePixel);
        }
        Core.SpriteBatch.End();

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

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
    }
}