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
    private Tilemap InfoPanelMap;

    public CreepInfo(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
        VerticalOffset = Props["VerticalOffset"];
        Padding = Props["TextPadding"];
        MapBounds = Props["MapBounds"];
        SideBuffer = Props["SideBuffer"];
        Atlas = Props["Atlas"];
        GameFont = Props["GameFont"];
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
        InfoPanelMap = Tilemap.FromFile(Core.Content, "images/side-banner-tilemap.xml");
        InfoPanelMap.Scale = Props["TilemapScale"];
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw banner
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

        InfoPanelMap.Draw(Core.SpriteBatch, new Vector2(SideBuffer, MapBounds.Top));
        Core.SpriteBatch.DrawString(GameFont, $"Creeps", CreepStringLocation, Color.White);
        Core.SpriteBatch.End();
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
    }
}

        