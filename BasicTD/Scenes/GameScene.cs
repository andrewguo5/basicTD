using Microsoft.Xna.Framework;
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
    public GameComponent Main { get; set; } 


    // Scene Manager
    public Scene NextScene { get; set; }

    // Font
    public SpriteFont GameFont;

    public GameScene() : base()
    {

    }

    public override void Initialize()
    {
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

        // Initialize Map: A class that tracks the placed towers, etc.
    }

    public override void LoadContent()
    {
        // 2. Load
        base.LoadContent();

        // Load Atlas
        Atlas = Core.Content.Load<TextureAtlas>("Atlases/Atlas");
        CardAtlas = Core.Content.Load<TextureAtlas>("Atlases/CardAtlas");

        // Load Fonts
        GameFont = Core.Content.Load<SpriteFont>("Fonts/GameFont");

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
            { "TorchSprite", Atlas.CreateAnimatedSprite("torch-red-animation") },
        };
    }

    private void LoadEffects()
    {
        
    }

}
