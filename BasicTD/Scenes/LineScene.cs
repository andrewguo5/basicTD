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

public class LineScene : Scene
{
    // Sprites
    private Sprite StartMarker;
    private Sprite EndMarker;
    private AnimatedSprite Torch;
    private List<Sprite> SpriteManager;
    private Vector2 SpriteScale = new Vector2(3f, 3f);
    private Texture2D WhitePixel;

    // Parameters
    private Vector2 StartingPosition { get; set; }
    private Vector2 EndingPosition { get; set; }

    // Path
    private LinePath Path;

    // Creeps
    private Creep TorchCreep;
    private float CreepSpeed = 400f; // pixels per second

    // Debug Mode
    private bool DebugDraw = false;

    // Scene Manager
    public Scene NextScene { get; set; }

    public LineScene() : base()
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
            Torch
        };

        // Scale and center the sprites
        foreach (var sprite in SpriteManager)
        {
            sprite.CenterOrigin();
            sprite.Scale = SpriteScale;
        }

        // Define the starting and ending positions
        StartingPosition = Coordinates.NormalizedToScreen(new Vector2(0.2f, 0.5f));
        EndingPosition = Coordinates.NormalizedToScreen(new Vector2(0.8f, 0.5f));

        // Create the path
        Path = new LinePath(StartingPosition, EndingPosition);

        // Create the creep
        TorchCreep = new Creep(Path, CreepSpeed, Torch);

        // Scene management
        NextScene = new MultiArcScene();
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");

        // Sprites for a starting point, an ending point, and a blue torch
        StartMarker = atlas.CreateSprite("lever-blue");
        EndMarker = atlas.CreateSprite("lever-red");
        Torch = atlas.CreateAnimatedSprite("torch-blue-animation");

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

        StartMarker.Draw(Core.SpriteBatch, StartingPosition);
        EndMarker.Draw(Core.SpriteBatch, EndingPosition);
        TorchCreep.Draw(Core.SpriteBatch, WhitePixel, DebugDraw);

        Core.SpriteBatch.End();
    }
}