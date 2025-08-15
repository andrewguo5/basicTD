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
using System;
using System.Reflection.Metadata;

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
    private int sideBannerBuffer = 30;
    private int topBannerInfoBuffer = 60;
    private int textPadding = 10;

    // Wall sprites
    // TextureAtlas WallFloorAtlas;
    // Sprite VerticalWallTop;
    // Sprite VerticalWallMid;
    // Sprite VerticalWallBot;

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

        // Set up font locations
        int p = textPadding;
        NameStringLocation = new Vector2(p+sideBannerBuffer, 20);
        CreepStringLocation = new Vector2(p+sideBannerBuffer, MapBounds.Top);
        InventoryStringLocation = new Vector2(p+MapBounds.Right + sideBannerBuffer, MapBounds.Top);
        GoldStringLocation = new Vector2(p+MapBounds.Right - 3*topBannerInfoBuffer, 20);
        LivesStringLocation = new Vector2(p+MapBounds.Right - 2*topBannerInfoBuffer, 20);
        WaveStringLocation = new Vector2(p+MapBounds.Right - 1*topBannerInfoBuffer, 20);
        ShopStringLocation = new Vector2(p+MapBounds.Left, MapBounds.Bottom + 60);

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
        Tilemap.Scale = new Vector2(60f/16f, 60f/16f);

        // TODO: Remove, as these are no longer needed
        // WallFloorAtlas = TextureAtlas.FromFile(Core.Content, "images/walls_floor_atlas.xml");
        // VerticalWallTop = WallFloorAtlas.CreateSprite("vertical-wall-top");
        // VerticalWallMid = WallFloorAtlas.CreateSprite("vertical-wall-mid");
        // VerticalWallBot = WallFloorAtlas.CreateSprite("vertical-wall-bot");

        // VerticalWallTop.Scale = new Vector2(3f, 3f);
        // VerticalWallMid.Scale = new Vector2(3f, 3f);
        // VerticalWallBot.Scale = new Vector2(3f, 3f);
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        UpdateCreep(gameTime);
        UpdatePlacingTower(gameTime);
        UpdateTowers(gameTime);
        UpdateCreepList(gameTime);
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
        DrawSpriteFonts(Core.SpriteBatch);
        DrawScaffoldingLines(Core.SpriteBatch);

        Core.SpriteBatch.End();
    }

    private void DrawBanners(SpriteBatch spriteBatch)
    {   
        // Draw the side banners
        // Draw left side banner rectangle
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(
                sideBannerBuffer, 
                0, 
                MapBounds.Left - 2*sideBannerBuffer, 
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
                Core.GraphicsDevice.Viewport.Width - (MapBounds.Right + 2*sideBannerBuffer), 
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
            new Rectangle(0, MapBounds.Bottom + 40, Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height - (MapBounds.Bottom+40)),
            TDConstants.DarkBG
        );
    }

    private void DrawSpriteFonts(SpriteBatch spriteBatch)
    {
        // Draw the various strings
        spriteBatch.DrawString(font, "Bishop", NameStringLocation, Color.White);
        spriteBatch.DrawString(font, $"Creeps", CreepStringLocation, Color.White);
        spriteBatch.DrawString(font, $"Inventory", InventoryStringLocation, Color.White);
        spriteBatch.DrawString(font, $"{99}", GoldStringLocation, Color.Gold);
        spriteBatch.DrawString(font, $"{20}", LivesStringLocation, Color.Red);
        spriteBatch.DrawString(font, $"{1}/{5}", WaveStringLocation, Color.White);
        spriteBatch.DrawString(font, "Shop: 1-Tower 2-Sell 3-Upgrade", ShopStringLocation, Color.White);
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

        // Draw left and right side banner boundaries
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, sideBannerBuffer);
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Left - sideBannerBuffer);

        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Right + sideBannerBuffer);
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, Core.GraphicsDevice.Viewport.Width - sideBannerBuffer);
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