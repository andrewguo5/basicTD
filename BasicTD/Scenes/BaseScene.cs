using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using System.Collections.Generic;
using MonoGameLibrary.Paths;
namespace BasicTD.Scenes;

public class BaseScene : Scene
{
    // Debug Mode
    protected bool DebugDraw = false;

    // Pause Mode
    protected bool Paused = false;

    // Grayscale Mode
    protected bool Grayed = false;

    // Scene Manager
    public Scene NextScene { get; set; }

    public BaseScene() : base()
    {

    }

    public override void Update(GameTime gameTime)
    {
        // Toggle debug mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.D))
            DebugDraw = !DebugDraw;

        // Toggle pause mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.P))
            Paused = !Paused;

        // Toggle grayscale mode
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.G))
            Grayed = !Grayed;

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            Core.ChangeScene(NextScene);
        }
    }

}