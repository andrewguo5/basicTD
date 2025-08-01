using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Collision;
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
    private float AttackCooldown = 0f; // in seconds
    private int TowerBoxRadius = 16;
    private Hitbox TowerBox;


    public Tower(Vector2 position, Sprite sprite)
    {
        Position = position;
        Sprite = sprite;
        TowerBox = new Hitbox(Position, TowerBoxRadius);
    }

    public void Update(GameTime gameTime)
    {
        // Reduce attack cooldown
        AttackCooldown = Math.Max(0f, AttackCooldown - (float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public void Draw(SpriteBatch spriteBatch, Color color)
    {
        if (Sprite != null)
            Sprite.Draw(spriteBatch, Position, color, 0f);
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
        if (AttackCooldown > 0f)
            return; // Cannot attack yet

        creep.TakeDamage(Damage);

        // Reset attack cooldown
        AttackCooldown = 1f / AttackSpeed;
    }

    public bool MouseCollision(Vector2 mousePos)
    {
        return Vector2.Distance(mousePos, Position) <= TowerBoxRadius;
    }

    public bool HasCollided(Hitbox hitbox)
    {
        return hitbox.HasCollided(TowerBox);
    }

    public bool HasCollided(Tower other)
    {
        return other.TowerBox.HasCollided(TowerBox);
    }
}
