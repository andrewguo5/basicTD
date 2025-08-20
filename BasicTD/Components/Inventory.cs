using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class Inventory : GComponent
{
    private int VerticalOffset;
    private int Padding;
    private int SideBuffer;
    private Rectangle MapBounds;
    private TextureAtlas Atlas;
    private Vector2 InventoryStringLocation;
    private SpriteFont GameFont;

    public Inventory(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
    {
        VerticalOffset = props["VerticalOffset"];
        Padding = props["TextPadding"];
        MapBounds = props["MapBounds"];
        SideBuffer = props["SideBuffer"];
        Atlas = props["Atlas"];
        GameFont = props["GameFont"];
    }


    protected override void InitializeSelf()
    {
        InventoryStringLocation = new Vector2(
            Padding + SideBuffer + MapBounds.Right,
            MapBounds.Top - VerticalOffset
        );
    }

    protected override void LoadContentSelf()
    {
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin();
        Core.SpriteBatch.DrawString(GameFont, $"Inventory", InventoryStringLocation, Color.White);
        Core.SpriteBatch.End();
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
    }
}

        