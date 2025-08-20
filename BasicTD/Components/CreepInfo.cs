using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class CreepInfo : GComponent
{
    private int VerticalOffset;
    private int Padding;
    private int SideBuffer;
    private Rectangle MapBounds;
    private TextureAtlas Atlas;
    private Vector2 CreepStringLocation;
    private SpriteFont GameFont;

    public CreepInfo(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
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
        CreepStringLocation = new Vector2(
            Padding + SideBuffer,
            MapBounds.Top - VerticalOffset
        );
    }

    protected override void LoadContentSelf()
    {
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin();
        Core.SpriteBatch.DrawString(GameFont, $"Creeps", CreepStringLocation, Color.White);
        Core.SpriteBatch.End();
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
    }
}

        