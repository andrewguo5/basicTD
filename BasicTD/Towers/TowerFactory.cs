using System;
using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BasicTD.Towers;

public enum TowerType
{
    Basic,
    Splash,
    Sniper,
    Pulse,
    Shockwave,
    Light,
    Heavy,
    Beacon,
    Vuln,
}

public class TowerFactory
{
    private TextureAtlas TowerAtlas;
    private TextureAtlas AttackAnimationAtlas;
    private TextureAtlas Water4Atlas;
    private Vector2 SpriteScale;

    public TowerFactory(TextureAtlas towerAtlas, TextureAtlas attackAnimationAtlas, TextureAtlas water4Atlas, Vector2 spriteScale)
    {
        TowerAtlas = towerAtlas;
        AttackAnimationAtlas = attackAnimationAtlas;
        Water4Atlas = water4Atlas;
        SpriteScale = spriteScale;
    }

    public Tower CreateTower(Vector2 position, TowerType towerType)
    {
        SpriteStack towerSprite = CreateTowerSprite(towerType);
        switch (towerType)
        {
            case TowerType.Basic:
                return new BasicTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            case TowerType.Splash:
                return new SplashTower(position, towerSprite, LoadActivationAnimation());
            default:
                throw new ArgumentException($"Unknown tower type: {towerType}");
        }
    }

    public SpriteStack CreateTowerSprite(TowerType towerType)
    {
        var baseSprite = towerType switch
        {
            TowerType.Basic => TowerAtlas.CreateSprite("minibase"),
            TowerType.Splash => TowerAtlas.CreateSprite("minibase"),
            TowerType.Sniper => TowerAtlas.CreateSprite("minibase"),
            _ => throw new ArgumentException($"Unknown tower type: {towerType}")
        };
        baseSprite.CenterOrigin();
        baseSprite.Scale = SpriteScale;

        var headSprite = towerType switch
        {
            TowerType.Basic => TowerAtlas.CreateSprite("mini-card-common"),
            TowerType.Splash => TowerAtlas.CreateSprite("mini-card-uncommon"),
            _ => throw new ArgumentException($"Unknown tower type: {towerType}")
        };
        headSprite.CenterOrigin();
        headSprite.Scale = SpriteScale;

        return new SpriteStack(Vector2.Zero, new List<Sprite> { baseSprite, headSprite });
    }

    public AnimatedSprite LoadActivationAnimation()
    {
        AnimatedSprite activationAnimation = TowerAtlas.CreateAnimatedSprite("activation-animation");
        activationAnimation.Origin = new Vector2(8, 8);
        activationAnimation.Scale = SpriteScale;
        activationAnimation.Repeat = false;
        return activationAnimation;
    }

    public AnimatedSprite LoadSplashAnimation()
    {
        AnimatedSprite splashAnimation = AttackAnimationAtlas.CreateAnimatedSprite("splash1-animation");
        splashAnimation.Origin = new Vector2(0, splashAnimation.Height * 0.5f);
        splashAnimation.Scale = new Vector2(0.25f, 0.25f);
        splashAnimation.Repeat = false;
        return splashAnimation;
    }

    public AnimatedSprite LoadWater4()
    {
        AnimatedSprite Water4Animation = Water4Atlas.CreateAnimatedSprite("Water4");
        Water4Animation.Scale = new Vector2(0.3f, 0.3f);
        Water4Animation.Repeat = false;
        return Water4Animation;
    }
}
