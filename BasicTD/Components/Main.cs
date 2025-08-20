using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Scenes;
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
    // private TextureAtlas Atlas;

    // private SpriteFont GameFont;

    public Main(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
    {
        GComponent TopBanner;
        GComponent CreepInfo;
        GComponent Battlefield;
        GComponent Inventory;
        GComponent Shop;

        MapBounds = props["MapBounds"];
        VerticalOffset = props["VerticalOffset"];
        Padding = props["TextPadding"];
        SideBuffer = props["SideBuffer"];
        HTileOffset = props["HTileOffset"];
        VTileOffset = props["VTileOffset"];

        TopBanner = new TopBanner(
            new Rectangle(
                0,
                0,
                Bounds.Width,
                60
            ),
            props
        );
        CreepInfo = new CreepInfo(
            new Rectangle(
                SideBuffer + HTileOffset,
                MapBounds.Top + VerticalOffset,
                MapBounds.Left - 2 * SideBuffer - 2 * HTileOffset,
                MapBounds.Height - 2 * VTileOffset
            ),
            props
        );
        Battlefield = new Battlefield(
            MapBounds,
            props
        );
        Inventory = new Inventory(
            new Rectangle(
                MapBounds.Right + SideBuffer + HTileOffset,
                MapBounds.Top + VerticalOffset,
                MapBounds.Left - 2 * SideBuffer - 2 * HTileOffset,
                MapBounds.Height - 2 * VTileOffset
            ),
            props
        );
        Shop = new Shop(
            new Rectangle(
                MapBounds.Left,
                MapBounds.Bottom + 40,
                MapBounds.Width,
                Bounds.Height - (MapBounds.Bottom + 40)
            ),
            props
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
        // Implement initialization logic for the main component
    }

    protected override void LoadContentSelf()
    {
        // Implement content loading logic for the main component
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