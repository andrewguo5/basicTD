using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using System.Collections.Generic;
using MonoGameLibrary.Paths;
using System;

namespace BasicTD.Scenes;

public class BasicMapScene : BattleScene
{
    Tilemap Tilemap;

    // Wall sprites
    TextureAtlas WallFloorAtlas;
    Sprite VerticalWallTop;
    Sprite VerticalWallMid;
    Sprite VerticalWallBot;

    public BasicMapScene() : base()
    {
        MapBounds = new Rectangle(
            240, 100,
            720, 420
        );
    }

    public override void Initialize()
    {
        // NOTE: Above this line, content has not been loaded yet.
        base.Initialize();
        // NOTE: Content has been loaded after this line

        // Scene management
        NextScene = new LineScene();
    }

    public override void InitializePath()
    {
        // Load the path from the XML file
        Path = LinkedPath.FromFile(
            Core.Content,
            "paths/beginner-map.xml",
            MapBounds.Location.ToVector2(),
            0.8f
        );
        Path.LoadSprites(Atlas);
    }

    public override void LoadContent()
    {
        base.LoadContent();

        Tilemap = Tilemap.FromFile(Core.Content, "images/tilemap-definition.xml");
        Tilemap.Scale = new Vector2(60f/16f, 60f/16f);

        WallFloorAtlas = TextureAtlas.FromFile(Core.Content, "images/walls_floor_atlas.xml");
        VerticalWallTop = WallFloorAtlas.CreateSprite("vertical-wall-top");
        VerticalWallMid = WallFloorAtlas.CreateSprite("vertical-wall-mid");
        VerticalWallBot = WallFloorAtlas.CreateSprite("vertical-wall-bot");

        VerticalWallTop.Scale = new Vector2(3f, 3f);
        VerticalWallMid.Scale = new Vector2(3f, 3f);
        VerticalWallBot.Scale = new Vector2(3f, 3f);
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        UpdateCreep(gameTime);
        UpdatePlacingTower(gameTime);
        UpdateTowers(gameTime);
        UpdateCreepList(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(92, 105, 127));

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Vector2 tilemapOffset = MapBounds.Location.ToVector2();
        Tilemap.Draw(Core.SpriteBatch, tilemapOffset);
        Core.SpriteBatch.End();

        DrawPath(gameTime);
        // DrawMarkers(gameTime);
        DrawCreeps(gameTime);
        DrawPlacedTowers(gameTime);
        DrawPlacingTower(gameTime);
        DrawSelectedTower(gameTime);


        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        // First batch
        int _y = MapBounds.Top;
        VerticalWallTop.Draw(Core.SpriteBatch, new Vector2(20, _y));
        VerticalWallTop.Draw(Core.SpriteBatch, new Vector2(1180 - VerticalWallTop.Width, _y));
        _y += (int)VerticalWallTop.Height;

        // Second batch
        VerticalWallMid.Draw(Core.SpriteBatch, new Vector2(20, _y));
        VerticalWallMid.Draw(Core.SpriteBatch, new Vector2(1180 - VerticalWallMid.Width, _y));
        _y += (int)VerticalWallMid.Height;

        // Third batch
        VerticalWallBot.Draw(Core.SpriteBatch, new Vector2(20, _y));
        VerticalWallBot.Draw(Core.SpriteBatch, new Vector2(1180 - VerticalWallBot.Width, _y));

        Core.SpriteBatch.End();

        // Draw the scaffold for debugging
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Top UI boundary: Separates the info UI from the map
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, MapBounds.Top);

        // Top UI subsection: Separates the top bar from the relic bar
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, 60);

        // Bottom player UI boundary: Separates the map from the player UI
        Core.Scaffold.DrawHorizontalLineAtY(Core.SpriteBatch, MapBounds.Bottom);

        // Left and right map boundaries
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Left);
        Core.Scaffold.DrawVerticalLineAtX(Core.SpriteBatch, MapBounds.Right);
        Core.SpriteBatch.End();
    }

    public void DrawMarkers(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: Grayscale);

        Vector2 StartingPosition = Path.StartingPoint;
        Vector2 EndingPosition = Path.EndingPoint;
        List<Vector2> ControlPoints = Path.ControlPoints;

        StartMarker.Draw(Core.SpriteBatch, StartingPosition);
        EndMarker.Draw(Core.SpriteBatch, EndingPosition);
        foreach (var controlPoint in ControlPoints)
        {
            ControlPointMarker.Draw(Core.SpriteBatch, controlPoint);
        }

        Core.SpriteBatch.End();
    }
}