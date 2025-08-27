using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;
using System;
using System.Linq;
using BasicTD.Scenes;
using BasicTD.Towers;

namespace BasicTD.Components;

public class Shop : GComponent
{
    // Props
    private int VerticalOffset => Props["VerticalOffset"];
    private int Padding => Props["TextPadding"];
    private int SideBuffer => Props["SideBuffer"];
    private TextureAtlas Atlas => Props["Atlas"];
    private TextureAtlas CardAtlas => Props["CardAtlas"];
    private SpriteFont GameFont => Props["GameFont"];
    private Rectangle MapBounds => Props["MapBounds"];
    private Vector2 SpriteScale => Props["SpriteScale"];

    // Component-specific content 
    private Vector2 ShopStringLocation;
    private List<CardSlot> CardSlotManager { get; set; }
    private CardSlot CardSlot0;
    private CardSlot CardSlot1;
    private CardSlot CardSlot2;
    private CardSlot CardSlot3;
    private CardSlot CardSlot4;
    private CardSlot CardSlot5;
    private int hoveredCardSlotIndex = -1;
    private List<Sprite> CardSpriteManager;
    private List<Sprite> CardEmblemSprites;
    private List<Sprite> CardSymbolSprites;
    private Sprite GoldSprite;

    // Shop generation
    private TowerFactory TowerFactory;
    Random random;
    private int RandomSymbolIndex;
    // Player properties
    private Player Player => ((GameScene)ParentScene).Player;

    public Shop(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
        int X = Bounds.Left + 20;
        int Y = Bounds.Top + 15;
        int padding = 40;

        CardSlot0 = new(CardAtlas, new Rectangle(X, Y, 80, 132), 0);
        CardSlot1 = new(CardAtlas, new Rectangle(CardSlot0.Bounds.Right + padding, Y, 80, 132), 1);
        CardSlot2 = new(CardAtlas, new Rectangle(CardSlot1.Bounds.Right + padding, Y, 80, 132), 1);
        CardSlot3 = new(CardAtlas, new Rectangle(CardSlot2.Bounds.Right + padding, Y, 80, 132), 2);
        CardSlot4 = new(CardAtlas, new Rectangle(CardSlot3.Bounds.Right + padding, Y, 80, 132), 3);
        CardSlot5 = new(CardAtlas, new Rectangle(CardSlot4.Bounds.Right + padding, Y, 80, 132), 4);

        TowerFactory = new(SpriteScale);
    }

    protected override void InitializeSelf()
    {
        random = new Random();
        RandomSymbolIndex = random.Next(CardSymbolSprites.Count);

        CardSlotManager = new List<CardSlot>
        {
            CardSlot0,
            CardSlot1,
            CardSlot2,
            CardSlot3,
            CardSlot4,
            CardSlot5
        };

        ShopStringLocation = new Vector2(
            Padding + SideBuffer,
            MapBounds.Bottom + 2 * VerticalOffset
        );

        GenerateCards();
    }

    private void GenerateCards()
    {
        foreach (var slot in CardSlotManager)
        {
            slot.GenerateCard();
        }
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

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Reset scene
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Reset();
        }

