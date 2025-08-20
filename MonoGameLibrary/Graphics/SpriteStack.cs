using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;

public class SpriteStack
{
    public List<Sprite> Sprites { get; } = new List<Sprite>();
    public Vector2 Position { get; set; }
    public Color Color { get; set; } = Color.White;

    public SpriteStack(Vector2 position)
    {
        Position = position;
    }

    public SpriteStack(Vector2 position, List<Sprite> sprites)
    {
        Position = position;
        if (sprites != null)
            Sprites.AddRange(sprites);
    }

    public void AddSprite(Sprite sprite)
    {
        if (sprite != null)
            Sprites.Add(sprite);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var sprite in Sprites)
        {
            sprite.Draw(spriteBatch, Position, Color);
        }
    }
}
