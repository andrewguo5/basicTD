using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;

namespace BasicTD.Components;

public class Main : GComponent
{
    private Rectangle MapBounds => Props["MapBounds"];
    private int VerticalOffset => Props["VerticalOffset"];
    private int SideBuffer => Props["SideBuffer"];
    private int HTileOffset => Props["HTileOffset"];
    private int VTileOffset => Props["VTileOffset"];

    public Main(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
        GComponent TopBanner;
        GComponent CreepInfo;
        GComponent Battlefield;
        GComponent Inventory;
        GComponent Shop;
        GComponent WinScreen;
        GComponent LoseScreen;

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
                MapBounds.Top + VerticalOffset / 2,
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
                MapBounds.Top + VerticalOffset / 2,
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
        WinScreen = new WinScreen(
            ParentScene,
            new Rectangle(
                Bounds.Left,
                Bounds.Top,
                Bounds.Width,
                Bounds.Height
            ),
            Props
        );
        LoseScreen = new LoseScreen(
            ParentScene,
            new Rectangle(
                Bounds.Left,
                Bounds.Top,
                Bounds.Width,
                Bounds.Height
            ),
            Props
        );

        // Control draw order here by re-ordering children
        AddChildren([
            Battlefield,
            CreepInfo,
            Inventory,
            Shop,
            TopBanner,
            WinScreen,
            LoseScreen
        ]);
    }

    protected override void InitializeSelf()
    {
    }

    protected override void LoadContentSelf()
    {
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(TDConstants.MainBG);
    }

}