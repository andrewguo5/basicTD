using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
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
    private Dictionary<string, Sprite> SpriteDictionary;
    private Vector2 SpriteScale = new(2.5f, 2.5f);
    private Vector2 CardSpriteScale = new(2f, 2f);

    // Atlas
    private TextureAtlas Atlas;
    private TextureAtlas CardAtlas;

    // Animations
    private Dictionary<string, AnimatedSprite> AnimationDictionary;

    // White pixel
    private Texture2D WhitePixel;

    // Main component
    public GComponent Main { get; set; }

    // States
    public bool DebugDraw { get; set; } = false;

    // Scene Manager
    public Scene NextScene { get; set; }

    // Font
    public SpriteFont GameFont;

    // Props
    private int VerticalOffset;
    private int TextPadding;
    private int SideBuffer;
    private int HTileOffset;
    private int VTileOffset;

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
        Main = new Components.Main(ScreenBounds, new Dictionary<string, dynamic>
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

        // // Draw left and right side banner boundaries
        // Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, SideBuffer);
        // Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Left - SideBuffer);

        // Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Right + SideBuffer);
        // Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, Core.GraphicsDevice.Viewport.Width - sideBannerBuffer);

        // // Draw guidelines for info banners
        // Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, LeftInfoPanelRect, 2);
        // Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, RightInfoPanelRect, 2);
        // Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, TopBannerRect, 2);
        // Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, ShopRect, 2);
    }

}
