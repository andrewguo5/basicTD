using BasicTD.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BasicTD.Components;

public class WinScreen : GComponent
{
    // Props
    private int Buffer => 120;
    private int Padding => Props["TextPadding"];
    private int SideBuffer => Props["SideBuffer"];
    private Rectangle MapBounds => Props["MapBounds"];
    private TextureAtlas Atlas => Props["Atlas"];
    private SpriteFont GameFont => Props["GameFont"];

    public bool Visible = true;

    // Player properties
    private Player Player => ((GameScene)ParentScene).Player;
    private Battlefield Battlefield => ((GameScene)ParentScene).Battlefield;

    public WinScreen(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
    }

    protected override void InitializeSelf()
    {
    }

    protected override void LoadContentSelf()
    {
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        if (((GameScene)ParentScene).Won)
        {
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
                Visible = !Visible;
        }
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        if (!Visible) return;
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        string winString = "You Win!";
        Vector2 winStringSize = GameFont.MeasureString(winString);

        if (((GameScene)ParentScene).Won)
        {
            // Draw the win screen if the player has won
            Core.Scaffold.DrawFilledRectangle(Core.SpriteBatch, Bounds, Color.Black * 0.75f);
            Core.SpriteBatch.DrawString(
                GameFont,
                winString,
                new Vector2(
                    Bounds.Left + (Bounds.Width - winStringSize.X) / 2,
                    Bounds.Top + (Bounds.Height - winStringSize.Y) / 2
                ),
                Color.Lime
            );
        }
        Core.SpriteBatch.End();
    }
}

        