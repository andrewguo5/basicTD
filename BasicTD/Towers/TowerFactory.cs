using System;
using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using System.Collections.Generic;

namespace BasicTD.Towers;

public enum TowerType
{
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
    private TextureAtlas CardAtlas;
    private Vector2 SpriteScale;

    public TowerFactory(Vector2 spriteScale)
    {
        TowerAtlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");
        CardAtlas = TextureAtlas.FromFile(Core.Content, "images/card-shop-atlas.xml");
        SpriteScale = spriteScale;
    }

    public Tower CreateTower(Vector2 position, TowerType towerType)
    {
        SpriteStack towerSprite = CreateTowerSprite(towerType);
        switch (towerType)
        {
            case TowerType.Light:
                return new LightTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            case TowerType.Heavy:
                return new HeavyTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            case TowerType.Pulse:
                return new PulseTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            case TowerType.Shockwave:
                return new ShockwaveTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            case TowerType.Beacon:
                return new BeaconTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            case TowerType.Vuln:
                return new VulnTower(position, towerSprite, LoadActivationAnimation()); // TODO: refactor
            default:
                throw new ArgumentException($"Unknown tower type: {towerType}");
        }
    }

    public SpriteStack CreateTowerSprite(TowerType towerType)
    {
        var baseSprite = towerType switch
        {
            TowerType.Light => TowerAtlas.CreateSprite("minibase"),
            TowerType.Heavy => TowerAtlas.CreateSprite("minibase"),
            TowerType.Pulse => TowerAtlas.CreateSprite("minibase"),
            TowerType.Shockwave => TowerAtlas.CreateSprite("minibase"),
            TowerType.Vuln => TowerAtlas.CreateSprite("minibase"),
            TowerType.Beacon => TowerAtlas.CreateSprite("minibase"),
            _ => throw new ArgumentException($"Unknown tower type: {towerType}")
        };
        baseSprite.CenterOrigin();
        baseSprite.Scale = SpriteScale;

        var headSprite = towerType switch
        {
            TowerType.Light => TowerAtlas.CreateSprite("mini-card-common"),
            TowerType.Heavy => TowerAtlas.CreateSprite("mini-card-uncommon"),
            TowerType.Pulse => TowerAtlas.CreateSprite("mini-card-rare"),
            TowerType.Shockwave => TowerAtlas.CreateSprite("mini-card-epic"),
            TowerType.Vuln => TowerAtlas.CreateSprite("mini-card-legendary"),
            TowerType.Beacon => TowerAtlas.CreateSprite("mini-card-legendary"),
            _ => throw new ArgumentException($"Unknown tower type: {towerType}")
        };
        headSprite.CenterOrigin();
        headSprite.Scale = SpriteScale;

        return new SpriteStack(new List<Sprite> { baseSprite, headSprite });
    }

    public AnimatedSprite LoadActivationAnimation()
    {
        AnimatedSprite activationAnimation = TowerAtlas.CreateAnimatedSprite("activation-animation");
        activationAnimation.Origin = new Vector2(8, 8);
        activationAnimation.Scale = SpriteScale;
        activationAnimation.Repeat = false;
        return activationAnimation;
    }

    public SpriteStack CreateCardIcon(TowerType towerType)
    {
        Sprite emblemSprite = towerType switch
        {
            TowerType.Light => CardAtlas.CreateSprite("emblem-bullet"),
            TowerType.Heavy => CardAtlas.CreateSprite("emblem-bullet"),
            TowerType.Pulse => CardAtlas.CreateSprite("emblem-shield"),
            TowerType.Shockwave => CardAtlas.CreateSprite("emblem-shield"),
            TowerType.Vuln => CardAtlas.CreateSprite("emblem-gem"),
            TowerType.Beacon => CardAtlas.CreateSprite("emblem-gem"),
            _ => throw new ArgumentException($"Unknown tower type: {towerType}")
        };
        emblemSprite.Scale = new Vector2(2.0f, 2.0f);

        Sprite symbolSprite = towerType switch
        {
            TowerType.Light => CardAtlas.CreateSprite("symbol-loop"),
            TowerType.Heavy => CardAtlas.CreateSprite("symbol-square"),
            TowerType.Pulse => CardAtlas.CreateSprite("symbol-pulse"),
            TowerType.Shockwave => CardAtlas.CreateSprite("symbol-shockwave"),
            TowerType.Vuln => CardAtlas.CreateSprite("symbol-spare"),
            TowerType.Beacon => CardAtlas.CreateSprite("symbol-oval"),
            _ => throw new ArgumentException($"Unknown tower type: {towerType}")
        };
        symbolSprite.Scale = new Vector2(2.0f, 2.0f);

        return new SpriteStack(new List<Sprite> { emblemSprite, symbolSprite });
    }
}
