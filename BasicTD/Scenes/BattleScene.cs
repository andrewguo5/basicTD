using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Paths;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using BasicTD.Towers;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Input;
using System.Collections.Generic;

using MonoGameLibrary.Collision;
using System.Xml.Serialization;

namespace BasicTD.Scenes;

public abstract class BattleScene : Scene
{
    // Map Bounds
    public Rectangle MapBounds { get; set; }

    // Common Sprites
    protected Sprite StartMarker;
    protected Sprite EndMarker;
    protected Sprite ControlPointMarker;

    protected AnimatedSprite TorchSprite;
    protected Sprite TowerSprite;
    protected List<Sprite> SpriteManager;
    protected Vector2 SpriteScale = new Vector2(3f, 3f);

    // Path
    public Path Path { get; set; }

    // Creeps
    protected Creep TorchCreep;
    protected float CreepSpeed = 250f; // pixels per second
    protected List<Creep> CreepList;

    // Towers
    protected List<Tower> Towers;
    protected bool TowerPlacementValid;
    protected Tower SelectedTower;

    // Toggleable Modes
    protected bool DebugDraw = false;
    protected bool Paused = false;
    protected bool Grayed = false;
    protected bool PlacingTower = false;
    protected bool SelectingTower = false;

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

    public BattleScene() : base()
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

        // Collect all of the sprites into a list for easy management
        SpriteManager = new List<Sprite>()
        {
            StartMarker,
            EndMarker,
            ControlPointMarker,
            TorchSprite,
            TowerSprite,
        };

        // Scale and center the sprites
        foreach (var sprite in SpriteManager)
        {
            sprite.CenterOrigin();
            sprite.Scale = SpriteScale;
        }

        // Delegate to child class to initialize the path
        InitializePath();

        // Create the creeps
        TorchCreep = new Creep(Path, CreepSpeed, TorchSprite);
        CreepList = new List<Creep> { TorchCreep };

        // Initialize the towers list
        Towers = new List<Tower>();
    }

    public abstract void InitializePath();

    public virtual void Reset()
    {
        // Clear the placed towers list
        Towers = new List<Tower>();
        PlacingTower = false;
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

        // Load common sprites
        StartMarker = Atlas.CreateSprite("lever-blue");
        EndMarker = Atlas.CreateSprite("lever-red");
        ControlPointMarker = Atlas.CreateSprite("lever-yellow");
        TorchSprite = Atlas.CreateAnimatedSprite("torch-blue-animation");
        TowerSprite = Atlas.CreateSprite("lever-green");
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

        if (Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left))
        {
            Tower selectedTower = SelectTower(gameTime);
            if (selectedTower != null)
                SelectingTower = true;
            else
                SelectingTower = false;
        }

        // Escape from tower placement and tower selection
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            PlacingTower = false;
            SelectingTower = false;
        }

        // Scene transition
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            Core.ChangeScene(NextScene);
        }

        // Reset scene
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Reset();
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

    public Tower SelectTower(GameTime gameTime)
    {
        // Check collision between current mouse position and each placed tower
        Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
        foreach (Tower tower in Towers)
        {
            if (tower.MouseCollision(mousePos))
            {
                SelectedTower = tower;
                return tower;
            }
        }
        return null;
    }

    public void UpdateCreep(GameTime gameTime)
    {
        if (!Paused)
        {
            // Update the creep
            TorchCreep.Update(gameTime);
        }
    }

    public void UpdatePlacingTower(GameTime gameTime)
    {
        if (PlacingTower)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
            Hitbox TowerBox = new Hitbox(
                mousePos, (int)(TowerSprite.Width * 0.5f)
            );
            TowerPlacementValid = !Path.HasCollided(TowerBox);
            if (TowerPlacementValid)
            {
                foreach (Tower tower in Towers)
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
                Towers.Add(new TestTower(mousePos, TowerSprite));
                PlacingTower = false;
            }
        }
    }

    public void UpdateTowers(GameTime gameTime)
    {
        foreach (var tower in Towers)
        {
            tower.Update(gameTime);
            List<Creep> creepsInRange = tower.CreepsInRange(CreepList);
            foreach (var creep in creepsInRange)
            {
                {
                    tower.Attack(creep);
                }
            }
        }
    }

    public Vector2 NormalizePosition(Vector2 location, Point bounds, Point offset)
    {
        return new Vector2(
            (float)(location.X - offset.X) / bounds.X,
            (float)(location.Y - offset.Y) / bounds.Y
        );
    }

    public void DrawCircleIndicator(Vector2 location, float circleRadiusPx = 200f)
    {
        float circleRadius = circleRadiusPx / MapBounds.Height;
        CircleIndicator.Parameters["circleRadius"].SetValue(circleRadius);
        // Vector2 normalizedMousePos = Core.Input.Mouse.GetNormalizedPosition(
        //     new Point(MapBounds.Width, MapBounds.Height), MapBounds.Location
        // );
        Vector2 normalizedMousePos = NormalizePosition(
            location, new Point(MapBounds.Width, MapBounds.Height), MapBounds.Location
        );
        CircleIndicator.Parameters["mousePos"].SetValue(normalizedMousePos);
        // Casts are necessary here to avoid integer division
        CircleIndicator.Parameters["aspectRatio"]?.SetValue((float)MapBounds.Width / (float)MapBounds.Height);

        // Create a matrix to convert from screen coordinates to normalized device coordinates
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.CreateOrthographicOffCenter(0, MapBounds.Width, MapBounds.Height, 0, 0, 1);
        CircleIndicator.Parameters["view_projection"]?.SetValue(view * projection);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: CircleIndicator);
        Core.SpriteBatch.Draw(WhitePixel, MapBounds, new Color(0, 0, 0, 128));
        Core.SpriteBatch.End();
    }

    public void DrawCircleIndicator(float circleRadiusPx = 200f)
    {
        DrawCircleIndicator(Core.Input.Mouse.Position.ToVector2());
    }

    public void DrawPath(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Path.Draw(Core.SpriteBatch, WhitePixel);
        Core.SpriteBatch.End();
    }

    public void DrawPlacedTowers(GameTime gameTime)
    {
        foreach (var tower in Towers)
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

    public void DrawPlacingTower(GameTime gameTime)
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
            DrawCircleIndicator();
        }
    }

    public void DrawSelectedTower(GameTime gameTime)
    {
        if (SelectingTower)
        {
            float circleRadius = SelectedTower.Range / MapBounds.Height;
            CircleIndicator.Parameters["circleRadius"].SetValue(circleRadius);
            // Vector2 normalizedMousePos = Core.Input.Mouse.GetNormalizedPosition(
            //     new Point(MapBounds.Width, MapBounds.Height), MapBounds.Location
            // );
            Vector2 normalizedMousePos = NormalizePosition(
                SelectedTower.Position, new Point(MapBounds.Width, MapBounds.Height), MapBounds.Location
            );
            CircleIndicator.Parameters["mousePos"].SetValue(normalizedMousePos);
            // Casts are necessary here to avoid integer division
            CircleIndicator.Parameters["aspectRatio"]?.SetValue((float)MapBounds.Width / (float)MapBounds.Height);

            // Create a matrix to convert from screen coordinates to normalized device coordinates
            Matrix view = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, MapBounds.Width, MapBounds.Height, 0, 0, 1);
            CircleIndicator.Parameters["view_projection"]?.SetValue(view * projection);

            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: CircleIndicator);
            Core.SpriteBatch.Draw(WhitePixel, MapBounds, new Color(0, 0, 0, 128));
            Core.SpriteBatch.End();
        }
    }
}