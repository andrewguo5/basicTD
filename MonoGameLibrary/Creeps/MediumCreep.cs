using MonoGameLibrary.Creeps;
using MonoGameLibrary.Paths;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Creeps;
public class MediumCreep(Path path, float speed, AnimatedSprite animatedSprite) : Creep(path, speed, animatedSprite)
{
    protected override int Health { get; set; } = 7;
}

public class BigCreep(Path path, float speed, AnimatedSprite animatedSprite) : Creep(path, speed, animatedSprite)
{
    protected override int Health { get; set; } = 15;
}

public class BossCreep(Path path, float speed, AnimatedSprite animatedSprite) : Creep(path, speed, animatedSprite)
{
    protected override int Health { get; set; } = 180;
}