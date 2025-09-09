using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;
using BasicTD.Scenes;

namespace BasicTD.Components;

public class CreepInfo : GComponent
{
    private int VerticalOffset => Props["VerticalOffset"];
    private int Padding => Props["TextPadding"];
    private int SideBuffer => Props["SideBuffer"];
    private Rectangle MapBounds => Props["MapBounds"];
    private TextureAtlas Atlas => Props["Atlas"];
    private Vector2 CreepStringLocation;
    private SpriteFont GameFont => Props["GameFont"];
    private Tilemap InfoPanelMap;
    private Vector2 WaveStringLocation;
    private Battlefield Battlefield => ((GameScene)ParentScene).Battlefield;

    public CreepInfo(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
    }


    protected override void InitializeSelf()
    {
        CreepStringLocation = new Vector2(
            Padding + SideBuffer,
            MapBounds.Top - VerticalOffset
        );

        WaveStringLocation = new Vector2(
            Bounds.Left,
            Bounds.Bottom - VerticalOffset
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
                SideBuffer,
                0,
                MapBounds.Left - 2 * SideBuffer,
                Core.GraphicsDevice.Viewport.Height
            ),
            TDConstants.LightBG
        );

        InfoPanelMap.Draw(Core.SpriteBatch, new Vector2(SideBuffer, MapBounds.Top));
        Core.SpriteBatch.DrawString(GameFont, $"Creeps", CreepStringLocation, Color.White);
        Core.SpriteBatch.DrawString(GameFont, $"{Battlefield.CurrentWave}/{5}", WaveStringLocation, Color.White);
        Core.SpriteBatch.End();
    }
}

        