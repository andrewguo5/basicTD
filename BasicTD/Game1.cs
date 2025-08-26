using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace BasicTD;

public class Game1 : Core
{
    
    public Game1() : base("BasicTD", 1200, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        // Start with the Line Scene
        ChangeScene(new Scenes.GameScene());
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }
}
