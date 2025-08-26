using BasicTD.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Paths;
using System.Collections.Generic;
using MonoGameLibrary.Creeps;

namespace BasicTD.Components;

public class Battlefield : GComponent
{
    private int VerticalOffset;
    private int Padding;
    private int SideBuffer;
    private TextureAtlas Atlas;
    private SpriteFont GameFont;
    private Rectangle MapBounds;
    private Tilemap PlatformTilemap;
    private Path BattlePath;
    private List<Creep> SpawnedCreepList;

    public Battlefield(Scene parent, Rectangle bounds, Dictionary<string, dynamic> props = null) : base(parent, bounds, props)
    {
        VerticalOffset = props["VerticalOffset"];
        Padding = props["TextPadding"];
        SideBuffer = props["SideBuffer"];
        Atlas = props["Atlas"];
        GameFont = props["GameFont"];
        MapBounds = props["MapBounds"];
    }


    protected override void InitializeSelf()
    {
        SpawnedCreepList = new List<Creep> {
            new Creep(
                BattlePath,
                150f,
                (AnimatedSprite)((GameScene)ParentScene).SpriteDictionary["TorchSprite"])
        };
    }

    protected override void LoadContentSelf()
    {
        PlatformTilemap = Tilemap.FromFile(Core.Content, "images/tilemap-definition.xml");
        PlatformTilemap.Scale = Props["TilemapScale"];

        // Load the path from the XML file
        BattlePath = LinkedPath.FromFile(
            Core.Content,
            "paths/beginner-map-std.xml",
            MapBounds.Location.ToVector2(),
            1f
        );
        BattlePath.LoadSprites(Atlas);
    }

    protected override void DrawSelf(GameTime gameTime)
    {
        DrawPlatform();
        DrawPath();

        if (((GameScene)ParentScene).DebugDraw)
        {
            Core.SpriteBatch.Begin();
            Core.Scaffold.DrawRectanglePerimeter(Core.SpriteBatch, MapBounds, 1);
            Core.SpriteBatch.End();
        }
    }

    protected override void UpdateSelf(GameTime gameTime)
    {
        if (!false)
        {
            // Update the creep
            foreach (var creep in SpawnedCreepList)
                creep.Update(gameTime);
        }
    }

    private void DrawPlatform()
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        PlatformTilemap.Draw(Core.SpriteBatch, MapBounds.Location.ToVector2());
        Core.SpriteBatch.End();
    }

    public void DrawPath()
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        BattlePath.Draw(Core.SpriteBatch, ((GameScene)ParentScene).WhitePixel);

        foreach (var creep in SpawnedCreepList)
            creep.Draw(Core.SpriteBatch, ((GameScene)ParentScene).WhitePixel, ((GameScene)ParentScene).DebugDraw);

        Core.SpriteBatch.End();
    }
}

        