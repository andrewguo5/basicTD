using BasicTD.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class LoseScreen : GComponent
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

    public LoseScreen(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
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
        if (((GameScene)ParentScene).Lost)
        {
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
                Visible = !Visible;
        }
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        if (!Visible) return;
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        string loseString = "You Lose!";
        Vector2 loseStringSize = GameFont.MeasureString(loseString);

        if (((GameScene)ParentScene).Lost)
        {
            // Draw the lose screen if the player has lost
            Core.Scaffold.DrawFilledRectangle(Core.SpriteBatch, Bounds, Color.Black * 0.75f);
            Core.SpriteBatch.DrawString(
                GameFont,
                loseString,
                new Vector2(
                    Bounds.Left + (Bounds.Width - loseStringSize.X) / 2,
                    Bounds.Top + (Bounds.Height - loseStringSize.Y) / 2
                ),
                Color.Red
            );
        }
        Core.SpriteBatch.End();
    }
}

        