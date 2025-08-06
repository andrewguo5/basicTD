using System;
using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;

namespace BasicTD.Towers;

public enum TowerType
{
    SingleTarget,
    Aoe,
}

public class TowerFactory
{
    private TextureAtlas TowerAtlas;
    private TextureAtlas AttackAnimationAtlas;
    private TextureAtlas Water4Atlas;

    public TowerFactory(TextureAtlas towerAtlas, TextureAtlas attackAnimationAtlas, TextureAtlas water4Atlas)
    {
        TowerAtlas = towerAtlas;
        AttackAnimationAtlas = attackAnimationAtlas;
        Water4Atlas = water4Atlas;
    }

    public Tower CreateTower(Vector2 position, TowerType towerType)
    {
        Sprite towerSprite;
        switch (towerType)
        {
            case TowerType.SingleTarget:
                towerSprite = TowerAtlas.CreateSprite("lever-blue");
                towerSprite.CenterOrigin();
                towerSprite.Scale = new Vector2(3f, 3f);
                return new SingleTargetTower(position, towerSprite, LoadSplashAnimation());
            case TowerType.Aoe:
                towerSprite = TowerAtlas.CreateSprite("lever-green");
                towerSprite.CenterOrigin();
                towerSprite.Scale = new Vector2(3f, 3f);
                return new AoeTower(position, towerSprite, LoadWater4());
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
        return Water4Animation;
    }
}
