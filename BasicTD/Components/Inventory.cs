using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using BasicTD.Scenes;
using BasicTD.Towers;

namespace BasicTD.Components;

public class Inventory : GComponent
{
    private int VerticalOffset => Props["VerticalOffset"];
    private int Padding => Props["TextPadding"];
    private int SideBuffer => Props["SideBuffer"];
    private Rectangle MapBounds => Props["MapBounds"];
    private TextureAtlas Atlas => Props["Atlas"];
    private Vector2 InventoryStringLocation;
    private Vector2 SupplyStringLocation;
    private SpriteFont GameFont => Props["GameFont"];
    private Tilemap InfoPanelMap;

    // Contents
    public List<Card> Cards;
    private Stack<CardPlacard> CardPlacards;
    private CardPlacard FocusedPlacard;
    // private int CurrentPlacardHeight = 0;

    // Player properties
    private Player Player => ((GameScene)ParentScene).Player;

    public Inventory(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
    }

    protected override void InitializeSelf()
    {
        InventoryStringLocation = new Vector2(
            Padding + SideBuffer + MapBounds.Right,
            MapBounds.Top - VerticalOffset
        );
        SupplyStringLocation = new Vector2(
            Bounds.Left,
            Bounds.Bottom - VerticalOffset
        );

        Cards = new List<Card>();
        CardPlacards = new Stack<CardPlacard>();
        Player.SetInventory(this);
    }

    public void AddCard(Card card)
    {
        // Player calls this function
        Cards.Add(card);
        CardPlacards.Push(new CardPlacard(
            new Rectangle(
                Bounds.Left,
                Bounds.Top + (CardPlacards.Count * 60),
                Bounds.Width,
                60
            ),
            card,
            Player.TowerInfo
        ));
    }

    protected override void LoadContentSelf()
    {
        InfoPanelMap = Tilemap.FromFile(Core.Content, "images/side-banner-tilemap.xml");
        InfoPanelMap.Scale = Props["TilemapScale"];
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        // Reset scene
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Reset();
        }
        UpdateFocusedPlacard();
        UpdateClickPlacard();
    }

    public void Reset()
    {
        Cards.Clear();
        CardPlacards.Clear();
        FocusedPlacard = null;
    }


    private void UpdateFocusedPlacard()
    {
        foreach (var placard in CardPlacards)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
            if (placard.Bounds.Contains(mousePos))
            {
                FocusedPlacard = placard;
                return;
            }
        }
        FocusedPlacard = null;
    }

    private void UpdateClickPlacard()
    {
        if (FocusedPlacard != null && Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left))
        {
            // Place tower logic
            if (Cards.Contains(FocusedPlacard.Card))
            {
                ((GameScene)ParentScene).Battlefield.StartPlacingTower(FocusedPlacard.Card.TowerType, FocusedPlacard.Card.Level);
                Cards.Remove(FocusedPlacard.Card);
                CardPlacards = new Stack<CardPlacard>();
                for (int i = 0; i < Cards.Count; i++)
                {
                    CardPlacards.Push(new CardPlacard(
                        new Rectangle(
                            Bounds.Left,
                            Bounds.Top + (i * 60),
                            Bounds.Width,
                            60
                        ),
                        Cards[i],
                        Player.TowerInfo
                    ));
                }
                FocusedPlacard = null;
            }
        }
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

        Color supplyColor = Cards.Count >= 5 ? TDConstants.RedBG : Color.White;
        Core.SpriteBatch.DrawString(GameFont, $"{Cards.Count}/{5}", SupplyStringLocation, supplyColor);
        Core.SpriteBatch.End();

        DrawPlacards();

        if (((GameScene)Core.CurrentScene).DebugDraw)
        {
            Core.SpriteBatch.Begin();
            Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, Bounds, 2);
            Core.SpriteBatch.End();
        }
    }

    public void DrawPlacards()
    {
        foreach (var placard in CardPlacards)
        {
            if (placard == FocusedPlacard)
                placard.Draw(GameFont, true);
            else
                placard.Draw(GameFont, false);
        }
    }
}

public class CardPlacard
{
    public Rectangle Bounds { get; }
    public Card Card { get; }
    private int Quantity;
    private TowerInfo TowerInfo;
    private GameScene GameScene => (GameScene)Core.CurrentScene;
    private TowerFactory TowerFactory => GameScene.TowerFactory;

    public CardPlacard(Rectangle bounds, Card card, TowerInfo towerInfo)
    {
        Bounds = bounds;
        Card = card;
        Quantity = 1;
        TowerInfo = towerInfo;
    }

    public void Draw(SpriteFont gameFont, bool focused)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        int Padding = 2;

        var imageRect = new Rectangle(
            Bounds.X + Padding,
            Bounds.Y + Padding,
            (int)((Bounds.Height - 2 * Padding) * (5f / 7f)),
            Bounds.Height - 2 * Padding
        );

        // Draw card
        Color highlightColor = focused ? Color.White : Color.LightGray;
        float heightRatio = (float)imageRect.Height / Card.CardSprite().Height;
        Vector2 scale = new Vector2(heightRatio, heightRatio);
        Card.Draw(imageRect.Location.ToVector2(), highlightColor: highlightColor, scale: scale);

        // Draw icon
        SpriteStack towerIcon = TowerFactory.CreateCardIcon(Card.TowerType);
        Vector2 cardCenter = new Vector2(imageRect.Center.X, imageRect.Center.Y);
        towerIcon.CenterOrigin();
        towerIcon.Draw(Core.SpriteBatch, cardCenter, highlightColor, scale: Vector2.One * 0.9f);

        // Draw card name
        var namePos = new Vector2(imageRect.Right + Padding, Bounds.Y + Padding);
        string nick = TowerInfo.GetTowerStat(Card.TowerType).Nick;
        Core.SpriteBatch.DrawString(gameFont, nick, namePos, Color.White);

        // Draw quantity
        Vector2 stringSize = gameFont.MeasureString($"x{Quantity}");
        var qtyPos = new Vector2(Bounds.Right - Padding - stringSize.X, Bounds.Y + Padding + stringSize.Y);
        Core.SpriteBatch.DrawString(gameFont, $"x{Quantity}", qtyPos, Color.White);

        if (GameScene.DebugDraw)
            Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, Bounds, 1);

        Core.SpriteBatch.End();
    }
}