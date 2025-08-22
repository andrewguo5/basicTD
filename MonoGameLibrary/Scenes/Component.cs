using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Paths;
using MonoGameLibrary.Scenes;
using MonoGameLibrary;
using MonoGameLibrary.Creeps;
using MonoGameLibrary.Coordinates;
using MonoGameLibrary.Input;
using System.Collections.Generic;

using MonoGameLibrary.Collision;
using System.IO.Pipes;

namespace MonoGameLibrary.Scenes;

/// <summary>
/// When should you load in the component and when should you load in the Scene?
/// 
/// The rule I've come up with is that if anything is shared between two components,
/// then we load it in the Scene and every component will have access to it via props.
/// 
/// This is redundant but it keeps things simple.
/// 
/// If a component can load something that only it needs, then it can load the component in itself.
/// Components really shouldn't modify Props at all as it passes it down to Children.
/// 
/// The reason is that Children are all set up during initialization. You can't load and then
/// pass loaded props into Children, because then the Child load doesn't happen. Why? See this:
/// 1. LoadSelf
/// 2. LoadChildren
/// 
/// If you do this before initializing Children, then the LoadChildren does nothing.
/// 
/// You'd have do this:
/// 1. LoadSelf
/// 2. InitializeChildren
/// 3. LoadChildren
/// 
/// This adds complexity that I'm already immediately opposed to. What this means is that
/// Props is really just a reference to Scene. It would equivalently work just to pass a reference
/// to Scene, and make anything that needs to be shared a public property of the Scene. This also
/// works better with states like DebugDraw. I'm going to move in this direction, and replace
/// Props with a reference to a shared Scene object.
/// </summary>
public abstract class GComponent
{
    protected Rectangle Bounds { get; set; }
    protected List<GComponent> Children { get; set; }
    protected Dictionary<string, dynamic> Props;

    public GComponent(
        Rectangle bounds,
        Dictionary<string, dynamic> props = null
    )
    {
        Bounds = bounds;
        Children = new List<GComponent>();
        Props = props ?? new Dictionary<string, dynamic>();
    }
    protected abstract void InitializeSelf();
    protected abstract void LoadContentSelf();
    protected abstract void DrawSelf(GameTime gameTime);
    protected abstract void UpdateSelf(GameTime gameTime);


    // --- //
    public void Draw(GameTime gameTime)
    {
        DrawSelf(gameTime);
        foreach (var child in Children)
            child.Draw(gameTime);
    }

    public void Update(GameTime gameTime)
    {
        UpdateSelf(gameTime);
        foreach (var child in Children)
            child.Update(gameTime);
    }

    public void Initialize()
    {
        InitializeSelf();
        foreach (var child in Children)
            child.Initialize();
    }

    public void LoadContent()
    {
        LoadContentSelf();
        foreach (var child in Children)
            child.LoadContent();
    }

    public void AddChild(GComponent child)
    {
        Children.Add(child);
    }

    public void AddChildren(IEnumerable<GComponent> children)
    {
        foreach (var child in children)
        {
            AddChild(child);
        }
    }
}