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
    protected abstract void DrawSelf(GameTime gameTime);
    protected abstract void UpdateSelf(GameTime gameTime);
    protected abstract void InitializeSelf();
    protected abstract void LoadContentSelf();


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