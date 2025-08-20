using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class Battlefield : GComponent
{
    private int VerticalOffset;
    private int Padding;
    private int SideBuffer;
    private TextureAtlas Atlas;
    private Vector2 InventoryStringLocation;
    private SpriteFont GameFont;

    public Battlefield(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
    {
        VerticalOffset = props["VerticalOffset"];
        Padding = props["TextPadding"];
        SideBuffer = props["SideBuffer"];
        Atlas = props["Atlas"];
        GameFont = props["GameFont"];
    }


    protected override void InitializeSelf()
    {
    }

    protected override void LoadContentSelf()
    {
    }

    protected override void DrawSelf(GameTime gameTime)
    {
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
    }
}

        