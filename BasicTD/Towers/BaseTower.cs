using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Graphics;

namespace BasicTD.Towers;

public abstract class Tower
{
    public Vector2 Position { get; set; }
    public Sprite Sprite { get; set; }
    public abstract int TowerId { get; }
    public abstract float Range { get; } // in units? m
    public abstract int Damage { get; } // Let's just have int damage
    public abstract float AttackSpeed { get; } // in hertz
    private float attackCooldown = 0f; // in seconds


    public Tower(Vector2 position, Sprite sprite)
    {
        Position = position;
        Sprite = sprite;
    }

    public void Update(GameTime gameTime)
    {
        // Reduce attack cooldown
        attackCooldown = Math.Max(0f, attackCooldown - (float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Sprite != null)
            Sprite.Draw(spriteBatch, Position, Color.White, 0f);
    }

    public bool IsCreepInRange(Vector2 creepPosition)
    {
        float distance = Vector2.Distance(Position, creepPosition);
        return distance <= Range;
    }

    public List<Creep> CreepsInRange(List<Creep> creeps)
    {
        List<Creep> creepsInRange = new List<Creep>();
        foreach (var creep in creeps)
        {
            if (IsCreepInRange(creep.CurrentPosition))
            {
                creepsInRange.Add(creep);
            }
        }
        return creepsInRange;
    }

    public void Attack(Creep creep)
    {
        // Logic to attack the creep, e.g., reduce its health
        // This method can be overridden in derived classes for specific attack behavior
        if (attackCooldown > 0f)
            return; // Cannot attack yet
            
        creep.TakeDamage(Damage);
        
        // Reset attack cooldown
        attackCooldown = 1f / AttackSpeed;
    }
}
