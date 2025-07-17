using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Coordinates;

public class Coordinates
{
    public static Vector2 NormalizedToScreen(Vector2 normalized)
    {
        return new Vector2(
            normalized.X * Core.GraphicsDevice.PresentationParameters.Bounds.Width,
            normalized.Y * Core.GraphicsDevice.PresentationParameters.Bounds.Height
        );
    }

    public static Vector2 ScreenToNormalized(Vector2 screen)
    {
        return new Vector2(
            screen.X / Core.GraphicsDevice.PresentationParameters.Bounds.Width,
            screen.Y / Core.GraphicsDevice.PresentationParameters.Bounds.Height
        );
    }
}