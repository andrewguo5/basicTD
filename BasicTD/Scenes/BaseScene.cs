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
    // Toggleable Modes
    protected bool DebugDraw = false;
    protected bool Paused = false;
    protected bool Grayed = false;

    // Scene Manager
    public Scene NextScene { get; set; }

    // Effects
    protected Effect Grayscale;
    protected Effect CircleIndicator;

    // Sprite Atlas
    protected TextureAtlas Atlas;

    public BaseScene() : base()
    {

    }

    public override void LoadContent()
    {
        base.LoadContent();

        // Create the texture atlas from the XML configuration file
        Atlas = TextureAtlas.FromFile(Core.Content, "images/things-atlas-definition.xml");

        // Load the grayscale effect
        Grayscale = Core.Content.Load<Effect>("effects/grayscaleEffect");

        // Load the circle indicator effect
        CircleIndicator = Core.Content.Load<Effect>("effects/circleIndicator");
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