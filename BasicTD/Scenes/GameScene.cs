using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Input;
using BasicTD.Towers;
using System.Collections.Generic;

namespace BasicTD.Scenes;

public class GameScene : Scene
{
    // Bounds
    private Rectangle MapBounds { get; set; }
    private Rectangle ScreenBounds = new(
        0,
        0,
        Core.GraphicsDevice.PresentationParameters.BackBufferWidth,
        Core.GraphicsDevice.PresentationParameters.BackBufferHeight
    );

    // Sprite Managers
    private List<Sprite> SpriteManager;
    private List<Sprite> CardSpriteManager;
    public Dictionary<string, Sprite> SpriteDictionary;
    private Vector2 SpriteScale = new(2.5f, 2.5f);
    private Vector2 CardSpriteScale = new(2f, 2f);

    // Atlas
    private TextureAtlas Atlas;
    private TextureAtlas CardAtlas;

    // Effects
    public Effect CircleIndicator;

    // Animations
    private Dictionary<string, AnimatedSprite> AnimationDictionary;

    // White pixel
    public Texture2D WhitePixel;

    // Main component
    public GComponent Main { get; set; }

    // States
    public bool DebugDraw { get; set; } = false;
    public bool Paused = false;
    public bool Grayed = false;
    public bool PlacingTower = false;
    public bool SelectingTower = false;

    // Scene Manager
    public Scene NextScene { get; set; }

    // Font
    public SpriteFont GameFont;

    // Creeps
    public List<Creep> CreepList;

    // Towers
    public List<Tower> Towers;
    public bool TowerPlacementValid;
    public Tower SelectedTower;
    public TowerType PlacingTowerType;
    public TowerFactory TowerFactory;

    // Toggleable Modes

    public GameScene() : base()
    {

    }

    public override void Initialize()
    {
        NextScene = new BasicMapScene();

        // 1. Pre-load initialization
        base.Initialize();

        // 3. Post-load initialization
        SpriteManager =
        [
            SpriteDictionary["StartMarker"],
            SpriteDictionary["EndMarker"],
            SpriteDictionary["ControlPointMarker"],
            SpriteDictionary["TorchSprite"],
            SpriteDictionary["TowerSprite"],
            SpriteDictionary["GoldSprite"],
            SpriteDictionary["HeartSprite"],
            SpriteDictionary["SkullSprite"],
            SpriteDictionary["GearSprite"],
        ];
        CardSpriteManager =
        [
            SpriteDictionary["CardCommonSprite"],
            SpriteDictionary["CardUncommonSprite"],
            SpriteDictionary["CardRateSprite"],
            SpriteDictionary["CardEpicSprite"],
            SpriteDictionary["CardLegendarySprite"],
            SpriteDictionary["CardNullSprite"],
            SpriteDictionary["TowerBaseSprite"],
            SpriteDictionary["EmblemShieldSprite"],
            SpriteDictionary["EmblemBulletSprite"],
            SpriteDictionary["EmblemGemSprite"],
            SpriteDictionary["SymbolPulseSprite"],
            SpriteDictionary["SymbolShockwaveSprite"],
            SpriteDictionary["SymbolLoopSprite"],
            SpriteDictionary["SymbolSquareSprite"],
            SpriteDictionary["SymbolOvalSprite"],
            SpriteDictionary["SymbolSpareSprite"]
        ];

        foreach (Sprite sprite in SpriteManager)
        {
            sprite.CenterOrigin();
            sprite.Scale = SpriteScale;
        }

        foreach (Sprite sprite in CardSpriteManager)
        {
            sprite.Scale = CardSpriteScale;
        }

        MapBounds = new Rectangle(
            240, 100,
            720, 420
        );

        // After pre-loading the scene, we can load the main component.
        Main = new Components.Main(this, ScreenBounds, new Dictionary<string, dynamic>
        {
            { "VerticalOffset", 30 },
            { "TextPadding", 10 },
            { "SideBuffer", 30 },
            { "HTileOffset", 24 },
            { "VTileOffset", 16 },
            { "MapBounds", MapBounds },
            { "Atlas", Atlas },
            { "CardAtlas", CardAtlas },
            { "GameFont", GameFont },
            { "TilemapScale", new Vector2(60f / 16f, 60f / 16f) },
            { "SpriteScale", SpriteScale }
        });
        Main.LoadContent();
        Main.Initialize();
    }

    public override void LoadContent()
    {
        // 2. Load
        base.LoadContent();

        // Load Atlas
        Atlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");
        CardAtlas = TextureAtlas.FromFile(Core.Content, "images/card-shop-atlas.xml");

        // Load Fonts
        GameFont = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load Sprites and Animations
        LoadSprites();

        // Create a white pixel texture for debug drawing
        WhitePixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        WhitePixel.SetData(new[] { Color.White });

        // Load effect
        CircleIndicator = Core.Content.Load<Effect>("effects/circleIndicator");
    }

