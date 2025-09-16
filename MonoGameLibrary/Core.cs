using System;
using BasicTD.Frontend;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace MonoGameLibrary;

public class Core : Game
{
    internal static Core s_instance;

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => s_instance;

    // The scene that is curently active.
    private static Scene s_activeScene;

    // The next scene to switch to, if there is one.
    private static Scene s_nextScene;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Gets a reference to the input management system.
    /// </summary>
    public static InputManager Input { get; private set; }

    /// <summary>
    /// Gets or Sets a value that indicates if the game should exit when the esc key is pressed.
    /// </summary>
    public static bool ExitOnEscape { get; set; }

    /// <summary>
    /// Gets a reference to the audio control system.
    /// </summary>
    public static AudioController Audio { get; private set; }

    public static Scaffold Scaffold { get; private set; }

    public static Scene CurrentScene => s_activeScene;

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title of the game window.</param>
    /// <param name="width">The preferred width of the game window.</param>
    /// <param name="height">The preferred height of the game window.</param>
    /// <param name="fullScreen">Whether the game should run in full screen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Ensure that multiple cores are not created. This is a singleton class.
        if (s_instance != null)
        {
            throw new InvalidOperationException("Only one instance of Core is allowed.");
        }

        // Store reference to engine for global member access (not sure what this means).
        s_instance = this;

        // Init graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set defaults.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // Apply changes.
        Graphics.ApplyChanges();

        // Where did Window come from? Perhaps the base Game class init?
        Window.Title = title;

        // Set reference for core's content manager to the base Game's content manager.
        Content = base.Content;

        // Set the root directory for Content (I missed this earlier and couldn't load assets)
        Content.RootDirectory = "Content";

        // Mouse visible by default
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Input = new InputManager();

        Audio = new AudioController();

        Scaffold = new Scaffold(graphicsDevice: GraphicsDevice);
    }
    protected override void LoadContent()
    {
        base.LoadContent();

        // Set static reference to graphics device. Why here and not earlier?
        GraphicsDevice = base.GraphicsDevice;
 
        // Create the sprite batch instance. Has to happen after content is loaded, I suppose.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        Input = new InputManager();

        Audio = new AudioController();
     }

    protected override void UnloadContent()
    {
        // Dispose of the audio controller.
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);

        Audio.Update();

        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (s_nextScene != null)
        {
            TransitionScene();
        }

        if (s_activeScene != null)
        {
            s_activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }
        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene next, bool force = false)
    {
        // Only change if the next scene value is different
        // from the currently active scene.
        if (s_activeScene != next || force) // can use equals??
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        // The opposite of initialize/load: dispose
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        GC.Collect();

        s_activeScene = s_nextScene;
        s_nextScene = null;

        // If the active scene is now non-null, initialize it.
        // Intialize also calls Scene.LoadContent
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }
}