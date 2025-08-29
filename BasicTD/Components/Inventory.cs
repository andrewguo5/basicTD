using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;
using BasicTD.Scenes;
using BasicTD.Towers;
using System.Threading;

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

    // Contents
    private List<Card> Cards;
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
                MapBounds.Right + SideBuffer + Padding,
                MapBounds.Top + VerticalOffset + Padding + (CardPlacards.Count * (60 + Padding)),
                Bounds.Width - 2 * SideBuffer - 2 * Padding,
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
        UpdateFocusedPlacard();
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
        
        DrawPlacards();
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
    private Card Card { get; }
    private int Quantity;
    private TowerInfo TowerInfo;
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

        // Draw card background
        Core.Scaffold.DrawFilledRectangle(Core.SpriteBatch, Bounds, TDConstants.LightBG);

        var imageRect = new Rectangle(Bounds.X + Padding, Bounds.Y + Padding, Bounds.Height - 2 * Padding, Bounds.Height - 2 * Padding);
        Color highlightColor = focused ? Color.White : Color.LightGray;
        float heightRatio = (float)imageRect.Height / Card.CardSprite().Height;
        Vector2 scale = new Vector2(heightRatio, heightRatio);
        Card.Draw(imageRect.Location.ToVector2(), highlightColor: highlightColor, scale: scale);
        // Draw card name
        var namePos = new Vector2(imageRect.Right + Padding, Bounds.Y + Padding);
        string nick = TowerInfo.GetTowerStat(Card.TowerType).Nick;
        Core.SpriteBatch.DrawString(gameFont, nick, namePos, Color.White);

        // Draw quantity
        Vector2 stringSize = gameFont.MeasureString($"x{Quantity}");
        var qtyPos = new Vector2(Bounds.Right - Padding - stringSize.Y, Bounds.Y + Padding);
        Core.SpriteBatch.DrawString(gameFont, $"x{Quantity}", qtyPos, Color.White);

        Core.SpriteBatch.End();
    }
}