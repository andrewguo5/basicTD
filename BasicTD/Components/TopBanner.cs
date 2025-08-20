using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class TopBanner : GComponent
{
    private int Buffer;
    private int Padding;
    private int SideBuffer;
    private Rectangle MapBounds;
    private TextureAtlas Atlas;
    private Vector2 NameStringLocation;
    private Vector2 LivesStringLocation;
    private Vector2 GoldStringLocation;
    private Vector2 WaveStringLocation;
    private Vector2 GearIconLocation;
    private Sprite GoldSprite;
    private Sprite HeartSprite;
    private Sprite SkullSprite;
    private Sprite GearSprite;
    private SpriteFont GameFont;

    public TopBanner(Rectangle bounds, Dictionary<string, dynamic> props = null) : base(bounds, props)
    {
        Buffer = 120;
        Padding = props["TextPadding"];
        MapBounds = props["MapBounds"];
        SideBuffer = props["SideBuffer"];
        Atlas = props["Atlas"];
        GameFont = props["GameFont"];
    }

    protected override void InitializeSelf()
    {
        NameStringLocation = new Vector2(
            Padding + SideBuffer,
            20
        );
        LivesStringLocation = new Vector2(
            Padding + MapBounds.Right - Buffer * 3,
            20
        );
        GoldStringLocation = new Vector2(
            Padding + MapBounds.Right - Buffer * 2,
            20
        );
        WaveStringLocation = new Vector2(
            Padding + MapBounds.Right - Buffer,
            20
        );
        GearIconLocation = new Vector2(
            Bounds.Width - SideBuffer - Padding - 40, 25
        );
    }

    protected override void LoadContentSelf()
    {
        // Implement content loading logic for the top banner
        GoldSprite = Atlas.CreateSprite("gold");
        HeartSprite = Atlas.CreateSprite("heart");
        SkullSprite = Atlas.CreateSprite("skull");
        GearSprite = Atlas.CreateSprite("gear");
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Implement update logic for the top banner
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin();
        // Draw the icons for lives, gold, and wave
        int iconPadding = 5;
        int iconDrop = 5;

        // Draw lives icon
        Vector2 livesIconPosition = new Vector2(
            LivesStringLocation.X - 20 - iconPadding,
            LivesStringLocation.Y + iconDrop
        );
        HeartSprite.Draw(
            Core.SpriteBatch,
            livesIconPosition
        );

        // Draw gold icon
        Vector2 goldIconPosition = new Vector2(
            GoldStringLocation.X - 20 - iconPadding,
            GoldStringLocation.Y + iconDrop
        );
        GoldSprite.Draw(
            Core.SpriteBatch,
            goldIconPosition
        );

        // Draw wave icon
        Vector2 waveIconPosition = new Vector2(
            WaveStringLocation.X - 20 - iconPadding,
            WaveStringLocation.Y + iconDrop
        );
        SkullSprite.Draw(
            Core.SpriteBatch,
            waveIconPosition
        );

        // Draw settings icon
        GearSprite.Draw(
            Core.SpriteBatch,
            GearIconLocation
        );

        // Draw strings
        Core.SpriteBatch.DrawString(
            GameFont,
            "Bishop",
            NameStringLocation,
            Color.White
        );

        Core.SpriteBatch.DrawString(
            GameFont,
            $"{20}",
            LivesStringLocation,
            Color.White
        );
        Core.SpriteBatch.DrawString(
            GameFont,
            $"{100}",
            GoldStringLocation,
            Color.White
        );
        Core.SpriteBatch.DrawString(
            GameFont,
            $"{1 / 5}",
            WaveStringLocation,
            Color.White
        );
        Core.SpriteBatch.End();
    }
}

        