        // Update logic for the shop can be added here if needed
        Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
        foreach (var slot in CardSlotManager)
        {
            if (slot.Bounds.Contains(mousePos))
            {
                // If the mouse is over a card slot, set the hovered index
                hoveredCardSlotIndex = CardSlotManager.IndexOf(slot);
                return;
            }
        }
        // If the mouse is not over any card slot, reset the hovered index
        hoveredCardSlotIndex = -1;
    }

    public void Reset()
    {
        GenerateCards();
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw banner
        Core.Scaffold.DrawFilledRectangle(
            Core.SpriteBatch,
            new Rectangle(0, MapBounds.Bottom + 40, Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height - (MapBounds.Bottom + 40)),
            TDConstants.DarkBG
        );

        DrawCards();
        DrawCardCosts();
        DrawText();

        if (((GameScene)ParentScene).DebugDraw)
        {
            foreach (var slot in CardSlotManager)
            {
                // Draw the card slots
                Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, slot.Bounds, 1);
            }
        }
        Core.SpriteBatch.End();
    }

    private void DrawCards()
    {
        // Draw each card at its corresponding card slot location
        for (int i = 0; i < CardSlotManager.Count; i++)
        {
            CardSlot slot = CardSlotManager[i];
            Rectangle slotRect = slot.Bounds;

            Color highlightColor = (hoveredCardSlotIndex == i) ? Color.White : Color.LightGray;
            slot.Card.Draw(new Vector2(slotRect.X, slotRect.Y), highlightColor);

            SpriteStack towerIcon = TowerFactory.CreateCardIcon(slot.Card.TowerType);
            towerIcon.Draw(Core.SpriteBatch, new Vector2(slotRect.X, slotRect.Y), highlightColor);
        }
    }

    private void DrawCardCosts()
    {
        // Draw the cost of each card in its corresponding slot
        for (int i = 0; i < CardSlotManager.Count; i++)
        {
            CardSlot slot = CardSlotManager[i];
            Rectangle slotRect = slot.Bounds;
            Vector2 costPosition = new Vector2(slotRect.X + slotRect.Width / 2, slotRect.Bottom - 18);

            Color highlightColor = (hoveredCardSlotIndex == i) ? Color.White : Color.LightGray;
            int cardCost = slot.Card.Cost;

            Core.SpriteBatch.DrawString(
                GameFont,
                $"{cardCost}",
                costPosition,
                highlightColor,
                0f,
                Vector2.Zero,
                1f,
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

public class CardSlot
{
    private TextureAtlas CardAtlas;
    public Rectangle Bounds { get; set; }
    public int Index { get; set; }
    public Card Card { get; set; }

    public CardSlot(TextureAtlas cardAtlas, Rectangle bounds, int index, Card card)
    {
        CardAtlas = cardAtlas;
        Bounds = bounds;
        Index = index;
        Card = card;
    }

    public CardSlot(TextureAtlas cardAtlas, Rectangle bounds, int index, int level = 0)
    {
        CardAtlas = cardAtlas;
        Bounds = bounds;
        Index = index;
        GenerateCard(level);
    }

    public void GenerateCard(int level = 0)
    {
        Card = new Card(CardAtlas, level);
    }
}

public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Null
}

public class Card
{
    private TextureAtlas CardAtlas;
    public TowerType TowerType { get; set; }
    public CardRarity Rarity { get; set; }
    public int Cost => GetCost();

    public Card(TextureAtlas cardAtlas, TowerType towerType, CardRarity rarity, int cost)
    {
        CardAtlas = cardAtlas;
        TowerType = towerType;
        Rarity = rarity;
    }

    public Card(TextureAtlas cardAtlas, int level = 0)
    {
        CardAtlas = cardAtlas;
        // Level affects the rarity distribution. For now we will just generate
        // random cards regardless of level.
        var rarities = Enum.GetValues(typeof(CardRarity)).Cast<CardRarity>().Where(r => r != CardRarity.Null).ToArray();
        var towerTypes = Enum.GetValues(typeof(TowerType)).Cast<TowerType>().ToArray();
        var rand = new Random();

        Rarity = rarities[rand.Next(rarities.Length)];
        TowerType = towerTypes[rand.Next(towerTypes.Length)];
    }

    public int GetCost()
    {
        int rarityCost = Rarity switch
        {
            CardRarity.Common => 1,
            CardRarity.Uncommon => 2,
            CardRarity.Rare => 3,
            CardRarity.Epic => 4,
            CardRarity.Legendary => 5,
            CardRarity.Null => 0,
            _ => 0
        };

        return rarityCost;
    }

    public Sprite CardSprite()
    {
        Sprite cardSprite = Rarity switch
        {
            CardRarity.Common => CardAtlas.CreateSprite("card-common"),
            CardRarity.Uncommon => CardAtlas.CreateSprite("card-uncommon"),
            CardRarity.Rare => CardAtlas.CreateSprite("card-rare"),
            CardRarity.Epic => CardAtlas.CreateSprite("card-epic"),
            CardRarity.Legendary => CardAtlas.CreateSprite("card-legendary"),
            CardRarity.Null => CardAtlas.CreateSprite("card-null"),
            _ => throw new ArgumentException($"Unknown rarity: {Rarity}")
        };
        cardSprite.Scale = new Vector2(2.0f, 2.0f);

        return cardSprite;
    }

    public void Draw(Vector2 position, Color highlightColor)
    {
        Sprite cardSprite = CardSprite();
        cardSprite.Draw(Core.SpriteBatch, position, highlightColor);
    }
}