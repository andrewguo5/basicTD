using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Creeps;
using System.Collections.Generic;
using MonoGameLibrary;
using Microsoft.VisualBasic;

namespace BasicTD.Towers
{
    public class BasicTower : Tower
    {
        public static Dictionary<string, float> Stats => TowerStats.AllTowerStats[TowerType.Basic];
        public override float AttackSpeed => Stats["AttackSpeed"];
        public override int Damage => (int)Stats["Damage"];
        public override float Range => Stats["Range"] * TDConstants.PixelsPerMeter;
        public override int TowerId { get; } = -1; // Unique identifier for the

        public BasicTower(
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
            // float angle = (float)Math.Atan2(creep.CurrentPosition.Y - Position.Y, creep.CurrentPosition.X - Position.X);
            // AttackAnimation.Rotation = angle;
            AttackAnimation.Play();

            // Reset attack cooldown
            AttackCooldown = 1f / AttackSpeed;
        }

    }
}