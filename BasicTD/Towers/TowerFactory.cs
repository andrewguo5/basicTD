using System;
using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;

namespace BasicTD.Towers;

public enum TowerType
{
    Basic,
    Splash,
    Sniper,
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
        Sprite towerSprite;
        switch (towerType)
        {
            case TowerType.Basic:
                towerSprite = TowerAtlas.CreateSprite("lever-blue");
                towerSprite.CenterOrigin();
                towerSprite.Scale = SpriteScale;
                return new BasicTower(position, towerSprite, LoadSplashAnimation());
            case TowerType.Splash:
                towerSprite = TowerAtlas.CreateSprite("lever-green");
                towerSprite.CenterOrigin();
                towerSprite.Scale = SpriteScale;
                return new SplashTower(position, towerSprite, LoadWater4());
            default:
                throw new ArgumentException($"Unknown tower type: {towerType}");
        }
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
