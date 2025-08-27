using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class Inventory : GComponent
{
    private int VerticalOffset => Props["VerticalOffset"];
    private int Padding => Props["TextPadding"];
    private int SideBuffer => Props["SideBuffer"];
    private Rectangle MapBounds => Props["MapBounds"];
    private TextureAtlas Atlas => Props["Atlas"];
    private Vector2 InventoryStringLocation;
    private SpriteFont GameFont => Props["GameFont"];
    private Tilemap InfoPanelMap;

    public Inventory(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
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
        InfoPanelMap = Tilemap.FromFile(Core.Content, "images/side-banner-tilemap.xml");
        InfoPanelMap.Scale = Props["TilemapScale"];
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw banner
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

        InfoPanelMap.Draw(Core.SpriteBatch, new Vector2(MapBounds.Right + SideBuffer, MapBounds.Top));
        Core.SpriteBatch.DrawString(GameFont, $"Inventory", InventoryStringLocation, Color.White);
        Core.SpriteBatch.End();
    }
}

        