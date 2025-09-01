using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Creeps;
using System.Collections.Generic;


namespace BasicTD.Towers
{
    public class ShockwaveTower : Tower
    {
        private static TowerInfo.TowerStat TowerStat;
        public override TowerInfo.TowerStatLine TowerStatLine => TowerStat.GetStatsForLevel(Level);
        public override int TowerId => TowerStat.Id;

        static ShockwaveTower()
        {
            TowerStat = TowerInfo.GetTowerStat(TowerType.Shockwave);
        }

        public ShockwaveTower(
            Vector2 position, SpriteStack sprite, AnimatedSprite attackAnimation, int level) : base(position, sprite, attackAnimation, level)
        {
            // Additional initialization if needed
        }

        public override void Attack(List<Creep> creepList)
        {
            List<Creep> creepsInRange = CreepsInRange(creepList);
            foreach (var creep in creepsInRange)
            {
                {
                    if (creep.Alive)
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
            AttackCooldown = 1f / Speed;
        }

    }
}