using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class Main : GComponent
{
    public Main(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
    {
        GComponent TopBanner;
        GComponent CreepInfo;
        GComponent Map;
        GComponent Inventory;
        GComponent Shop;

        TopBanner = new TopBanner(
            new Rectangle(0, 0, bounds.Width, 60),
            props
        );

        AddChildren([
            TopBanner,
            CreepInfo,
            Map,
            Inventory,
            Shop
        ]);
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        // Clear the background with the main background color
        Core.GraphicsDevice.Clear(TDConstants.MainBG);
        // Draw the main component
        base.DrawSelf(gameTime);
    }
    {
        Core.GraphicsDevice.Clear(TDConstants.MainBG);
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Implement update logic for the main component
    }

    protected override void InitializeSelf()
    {
        // Implement initialization logic for the main component
    }

    protected override void LoadContentSelf()
    {
        // Implement content loading logic for the main component
    }
}