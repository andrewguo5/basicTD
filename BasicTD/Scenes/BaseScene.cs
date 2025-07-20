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

public class BaseScene : Scene
{
    // Map Bounds
    public Rectangle MapBounds { get; set; }

    // Toggleable Modes
    protected bool DebugDraw = false;
    protected bool Paused = false;
    protected bool Grayed = false;
    protected bool PlacingTower = false;

    // Grayscale Mode
    protected float Saturation = 1f;

    // Scene Manager
    public Scene NextScene { get; set; }

    // Effects
    protected Effect Grayscale;
    protected Effect CircleIndicator;

    // Sprite Atlas
    protected TextureAtlas Atlas;
    protected Texture2D WhitePixel;

    public BaseScene() : base()
    {

    }

    public override void Initialize()
    {
        // NOTE: Above this line, content has not been loaded yet.
        base.Initialize();
        // NOTE: Content has been loaded after this line

        // Set the map bounds to the entire screen by default
        MapBounds = new Rectangle(
            0,
            0,
            Core.GraphicsDevice.PresentationParameters.BackBufferWidth,
            Core.GraphicsDevice.PresentationParameters.BackBufferHeight
        );
    }

    public override void LoadContent()
    {
        base.LoadContent();

        // Create the texture atlas from the XML configuration file
        Atlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");

        // Load the grayscale effect
        Grayscale = Core.Content.Load<Effect>("effects/grayscaleEffect");

        // Load the circle indicator effect
        CircleIndicator = Core.Content.Load<Effect>("effects/circleIndicator");

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
        // Toggle debug mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.D))
            DebugDraw = !DebugDraw;

        // Toggle pause mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.P))
            Paused = !Paused;

        // Toggle grayscale mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.G))
            Grayed = !Grayed;

        // Toggle tower placement mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Q))
            PlacingTower = !PlacingTower;

        // Scene transition
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            Core.ChangeScene(NextScene);
        }

        // Grayscale logic
        if (Grayed)
        {
            Saturation = MathHelper.Clamp(Saturation - 0.03f, 0f, 1f);
        }
        else
        {
            Saturation = MathHelper.Clamp(Saturation + 0.03f, 0f, 1f);
        }
        Grayscale.Parameters["Saturation"].SetValue(Saturation);
    }

    protected void DrawCircleIndicator(float circleRadius = 0.2f)
    {
        CircleIndicator.Parameters["circleRadius"].SetValue(circleRadius);
        Vector2 normalizedMousePos = Core.Input.Mouse.GetNormalizedPosition(
            new Point(MapBounds.Width, MapBounds.Height), MapBounds.Location
        );
        CircleIndicator.Parameters["mousePos"].SetValue(normalizedMousePos);
        // Casts are necessary here to avoid integer division
        CircleIndicator.Parameters["aspectRatio"]?.SetValue((float)MapBounds.Width / (float)MapBounds.Height);

        // Create a matrix to convert from screen coordinates to normalized device coordinates
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.CreateOrthographicOffCenter(0, MapBounds.Width, MapBounds.Height, 0, 0, 1);
        CircleIndicator.Parameters["view_projection"].SetValue(view * projection);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: CircleIndicator);
        Core.SpriteBatch.Draw(WhitePixel, MapBounds, new Color (0, 0, 0, 128));
        Core.SpriteBatch.End();
    }
}