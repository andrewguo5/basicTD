using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace BasicTD.Towers
{
    public class TestTower : Tower
    {
        public override float AttackSpeed { get; } = 1.2f; // 1 attack per second
        public override int Damage { get; } = 10; // Example damage value
        public override float Range { get; } = 200.0f; // Example range
        public override int TowerId { get; } = -1; // Unique identifier for the

        public TestTower(Vector2 position, Sprite sprite) : base(position, sprite)
        {
            // Additional initialization if needed
        }
    }
}