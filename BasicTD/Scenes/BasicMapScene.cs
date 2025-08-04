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
    public BasicMapScene() : base()
    {
        MapBounds = new Rectangle(
            200, 72,
            800, 360
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
        Path = LinkedPath.FromFile(Core.Content, "paths/beginner-map.xml", MapBounds.Location.ToVector2());
        Path.LoadSprites(Atlas);
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        UpdateCreep(gameTime);
        UpdatePlacingTower(gameTime);
        UpdateTowers(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);

        DrawPath(gameTime);
        DrawMarkers(gameTime);
        DrawPlacedTowers(gameTime);
        DrawPlacingTower(gameTime);
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

        TorchCreep.Draw(Core.SpriteBatch, WhitePixel, DebugDraw);

        Core.SpriteBatch.End();
    }
}