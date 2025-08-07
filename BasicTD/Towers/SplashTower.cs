using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Creeps;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BasicTD.Towers
{
    public class SplashTower : Tower
    {
        public TowerType towerType => TowerType.Splash;
        public override float AttackSpeed { get; } = 2f; // 1 attack per second
        public override int Damage { get; } = 1; // Example damage value
        public override float Range { get; } = 100.0f; // Example range
        public override int TowerId { get; } = -1; // Unique identifier for the

        public SplashTower(
            Vector2 position, Sprite sprite, AnimatedSprite attackAnimation) : base(position, sprite, attackAnimation)
        {
            // Additional initialization if needed
        }

        public override void Attack(List<Creep> creepList)
        {
            // Logic to attack the creep, e.g., reduce its health
            // This method can be overridden in derived classes for specific attack behavior
            if (AttackCooldown > 0f || WindupTime > 0f)
                return; // Cannot attack yet

            float delaySeconds = AttackAnimation.AnimationTime * 0.9f;

            if (State == "neutral")
            {
                if (CreepsInRange(creepList).Count > 0)
                {
                    State = "attacking";
                    AttackAnimation.Play();
                    WindupTime = delaySeconds;
                }
            }

            if (State == "attacking")
            {
                List<Creep> creepsInRange = CreepsInRange(creepList);
                foreach (var creep in creepsInRange)
                {
                    creep.TakeDamage(Damage, 0f);
                }
                State = "neutral";
            }

            AttackCooldown = 1f / AttackSpeed + AttackAnimation.AnimationTime;
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (Sprite != null)
                Sprite.Draw(spriteBatch, Position, color, 0f);

            if (AttackAnimation != null)
                AttackAnimation.Draw(spriteBatch, Position + new Vector2(5, Sprite.Height * 0.5f));
        }
    }
}