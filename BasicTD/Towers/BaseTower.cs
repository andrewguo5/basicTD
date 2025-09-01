using System;
using System.Collections.Generic;
using BasicTD.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Graphics;

namespace BasicTD.Towers;

public abstract class Tower
{
    // Represents a placed tower
    // See also: TowerCard
    public Vector2 Position { get; set; }
    public SpriteStack Sprite { get; set; }
    public int Level;
    public abstract TowerInfo.TowerStatLine TowerStatLine { get; }
    public abstract int TowerId { get; }
    public virtual float Range => TowerStatLine.Range * TDConstants.PixelsPerMeter;
    public virtual int Damage => TowerStatLine.Damage;
    public virtual float Speed => TowerStatLine.Speed;
    public float AttackCooldown = 0f;
    private int TowerBoxRadius = 16;
    private Hitbox TowerBox;
    public AnimatedSprite AttackAnimation;
    public static TowerInfo TowerInfo;

    static Tower()
    {
        TowerInfo = TowerInfo.FromJsonFile(Core.Content, "tower-info.json");
    }

    public Tower(Vector2 position, SpriteStack sprite, AnimatedSprite attackAnimation, int level = 1)
    {
        Position = position;
        Sprite = sprite;
        TowerBox = new Hitbox(Position, TowerBoxRadius);
        AttackAnimation = attackAnimation;
        Level = level;
    }

    public void Update(GameTime gameTime)
    {
        // Reduce attack cooldown
        AttackCooldown = Math.Max(0f, AttackCooldown - (float)gameTime.ElapsedGameTime.TotalSeconds);
        AttackAnimation.Update(gameTime);
    }

    public virtual void Attack(List<Creep> creepList)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch, Color color)
    {
        if (Sprite != null)
            Sprite.Draw(spriteBatch, Position, color);

        if (AttackAnimation != null)
            AttackAnimation.Draw(spriteBatch, Position);
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
            if (IsCreepInRange(creep.CurrentPosition) && creep.Alive)
            {
                creepsInRange.Add(creep);
            }
        }
        return creepsInRange;
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