    private void LoadSprites()
    {
        SpriteDictionary = new()
        {
            // Common Sprites
            { "StartMarker", Atlas.CreateSprite("lever-blue") },
            { "EndMarker", Atlas.CreateSprite("lever-red") },
            { "ControlPointMarker", Atlas.CreateSprite("lever-yellow") },
            { "TowerSprite", Atlas.CreateSprite("minibase") },
            { "GoldSprite", Atlas.CreateSprite("gold") },
            { "HeartSprite", Atlas.CreateSprite("heart") },
            { "SkullSprite", Atlas.CreateSprite("skull") },
            { "GearSprite", Atlas.CreateSprite("gear") },
            { "TorchSprite", Atlas.CreateAnimatedSprite("torch-red-animation") },
            // Card Sprites
            { "CardCommonSprite", CardAtlas.CreateSprite("card-common") },
            { "CardUncommonSprite", CardAtlas.CreateSprite("card-uncommon") },
            { "CardRateSprite", CardAtlas.CreateSprite("card-rare") },
            { "CardEpicSprite", CardAtlas.CreateSprite("card-epic") },
            { "CardLegendarySprite", CardAtlas.CreateSprite("card-legendary") },
            { "CardNullSprite", CardAtlas.CreateSprite("card-null") },
            { "TowerBaseSprite", CardAtlas.CreateSprite("tower-base") },
            // Emblem Sprites
            { "EmblemShieldSprite", CardAtlas.CreateSprite("emblem-shield") },
            { "EmblemBulletSprite", CardAtlas.CreateSprite("emblem-bullet") },
            { "EmblemGemSprite", CardAtlas.CreateSprite("emblem-gem") },
            // Symbol Sprites
            { "SymbolPulseSprite", CardAtlas.CreateSprite("symbol-pulse") },
            { "SymbolShockwaveSprite", CardAtlas.CreateSprite("symbol-shockwave") },
            { "SymbolLoopSprite", CardAtlas.CreateSprite("symbol-loop") },
            { "SymbolSquareSprite", CardAtlas.CreateSprite("symbol-square") },
            { "SymbolOvalSprite", CardAtlas.CreateSprite("symbol-oval") },
            { "SymbolSpareSprite", CardAtlas.CreateSprite("symbol-spare") },
        };

        AnimationDictionary = new()
        {
        };
    }

    private void LoadEffects()
    {

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

        // Scene transition
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            Core.ChangeScene(NextScene);
        }

        Main.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Main.Draw(gameTime);

        if (DebugDraw)
        {
            Core.SpriteBatch.Begin();
            DrawScaffoldingLines(Core.SpriteBatch);
            Core.SpriteBatch.End();
        }
    }
    private void DrawScaffoldingLines(SpriteBatch spriteBatch)
    {
        // Top UI boundary: Separates the info UI from the map
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, MapBounds.Top);

        // Top UI subsection: Separates the top bar from the relic bar
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, 60);

        // Bottom player UI boundary: Separates the map from the player UI
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, MapBounds.Bottom);

        // Buffer for player UI boundary
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, MapBounds.Bottom + 40);

        // Left and right map boundaries
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Left);
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Right);
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
        float circleRadius = circleRadiusPx / ScreenBounds.Height;
        CircleIndicator.Parameters["circleRadius"].SetValue(circleRadius);
        Vector2 normalizedMousePos = NormalizePosition(
            location, new Point(ScreenBounds.Width, ScreenBounds.Height), ScreenBounds.Location
        );
        CircleIndicator.Parameters["mousePos"].SetValue(normalizedMousePos);
        // Casts are necessary here to avoid integer division
        CircleIndicator.Parameters["aspectRatio"]?.SetValue((float)ScreenBounds.Width / (float)ScreenBounds.Height);

        // Create a matrix to convert from screen coordinates to normalized device coordinates
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.CreateOrthographicOffCenter(ScreenBounds.Left, ScreenBounds.Right, ScreenBounds.Bottom, ScreenBounds.Top, 0, 1);
        CircleIndicator.Parameters["view_projection"]?.SetValue(view * projection);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: CircleIndicator);
        Core.SpriteBatch.Draw(WhitePixel, ScreenBounds, new Color(0, 0, 0, 128));
        Core.SpriteBatch.End();
    }

    public void DrawCircleIndicator(float circleRadiusPx = 200f)
    {
        DrawCircleIndicator(Core.Input.Mouse.Position.ToVector2(), circleRadiusPx);
    }
}
