using BasicTD.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Paths;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace BasicTD.Scenes;

public class MultiArcScene : BattleScene
{
    private LinkedPath PathCollection;
    private List<Creep> TorchCreepList;
    public MultiArcScene() : base()
    {

    }

    public override void Initialize()
    {
        // NOTE: Above this line, content has not been loaded yet.
        base.Initialize();
        // NOTE: Content has been loaded after this line

        // Scene management
        NextScene = new LinkedArcScene();
    }

    public override void InitializePath()
    {
        // Load the path from the XML file
        Path = LinkedPath.FromFile(Core.Content, "paths/multi-path.xml");
        PathCollection = (LinkedPath)Path;

        // Create the creeps
        TorchCreepList = new List<Creep>();
        foreach (var path in PathCollection.Paths)
        {
            TorchCreepList.Add(new Creep(path, CreepSpeed, TorchSprite));
            path.LoadSprites(Atlas);
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        if (!Paused)
        {
            // Update the creeps
            foreach (var TorchCreep in TorchCreepList)
            {
                TorchCreep.Update(gameTime);
            }
        }

        if (PlacingTower)
        {
            Vector2 mousePos = Core.Input.Mouse.Position.ToVector2();
            Hitbox TowerBox = new Hitbox(
                mousePos, (int)(TowerSprite.Width * 0.5f)
            );

            TowerPlacementValid = true;
            foreach (var path in PathCollection.Paths)
            {
                if (path.HasCollided(TowerBox))
                {
                    TowerPlacementValid = false;
                    break;
                }
            }

            if (Core.Input.Mouse.WasButtonJustPressed(MouseButton.Left) && TowerPlacementValid)
            {
                Towers.Add(new TestTower(mousePos, TowerSprite));
                PlacingTower = false;
            }
        }

        foreach (var tower in Towers)
        {
            tower.Update(gameTime);
            List<Creep> creepsInRange = tower.CreepsInRange(TorchCreepList);
            foreach (var creep in creepsInRange)
            {
                {
                    tower.Attack(creep);
                }
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);
        DrawMarkers(gameTime);
        DrawPlacedTowers(gameTime);
        DrawPlacingTower(gameTime);
    }

    public void DrawMarkers(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: Grayscale);

        foreach (var path in PathCollection.Paths)
        {
            path.Draw(Core.SpriteBatch, WhitePixel);

            Vector2 StartingPosition = path.StartingPoint;
            Vector2 EndingPosition = path.EndingPoint;
            List<Vector2> ControlPoints = path.ControlPoints;

            StartMarker.Draw(Core.SpriteBatch, StartingPosition);
            EndMarker.Draw(Core.SpriteBatch, EndingPosition);
            foreach (var controlPoint in ControlPoints)
            {
                ControlPointMarker.Draw(Core.SpriteBatch, controlPoint);
            }
        }

        foreach (var TorchCreep in TorchCreepList)
        {
            TorchCreep.Draw(Core.SpriteBatch, WhitePixel, DebugDraw);
        }

        Core.SpriteBatch.End();

    }
}