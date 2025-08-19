using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using BasicTD.Scenes;
using System.Collections.Generic;
using MonoGameLibrary;
using MonoGameLibrary.Input;

namespace BasicTD.Sections;

public class Shop
{
    private Rectangle Bounds;
    private BasicMapScene ParentScene; // Reference to the Scene

    public List<Rectangle> CardSlotManager { get; private set; }
    private Rectangle CardSlot1;
    private Rectangle CardSlot2;
    private Rectangle CardSlot3;
    private Rectangle CardSlot4;
    private Rectangle CardSlot5;
    private Rectangle CardSlot6;
    private int hoveredCardSlotIndex = -1;

    public Shop(BasicMapScene scene, Rectangle ShopRect)
    {
        ParentScene = scene;
        Bounds = ShopRect;

        int X = ShopRect.Left + 20;
        int Y = ShopRect.Top + 15;
        int padding = 40;
        CardSlot1 = new Rectangle(X, Y, 80, 132);
        CardSlot2 = new Rectangle(CardSlot1.Right + padding, Y, 80, 132);
        CardSlot3 = new Rectangle(CardSlot2.Right + padding, Y, 80, 132);
        CardSlot4 = new Rectangle(CardSlot3.Right + padding, Y, 80, 132);
        CardSlot5 = new Rectangle(CardSlot4.Right + padding, Y, 80, 132);
        CardSlot6 = new Rectangle(CardSlot5.Right + padding, Y, 80, 132);

        CardSlotManager = new List<Rectangle>
        {
            CardSlot1,
            CardSlot2,
            CardSlot3,
            CardSlot4,
            CardSlot5,
            CardSlot6
        };
    }

    public void Update(GameTime gameTime)
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

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawCards(spriteBatch);
        DrawCardCosts(spriteBatch);

        if (ParentScene.DebugDraw)
        {
            foreach (var rect in CardSlotManager)
            {
                // Draw the card slots
                Core.Scaffold.DrawRectanglePerimeter(spriteBatch, rect, 1);
            }
        }
    }

    public void DrawCards(SpriteBatch spriteBatch)
    {
        // Draw each card at its corresponding card slot location
        for (int i = 0; i < ParentScene.CardSpriteManager.Count && i < CardSlotManager.Count; i++)
        {
            var card = ParentScene.CardSpriteManager[i];
            var slotRect = CardSlotManager[i];
            Color highlightColor = (hoveredCardSlotIndex == i) ? Color.White : Color.LightGray;
            card.Draw(spriteBatch, new Vector2(slotRect.X, slotRect.Y), highlightColor);
        }
    }

    public void DrawCardCosts(SpriteBatch spriteBatch)
    {
        // Draw the cost of each card in its corresponding slot
        for (int i = 0; i < ParentScene.CardSpriteManager.Count && i < CardSlotManager.Count; i++)
        {
            var card = ParentScene.CardSpriteManager[i];
            var slotRect = CardSlotManager[i];
            Vector2 costPosition = new Vector2(slotRect.X + slotRect.Width / 2, slotRect.Bottom - 18); // Adjust position as needed

            Color highlightColor = (hoveredCardSlotIndex == i) ? Color.White : Color.LightGray;
            spriteBatch.DrawString(
                ParentScene.font,
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
            if (ParentScene.GoldSprite != null)
            {
                Vector2 goldSpritePosition = new Vector2(costPosition.X - 20, costPosition.Y + 6);
                ParentScene.GoldSprite.Draw(spriteBatch, goldSpritePosition, highlightColor, new Vector2(0.8f, 0.8f));
            }
        }
    }
}
