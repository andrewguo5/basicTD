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
using BasicTD.Sections;

namespace BasicTD.Scenes;

public class BasicMapScene : BattleScene
{
    Tilemap Tilemap;

    // Font locations
    private Vector2 NameStringLocation;
    private Vector2 CreepStringLocation;
    private Vector2 InventoryStringLocation;
    private Vector2 GoldStringLocation;
    private Vector2 LivesStringLocation;
    private Vector2 WaveStringLocation;
    private Vector2 ShopStringLocation;
    private Vector2 GearIconLocation;
    private int sideBannerBuffer = 30;
    private int topBannerInfoBuffer = 120;
    private int textPadding = 10;

    // Info panels
    private Tilemap InfoPanelMap;

    // Layout Rectangles
    private Rectangle LeftInfoPanelRect;
    private Rectangle RightInfoPanelRect;
    private Rectangle TopBannerRect;
    private Rectangle ShopRect;

    // Shop Section
    private Shop CardShop;

    public BasicMapScene() : base()
    {
        MapBounds = new Rectangle(
            240, 100,
            720, 420
        );
    }

    public override void Initialize()
    {
        // NOTE: Above this line, content has not been loaded yet.
        base.Initialize();
        // NOTE: Content has been loaded after this line

        // Setup layout rectangles
        int VTileOffset = 16;
        int HTileOffset = 24;
        LeftInfoPanelRect = new Rectangle(
            sideBannerBuffer + HTileOffset,
            MapBounds.Top + VTileOffset,
            MapBounds.Left - 2 * sideBannerBuffer - 2 * HTileOffset,
            MapBounds.Height - 2 * VTileOffset
        );

        RightInfoPanelRect = new Rectangle(
            MapBounds.Right + sideBannerBuffer + HTileOffset,
            MapBounds.Top + VTileOffset,
            MapBounds.Left - 2 * sideBannerBuffer - 2 * HTileOffset,
            MapBounds.Height - 2 * VTileOffset
        );

        TopBannerRect = new Rectangle(
            0,
            0,
            Core.GraphicsDevice.Viewport.Width,
            60
        );

        ShopRect = new Rectangle(
            MapBounds.Left,
            MapBounds.Bottom + 40,
            MapBounds.Right - MapBounds.Left,
            Core.GraphicsDevice.Viewport.Height - (MapBounds.Bottom + 40)
        );

        // Set up font locations
        int p = textPadding;
        int v = 30;
        NameStringLocation = new Vector2(p+sideBannerBuffer, 20);
        CreepStringLocation = new Vector2(p+sideBannerBuffer, MapBounds.Top - v);
        InventoryStringLocation = new Vector2(p+MapBounds.Right + sideBannerBuffer, MapBounds.Top - v);
        LivesStringLocation = new Vector2(p+MapBounds.Right - 3*topBannerInfoBuffer, 20);
        GoldStringLocation = new Vector2(p+MapBounds.Right - 2*topBannerInfoBuffer, 20);
        WaveStringLocation = new Vector2(p+MapBounds.Right - 1*topBannerInfoBuffer, 20);
        ShopStringLocation = new Vector2(p+sideBannerBuffer, MapBounds.Bottom + 60);
        GearIconLocation = new Vector2(Core.GraphicsDevice.Viewport.Width - sideBannerBuffer - p - GearSprite.Region.Width, 25);

        // Initialize the card shop
        CardShop = new Shop(this, ShopRect);

        // Scene management
        NextScene = new LineScene();
    }

    public override void InitializePath()
    {
        // Load the path from the XML file
        Path = LinkedPath.FromFile(
            Core.Content,
            "paths/beginner-map-std.xml",
            MapBounds.Location.ToVector2(),
            1f
        );
        Path.LoadSprites(Atlas);
    }

    public override void LoadContent()
    {
        base.LoadContent();

        Tilemap = Tilemap.FromFile(Core.Content, "images/tilemap-definition.xml");
        Tilemap.Scale = new Vector2(60f / 16f, 60f / 16f);

        InfoPanelMap = Tilemap.FromFile(Core.Content, "images/side-banner-tilemap.xml");
        InfoPanelMap.Scale = new Vector2(60f / 16f, 60f / 16f);
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        UpdateCreep(gameTime);
        UpdatePlacingTower(gameTime);
        UpdateTowers(gameTime);
        UpdateCreepList(gameTime);
        CardShop.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(92, 105, 127));

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Vector2 tilemapOffset = MapBounds.Location.ToVector2();
        Tilemap.Draw(Core.SpriteBatch, tilemapOffset);
        Core.SpriteBatch.End();

        DrawPath(gameTime);
        DrawCreeps(gameTime);
        DrawPlacedTowers(gameTime);
        DrawPlacingTower(gameTime);
        DrawSelectedTower(gameTime);

        // Draw the scaffold for debugging
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawBanners(Core.SpriteBatch);
        DrawInfoPanels(Core.SpriteBatch);
        // DrawCardShop(Core.SpriteBatch);
        CardShop.Draw(Core.SpriteBatch);
        DrawSpriteFonts(Core.SpriteBatch);
        DrawTopBannerIcons(Core.SpriteBatch);

