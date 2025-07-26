using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace BasicTD.Towers;
public abstract class BaseTower
{
    public Vector2 Position { get; set; }
    public Sprite Sprite { get; set; }
    protected abstract string SpriteName { get; }

    public BaseTower(Vector2 position, TextureAtlas atlas)
    {
        Position = position;
        Sprite = atlas.CreateSprite(SpriteName);
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
