using BasicTD.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary;
using System.Collections.Generic;
using MonoGameLibrary.Paths;
using MonoGameLibrary.Collision;

namespace BasicTD.Scenes;

public class LineScene : BattleScene
{
    // Splash atlas
    private TextureAtlas SplashAtlas;
    private AnimatedSprite SplashAnimation1;
    // Parameters
    private Vector2 StartingPosition { get; set; }
    private Vector2 EndingPosition { get; set; }

    public LineScene() : base()
    {

    }

    public override void Initialize()
    {
        // NOTE: Above this line, content has not been loaded yet.
        base.Initialize();
        // NOTE: Content has been loaded after this line

        // Scene management
        NextScene = new MultiArcScene();
    }

    public override void InitializePath()
    {
        // Define the starting and ending positions
        StartingPosition = Coordinates.NormalizedToScreen(new Vector2(0.2f, 0.5f));
        EndingPosition = Coordinates.NormalizedToScreen(new Vector2(0.8f, 0.5f));
        // EndingPosition = Coordinates.NormalizedToScreen(new Vector2(0.7f, 0.6f));
        // EndingPosition = Coordinates.NormalizedToScreen(new Vector2(0.2f, 0.8f));

        // Create the path
        Path = new LinePath(StartingPosition, EndingPosition);
        Path.LoadSprites(Atlas);

        SplashAnimation1.Origin = new Vector2(0, SplashAnimation1.Height * 0.5f);
        SplashAnimation1.Scale = new Vector2(0.25f, 0.25f);
        SplashAnimation1.Rotation = 3.14f;
    }

    public override void LoadContent()
    {
        base.LoadContent();

        // Create the texture atlas from the XML configuration file
        SplashAtlas = TextureAtlas.FromFile(Core.Content, "images/splash1.xml");
        SplashAnimation1 = SplashAtlas.CreateAnimatedSprite("splash1-animation");
    }

    public override void Update(GameTime gameTime)
    {
        // A lot of common update logic in BaseScene class
        base.Update(gameTime);

        UpdateCreep(gameTime);
        UpdatePlacingTower(gameTime);
        UpdateTowers(gameTime);
        SplashAnimation1.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.DarkSlateBlue);

        DrawPath(gameTime);
        DrawMarkers(gameTime);
        DrawPlacedTowers(gameTime);
        DrawPlacingTower(gameTime);
        DrawSelectedTower(gameTime);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        SplashAnimation1.Draw(Core.SpriteBatch, new Vector2(100, 100));
        Core.SpriteBatch.End();

    }

    public void DrawMarkers(GameTime gameTime)
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: Grayscale);
        StartMarker.Draw(Core.SpriteBatch, StartingPosition);
        EndMarker.Draw(Core.SpriteBatch, EndingPosition);
        TorchCreep.Draw(Core.SpriteBatch, WhitePixel, DebugDraw);
        Core.SpriteBatch.End();
    }
}