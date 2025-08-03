using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;
    private bool _repeat;
    public bool Repeat
    {
        get => _repeat;
        set
        {
            if (value == false)
            {
                _currentFrame = _animation.Frames.Count - 1;
            }
            _repeat = value;
        }
    }
    
    public float AnimationTime
    {
        get => (_animation.Frames.Count - 1) * (float)_animation.Delay.TotalSeconds;
    }
    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    /// <summary>
    /// Creates a new animated sprite.
    /// </summary>
    public AnimatedSprite()
    {
        Repeat = true;    
    }

    /// <summary>
    /// Creates a new animated sprite with the specified frames and delay
    /// </summary>
    /// <param name="animation">The animation for this animated sprite</param>
    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
        Repeat = true;
    }

    public AnimatedSprite(Animation animation, bool repeat)
    {
        Animation = animation;
        Repeat = repeat;
    }

    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;

        if (_elapsed >= _animation.Delay)
        {
            _elapsed -= _animation.Delay;
            _currentFrame++;
        }

        if (_currentFrame >= _animation.Frames.Count)
        {
            if (Repeat)
                _currentFrame = 0;
            else
                _currentFrame = _animation.Frames.Count -1;
        }

        Region = _animation.Frames[_currentFrame];
    }

    public void Play()
    {
        _currentFrame = 0;
    }
}