using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;

namespace BasicTD.Components;

public class Main : GComponent
{
    private Rectangle MapBounds;
    private int VerticalOffset;
    private int Padding;
    private int SideBuffer;
    private int HTileOffset;
    private int VTileOffset;
    private Vector2 TilemapScale;
    private Tilemap InfoPanelMap;

    // private TextureAtlas Atlas;

    // private SpriteFont GameFont;

    public Main(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
    {
        // Extraction
        MapBounds = Props["MapBounds"];
        VerticalOffset = Props["VerticalOffset"];
        Padding = Props["TextPadding"];
        SideBuffer = Props["SideBuffer"];
        HTileOffset = Props["HTileOffset"];
        VTileOffset = Props["VTileOffset"];
        TilemapScale = Props["TilemapScale"];

        GComponent TopBanner;
        GComponent CreepInfo;
        GComponent Battlefield;
        GComponent Inventory;
        GComponent Shop;

        TopBanner = new TopBanner(
            new Rectangle(
                0,
                0,
                Bounds.Width,
                60
            ),
            Props
        );
        CreepInfo = new CreepInfo(
            new Rectangle(
                SideBuffer + HTileOffset,
                MapBounds.Top + VerticalOffset,
                MapBounds.Left - 2 * SideBuffer - 2 * HTileOffset,
                MapBounds.Height - 2 * VTileOffset
            ),
            Props
        );
        Battlefield = new Battlefield(
            MapBounds,
            Props
        );
        Inventory = new Inventory(
            new Rectangle(
                MapBounds.Right + SideBuffer + HTileOffset,
                MapBounds.Top + VerticalOffset,
                MapBounds.Left - 2 * SideBuffer - 2 * HTileOffset,
                MapBounds.Height - 2 * VTileOffset
            ),
            Props
        );
        Shop = new Shop(
            new Rectangle(
                MapBounds.Left,
                MapBounds.Bottom + 40,
                MapBounds.Width,
                Bounds.Height - (MapBounds.Bottom + 40)
            ),
            Props
        );

        AddChildren([
            TopBanner,
            CreepInfo,
            Battlefield,
            Inventory,
            Shop
        ]);
    }

    protected override void InitializeSelf()
    {
    }

    protected override void LoadContentSelf()
    {
        // As the immediate parent of the CreepInfo and Inventory comopnents, we load
        // the necessary tilemap here. This is a pattern that I want to keep in the GComponent model.
        // Tilemap = Tilemap.FromFile(Core.Content, "images/tilemap-definition.xml");
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        // Clear the background with the main background color
        Core.GraphicsDevice.Clear(TDConstants.MainBG);

        Core.SpriteBatch.Begin();
        DrawBanners();
        Core.SpriteBatch.End();
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Implement update logic for the main component
    }

    private void DrawBanners()
    {
        // Draw the side banners
        // Draw left side banner rectangle
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(
                SideBuffer,
                0,
                MapBounds.Left - 2 * SideBuffer,
                Core.GraphicsDevice.Viewport.Height
            ),
            TDConstants.LightBG
        );

        // Draw right side banner rectangle
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(
                MapBounds.Right + SideBuffer,
                0,
                Core.GraphicsDevice.Viewport.Width - (MapBounds.Right + 2 * SideBuffer),
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
}