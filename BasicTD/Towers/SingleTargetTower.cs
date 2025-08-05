using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Creeps;
using System.Collections.Generic;

namespace BasicTD.Towers
{
    public class SingleTargetTower : Tower
    {
        public override float AttackSpeed { get; } = 1.2f; // 1 attack per second
        public override int Damage { get; } = 1; // Example damage value
        public override float Range { get; } = 200.0f; // Example range
        public override int TowerId { get; } = -1; // Unique identifier for the

        public SingleTargetTower(
            Vector2 position, Sprite sprite, AnimatedSprite attackAnimation) : base(position, sprite, attackAnimation)
        {
            // Additional initialization if needed
        }

        public override void Attack(List<Creep> creepList)
        {
            List<Creep> creepsInRange = CreepsInRange(creepList);
            foreach (var creep in creepsInRange)
            {
                {
                    AttackOne(creep);
                }
            }
        }
        
        public void AttackOne(Creep creep)
        {
            // Logic to attack the creep, e.g., reduce its health
            // This method can be overridden in derived classes for specific attack behavior
            if (AttackCooldown > 0f)
                return; // Cannot attack yet

            float delaySeconds = AttackAnimation.AnimationTime;
            creep.TakeDamage(Damage, delaySeconds);
            float angle = (float)Math.Atan2(creep.CurrentPosition.Y - Position.Y, creep.CurrentPosition.X - Position.X);
            AttackAnimation.Rotation = angle;
            AttackAnimation.Play();

            // Reset attack cooldown
            AttackCooldown = 1f / AttackSpeed;
        }

    }
}