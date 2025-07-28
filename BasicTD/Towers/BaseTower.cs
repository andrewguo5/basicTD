using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace BasicTD.Towers;
public abstract class Tower
{
    public Vector2 Position { get; set; }
    public Sprite Sprite { get; set; }
    protected abstract string SpriteName { get; }
    public abstract int TowerId { get; }
    public abstract float Range { get; } // in units? m
    public abstract int Damage { get; } // Let's just have int damage
    public abstract float AttackSpeed { get; } // in hertz


    public Tower(Vector2 position, Sprite sprite)
    {
        Position = position;
        Sprite = sprite;
    }

    public void Update(GameTime gameTime)
    {
        // Update logic for the tower can be added here
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Sprite != null)
            Sprite.Draw(spriteBatch, Position, Color.White, 0f);
    }
}
