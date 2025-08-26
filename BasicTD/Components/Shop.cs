using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;
using System;
using System.Linq;
using BasicTD.Scenes;

namespace BasicTD.Components;

public class Shop : GComponent
{
    private int VerticalOffset;
    private int Padding;
    private int SideBuffer;
    private TextureAtlas Atlas;
    private TextureAtlas CardAtlas;
    private SpriteFont GameFont;
    private Vector2 ShopStringLocation;
    private Rectangle MapBounds;
    private List<Rectangle> CardSlotManager { get; set; }
    private Rectangle CardSlot1;
    private Rectangle CardSlot2;
    private Rectangle CardSlot3;
    private Rectangle CardSlot4;
    private Rectangle CardSlot5;
    private Rectangle CardSlot6;
    private int hoveredCardSlotIndex = -1;
    private List<Sprite> CardSpriteManager;
    private List<Sprite> CardEmblemSprites;
    private List<Sprite> CardSymbolSprites;
    private Sprite GoldSprite;
    Random random;
    private int RandomEmblemIndex;
    private int RandomSymbolIndex;

    public Shop(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
        VerticalOffset = props["VerticalOffset"];
        Padding = props["TextPadding"];
        SideBuffer = props["SideBuffer"];
        CardAtlas = props["CardAtlas"];
        Atlas = props["Atlas"];
        GameFont = props["GameFont"];
        MapBounds = props["MapBounds"];

        int X = Bounds.Left + 20;
        int Y = Bounds.Top + 15;
        int padding = 40;

        CardSlot1 = new Rectangle(X, Y, 80, 132);
        CardSlot2 = new Rectangle(CardSlot1.Right + padding, Y, 80, 132);
        CardSlot3 = new Rectangle(CardSlot2.Right + padding, Y, 80, 132);
        CardSlot4 = new Rectangle(CardSlot3.Right + padding, Y, 80, 132);
        CardSlot5 = new Rectangle(CardSlot4.Right + padding, Y, 80, 132);
        CardSlot6 = new Rectangle(CardSlot5.Right + padding, Y, 80, 132);
    }

    protected override void InitializeSelf()
    {
        random = new Random();
        RandomEmblemIndex = random.Next(CardEmblemSprites.Count);
        RandomSymbolIndex = random.Next(CardSymbolSprites.Count);

        CardSlotManager = new List<Rectangle>
        {
            CardSlot1,
            CardSlot2,
            CardSlot3,
            CardSlot4,
            CardSlot5,
            CardSlot6
        };

        ShopStringLocation = new Vector2(
            Padding + SideBuffer,
            MapBounds.Bottom + 2 * VerticalOffset
        );
    }

    protected override void LoadContentSelf()
    {
        CardSpriteManager = new List<Sprite>
        {
            CardAtlas.CreateSprite("card-common"),
            CardAtlas.CreateSprite("card-uncommon"),
            CardAtlas.CreateSprite("card-rare"),
            CardAtlas.CreateSprite("card-epic"),
            CardAtlas.CreateSprite("card-legendary"),
            CardAtlas.CreateSprite("card-null")
        };

        // Load card emblem sprites
        CardEmblemSprites = new List<Sprite>()
        {
            CardAtlas.CreateSprite("emblem-shield"),
            CardAtlas.CreateSprite("emblem-bullet"),
            CardAtlas.CreateSprite("emblem-gem"),
        };

        CardSymbolSprites = new List<Sprite>()
        {
            CardAtlas.CreateSprite("symbol-pulse"),
            CardAtlas.CreateSprite("symbol-shockwave"),
            CardAtlas.CreateSprite("symbol-loop"),
            CardAtlas.CreateSprite("symbol-square"),
            CardAtlas.CreateSprite("symbol-oval"),
            CardAtlas.CreateSprite("symbol-spare"),
        };

        foreach (Sprite sprite in CardSpriteManager.Concat(CardEmblemSprites).Concat(CardSymbolSprites))
        {
            sprite.Scale = new Vector2(2.0f, 2.0f);
        }

        GoldSprite = Atlas.CreateSprite("gold");
        GoldSprite.CenterOrigin();
        GoldSprite.Scale = new Vector2(2.5f, 2.5f);
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        DrawCards();
        DrawCardCosts();
        DrawText();

        if (((GameScene)ParentScene).DebugDraw)
        {
            foreach (var rect in CardSlotManager)
            {
                // Draw the card slots
                Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, rect, 1);
            }
        }
        Core.SpriteBatch.End();
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Update logic for the shop can be added here if needed
        Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
        foreach (var rect in CardSlotManager)
        {
            if (rect.Contains(mousePos))
            {
                // If the mouse is over a card slot, set the hovered index
                hoveredCardSlotIndex = CardSlotManager.IndexOf(rect);
                return;
            }
        }
        // If the mouse is not over any card slot, reset the hovered index
        hoveredCardSlotIndex = -1;
    }


    private void DrawCards()
    {
        // Draw each card at its corresponding card slot location
        for (int i = 0; i < CardSpriteManager.Count && i < CardSlotManager.Count; i++)
        {
            var card = CardSpriteManager[i];
            var slotRect = CardSlotManager[i];
            Color highlightColor = (hoveredCardSlotIndex == i) ? Color.White : Color.LightGray;
            card.Draw(Core.SpriteBatch, new Vector2(slotRect.X, slotRect.Y), highlightColor);

            // Draw a random emblem and random symbol on each card
            if (CardEmblemSprites != null && CardEmblemSprites.Count > 0)
            {
                var emblemSprite = CardEmblemSprites[RandomEmblemIndex];
                Vector2 emblemPosition = new Vector2(slotRect.X, slotRect.Y);
                emblemSprite.Draw(Core.SpriteBatch, emblemPosition, highlightColor);
            }

            if (CardSymbolSprites != null && CardSymbolSprites.Count > 0)
            {
                var symbolSprite = CardSymbolSprites[RandomSymbolIndex];
                Vector2 symbolPosition = new Vector2(slotRect.X, slotRect.Y);
                symbolSprite.Draw(Core.SpriteBatch, symbolPosition, highlightColor);
            }
        }
    }

    private void DrawCardCosts()
    {
        // Draw the cost of each card in its corresponding slot
        for (int i = 0; i < CardSpriteManager.Count && i < CardSlotManager.Count; i++)
        {
            var card = CardSpriteManager[i];
            var slotRect = CardSlotManager[i];
            Vector2 costPosition = new Vector2(slotRect.X + slotRect.Width / 2, slotRect.Bottom - 18); // Adjust position as needed

            Color highlightColor = (hoveredCardSlotIndex == i) ? Color.White : Color.LightGray;
            Core.SpriteBatch.DrawString(
                GameFont,
                $"{i + 1}",
                costPosition,
                highlightColor,
                0f,
                Vector2.Zero,
                1f, // 1/3 scale
                SpriteEffects.None,
                0f
            );

            // Draw the GoldSprite to the left of the cost string
            if (GoldSprite != null)
            {
                Vector2 goldSpritePosition = new Vector2(costPosition.X - 20, costPosition.Y + 6);
                GoldSprite.Draw(Core.SpriteBatch, goldSpritePosition, highlightColor, new Vector2(0.8f, 0.8f));
            }
        }
    }

    private void DrawText()
    {
        Core.SpriteBatch.DrawString(GameFont, $"Shop", ShopStringLocation, Color.White);
    }
}

        