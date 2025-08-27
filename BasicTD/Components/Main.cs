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

    // private TextureAtlas Atlas;

    // private SpriteFont GameFont;

    public Main(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
        // Extraction
        MapBounds = Props["MapBounds"];
        VerticalOffset = Props["VerticalOffset"];
        Padding = Props["TextPadding"];
        SideBuffer = Props["SideBuffer"];
        HTileOffset = Props["HTileOffset"];
        VTileOffset = Props["VTileOffset"];

        GComponent TopBanner;
        GComponent CreepInfo;
        GComponent Battlefield;
        GComponent Inventory;
        GComponent Shop;

        TopBanner = new TopBanner(
            ParentScene,
            new Rectangle(
                0,
                0,
                Bounds.Width,
                60
            ),
            Props
        );
        CreepInfo = new CreepInfo(
            ParentScene,
            new Rectangle(
                SideBuffer + HTileOffset,
                MapBounds.Top + VerticalOffset,
                MapBounds.Left - 2 * SideBuffer - 2 * HTileOffset,
                MapBounds.Height - 2 * VTileOffset
            ),
            Props
        );
        Battlefield = new Battlefield(
            ParentScene,
            MapBounds,
            Props
        );
        Inventory = new Inventory(
            ParentScene,
            new Rectangle(
                MapBounds.Right + SideBuffer + HTileOffset,
                MapBounds.Top + VerticalOffset,
                MapBounds.Left - 2 * SideBuffer - 2 * HTileOffset,
                MapBounds.Height - 2 * VTileOffset
            ),
            Props
        );
        Shop = new Shop(
            ParentScene,
            new Rectangle(
                MapBounds.Left,
                MapBounds.Bottom + 40,
                MapBounds.Width,
                Bounds.Height - (MapBounds.Bottom + 40)
            ),
            Props
        );

        AddChildren([
            Battlefield,
            CreepInfo,
            Inventory,
            Shop,
            TopBanner
        ]);
    }

    protected override void InitializeSelf()
    {
    }

    protected override void LoadContentSelf()
    {
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        // Clear the background with the main background color
        Core.GraphicsDevice.Clear(TDConstants.MainBG);
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Implement update logic for the main component
    }
}