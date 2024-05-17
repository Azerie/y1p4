using System;
using GXPEngine;

public class Finish2 : Ball
{
    AnimationSprite sprite;
    bool animationPlaying;
    Sound splash;

    public Finish2(Vec2 pPosition) : base(30, pPosition, pMoving: false)
    {
        sprite = new AnimationSprite("assets/redBucket.png", 3, 3, addCollider: false);
        sprite.height = GetRadius() * 4;
        sprite.width = GetRadius() * 4;
        AddChild(sprite);
        sprite.SetOrigin(60, 60);
        sprite.SetCycle(8, 1); // Set initial frame to the first frame of the animation
        animationPlaying = false;
        splash = new Sound("assets/Milk splash.wav");
        sprite.SetOrigin(170, 220);
    }

    public void PlayAnimation()
    {
        sprite.SetCycle(0, 9); // Set cycle to play the entire animation
        sprite.currentFrame = 0; // Start the animation from the beginning
        animationPlaying = true; // Set flag to indicate animation is playing
        splash.Play();
    }

    void Update()
    {
        if (animationPlaying)
        {
            sprite.Animate(0.5f); // Advance the animation
            Console.WriteLine("Animation playing...");

            // Check if animation has reached the end
            if (sprite.currentFrame >= 8)
            {
                animationPlaying = false; // Animation has finished playing
                sprite.SetCycle(8, 1);
                sprite.currentFrame = 9; // Set frame to the last frame
                Console.WriteLine("Animation finished.");
            }
        }
    }
}
