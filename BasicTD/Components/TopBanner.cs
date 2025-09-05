using BasicTD.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Components;

public class TopBanner : GComponent
{
    // Props
    private int Buffer => 120;
    private int Padding => Props["TextPadding"];
    private int SideBuffer => Props["SideBuffer"];
    private Rectangle MapBounds => Props["MapBounds"];
    private TextureAtlas Atlas => Props["Atlas"];
    private SpriteFont GameFont => Props["GameFont"];

    // Strings
    private Vector2 NameStringLocation;
    private Vector2 LivesStringLocation;
    private Vector2 GoldStringLocation;
    private Vector2 WaveStringLocation;
    private Vector2 GearIconLocation;
    
    // Component-specific content
    private List<Sprite> IconSpriteManager;
    private Sprite GoldSprite;
    private Sprite HeartSprite;
    private Sprite SkullSprite;
    private Sprite GearSprite;

    // Player properties
    private Player Player => ((GameScene)ParentScene).Player;
    private Battlefield Battlefield => ((GameScene)ParentScene).Battlefield;

    public TopBanner(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
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
            Bounds.Width - SideBuffer - Padding, 25
        );
    }

    protected override void LoadContentSelf()
    {
        GoldSprite = Atlas.CreateSprite("gold");
        HeartSprite = Atlas.CreateSprite("heart");
        SkullSprite = Atlas.CreateSprite("skull");
        GearSprite = Atlas.CreateSprite("gear");

        IconSpriteManager = [
            GoldSprite,
            HeartSprite,
            SkullSprite,
            GearSprite
        ];

        foreach (Sprite sprite in IconSpriteManager)
        {
            sprite.CenterOrigin();
            sprite.Scale = new Vector2(2.5f, 2.5f);
        }
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Reset scene
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Reset();
        }
    }

    public void Reset()
    {
        Player.Reset();
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw banner
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(0, 0, Core.GraphicsDevice.Viewport.Width, 60),
            TDConstants.DarkBG
        );

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
            $"{Player.Health}",
            LivesStringLocation,
            Color.White
        );
        Core.SpriteBatch.DrawString(
            GameFont,
            $"{Player.Gold}",
            GoldStringLocation,
            Color.White
        );
        Core.SpriteBatch.DrawString(
            GameFont,
            $"{Battlefield.CurrentWave}/{5}",
            WaveStringLocation,
            Color.White
        );
        Core.SpriteBatch.End();
    }
}

        