        if (DebugDraw)
            DrawScaffoldingLines(Core.SpriteBatch);

        Core.SpriteBatch.End();
    }

    /// <summary>
    /// Draws the solid color banners on the sides, top, and bottom of the map area.
    /// </summary>
    /// <param name="spriteBatch"></param>
    private void DrawBanners(SpriteBatch spriteBatch)
    {
        // Draw the side banners
        // Draw left side banner rectangle
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(
                sideBannerBuffer,
                0,
                MapBounds.Left - 2 * sideBannerBuffer,
                Core.GraphicsDevice.Viewport.Height
            ),
            TDConstants.LightBG
        );

        // Draw right side banner rectangle
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(
                MapBounds.Right + sideBannerBuffer,
                0,
                Core.GraphicsDevice.Viewport.Width - (MapBounds.Right + 2 * sideBannerBuffer),
                Core.GraphicsDevice.Viewport.Height
            ),
            TDConstants.LightBG
        );

        // Draw the top banner
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(0, 0, Core.GraphicsDevice.Viewport.Width, 60),
            TDConstants.DarkBG
        );

        // Draw the bottom banner
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(0, MapBounds.Bottom + 40, Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height - (MapBounds.Bottom + 40)),
            TDConstants.DarkBG
        );
    }


    private void DrawInfoPanels(SpriteBatch spriteBatch)
    {
        InfoPanelMap.Draw(spriteBatch, new Vector2(sideBannerBuffer, MapBounds.Top));
        InfoPanelMap.Draw(spriteBatch, new Vector2(MapBounds.Right + sideBannerBuffer, MapBounds.Top));
    }

    /// <summary>
    /// Draws all sprite font texts on the screen (non-UI)
    /// </summary>
    /// <param name="spriteBatch"></param>
    private void DrawSpriteFonts(SpriteBatch spriteBatch)
    {
        // Draw the various strings
        spriteBatch.DrawString(font, "Bishop", NameStringLocation, Color.White);
        spriteBatch.DrawString(font, $"Creeps", CreepStringLocation, Color.White);
        spriteBatch.DrawString(font, $"Inventory", InventoryStringLocation, Color.White);
        spriteBatch.DrawString(font, $"{20}", LivesStringLocation, Color.Red);
        spriteBatch.DrawString(font, $"{99}", GoldStringLocation, Color.Gold);
        spriteBatch.DrawString(font, $"{1}/{5}", WaveStringLocation, Color.White);
        spriteBatch.DrawString(font, "Shop", ShopStringLocation, Color.White);
    }

    /// <summary>
    /// Draws the icons that live in the top banner.
    /// </summary>
    /// <param name="spriteBatch"></param>
    private void DrawTopBannerIcons(SpriteBatch spriteBatch)
    {
        // Draw the icons for lives, gold, and wave
        int iconPadding = 5;
        int iconDrop = 5;

        // Draw lives icon
        Vector2 livesIconPosition = new Vector2(
            LivesStringLocation.X - 20 - iconPadding,
            LivesStringLocation.Y + iconDrop
        );
        HeartSprite.Draw(
            spriteBatch,
            livesIconPosition
        );

        // Draw gold icon
        Vector2 goldIconPosition = new Vector2(
            GoldStringLocation.X - 20 - iconPadding,
            GoldStringLocation.Y + iconDrop
        );
        GoldSprite.Draw(
            spriteBatch,
            goldIconPosition
        );

        // Draw wave icon
        Vector2 waveIconPosition = new Vector2(
            WaveStringLocation.X - 20 - iconPadding,
            WaveStringLocation.Y + iconDrop
        );
        SkullSprite.Draw(
            spriteBatch,
            waveIconPosition
        );

        // Draw settings icon
        GearSprite.Draw(
            spriteBatch,
            GearIconLocation
        );
    }

    // private void DrawCardShop(SpriteBatch spriteBatch)
    // {
    //     // Draw the card shop area
    //     var pos = new Vector2(ShopRect.Left + 20, ShopRect.Top + 15);

    //     foreach (var card in CardSpriteManager)
    //     {
    //         card.Draw(spriteBatch, pos);
    //         pos.X += 120;
    //     }
    // }

    /// <summary>
    /// Draws red 1px debug lines
    /// </summary>
    /// <param name="spriteBatch"></param>
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

        // Draw left and right side banner boundaries
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, sideBannerBuffer);
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Left - sideBannerBuffer);

        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Right + sideBannerBuffer);
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, Core.GraphicsDevice.Viewport.Width - sideBannerBuffer);

        // Draw guidelines for info banners
        Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, LeftInfoPanelRect, 2);
        Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, RightInfoPanelRect, 2);
        Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, TopBannerRect, 2);
        Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, ShopRect, 2);
    }


    public void DrawMarkers(GameTime gameTime)
    {
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

        Core.SpriteBatch.End();
    }
}