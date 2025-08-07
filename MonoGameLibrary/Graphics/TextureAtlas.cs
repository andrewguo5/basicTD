using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    private Dictionary<string, TextureRegion> _regions;

    /// <summary>
    /// Gets or Sets the source texture represented by this texture atlas
    /// </summary>
    public Texture2D Texture { get; set; }

    // Stores animations added to this atlas.
    private Dictionary<string, Animation> _animations;

    // Two constructors: One which doesn't take a texture, and one which does.
    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    // Add, Get, remove, etc. Functions to interface with the class.

    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }
    public void AddRegion(string name, int x, int y, int width, int height, bool rotate, int x_off, int y_off)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height, rotate, x_off, y_off);
        _regions.Add(name, region);
    }

    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    public void Clear()
    {
        _regions.Clear();
    }

    /// <summary>
    /// Adds the given animation to this texture atlas with the specified name
    /// </summary>
    /// <param name="animationName">Name of animation</param>
    /// <param name="animation">Animation to add</param>
    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }

    /// <summary>
    /// Gets the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to retrieve.</param>
    /// <returns>The animation with the specified name.</returns>
    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }

    /// <summary>
    /// Removes the animation with the specified name from this texture atlas.
    /// </summary>
    /// <param name="animationName">The name of the animation to remove.</param>
    /// <returns>true if the animation is removed successfully; otherwise, false</returns>
    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

    // Requires a certain format for an XML configuration file.
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filepath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filepath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                // These are some ways to interact with files, I guess?
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                var regions = root.Element("Regions")?.Elements("Region");

                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        // Get values if they exist, otherwise no-op (handled by null case)
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddRegion(name, x, y, width, height);
                        }
                    }
                }

                // Parse animations
                var animationElements = root.Element("Animations")?.Elements("Animation");

                if (animationElements != null)
                {
                    foreach (var animationElement in animationElements)
                    {
                        // Extract information from XML
                        string name = animationElement.Attribute("name")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                        List<TextureRegion> frames = new List<TextureRegion>();

                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach (var frameElement in frameElements)
                            {
                                string regionName = frameElement.Attribute("region").Value;
                                TextureRegion region = atlas.GetRegion(regionName);
                                frames.Add(region);
                            }
                        }

                        Animation animation = new Animation(frames, delay);
                        atlas.AddAnimation(name, animation);
                    }
                }

                return atlas;
            }
        }
    }
    public static TextureAtlas FromSpineAtlas(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filepath = Path.Combine(content.RootDirectory, fileName);
        List<TextureRegion> frames = new();

        string texturePath = null;
        using (StreamReader reader = new StreamReader(filepath))
        {
            string currentName = null;
            bool rotate = false;
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            int x_off = 0;
            int y_off = 0;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Process each line
                if (texturePath == null & line.Length > 0)
                {
                    texturePath = Path.GetFileNameWithoutExtension(line);
                    atlas.Texture = content.Load<Texture2D>("images/" + texturePath);
                    continue;
                }

                if (line.Length > 1 && char.IsWhiteSpace(line, 0))
                {
                    var values = line.Split(':', ' ', StringSplitOptions.RemoveEmptyEntries);

                    var tag = values[0].Trim();
                    var value = values[1].Trim();
                    switch (tag)
                    {
                        case "rotate":
                            rotate = value == "true";
                            Console.WriteLine("Rotate: " + value);
                            break;
                        case "xy":
                            var xy = value.Split(',', ' ', StringSplitOptions.RemoveEmptyEntries);
                            x = int.Parse(xy[0]);
                            y = int.Parse(xy[1]);
                            Console.WriteLine("xy: " + value);
                            break;
                        case "size":
                            var size = value.Split(',', ' ', StringSplitOptions.RemoveEmptyEntries);
                            width = int.Parse(size[0]);
                            height = int.Parse(size[1]);
                            Console.WriteLine("Size: " + value);
                            break;
                        case "offset":
                            var offset = value.Split(',', ' ', StringSplitOptions.RemoveEmptyEntries);
                            x_off = int.Parse(offset[0]);
                            y_off = int.Parse(offset[1]);
                            break;
                        default:
                            Console.WriteLine("Skip: " + tag);
                            break;
                    }
                }
                else
                {
                    if (currentName != null)
                    {
                        // flush to atlas
                        Console.WriteLine("Adding region: " + currentName);
                        if (rotate)
                            atlas.AddRegion(currentName, x, y, height, width, rotate, x_off - x, y_off - y);
                        else
                            atlas.AddRegion(currentName, x, y, width, height, rotate, x_off - x, y_off - y);
                        frames.Add(atlas.GetRegion(currentName));
                    }
                    currentName = line;
                    x = 0;
                    y = 0;
                    width = 0;
                    height = 0;
                    x_off = 0;
                    y_off = 0;
                    rotate = false;
                }
            }
        }

        TimeSpan delay = TimeSpan.FromMilliseconds(20);
        Animation animation = new Animation(frames, delay);
        atlas.AddAnimation(texturePath, animation);
        return atlas;
    }

    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);
    }

}