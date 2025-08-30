using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;

public class SpriteStack
{
    public List<Sprite> Sprites { get; } = new List<Sprite>();
    public Vector2 Position { get; set; }
    public Color Color { get; set; } = Color.White;

    public SpriteStack()
    {
    }

    public SpriteStack(List<Sprite> sprites)
    {
        if (sprites != null)
            Sprites.AddRange(sprites);
    }

    public void AddSprite(Sprite sprite)
    {
        if (sprite != null)
            Sprites.Add(sprite);
    }

    public void CenterOrigin()
    {
        foreach (var sprite in Sprites)
        {
            sprite.CenterOrigin();
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        foreach (var sprite in Sprites)
        {
            sprite.Draw(spriteBatch, position, Color);
        }
    }
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        foreach (var sprite in Sprites)
        {
            sprite.Draw(spriteBatch, position, color);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale)
    {
        foreach (var sprite in Sprites)
        {
            sprite.Draw(spriteBatch, position, color, scale);
        }
    }

}
