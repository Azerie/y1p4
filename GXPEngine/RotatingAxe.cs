using System;
using GXPEngine;

public class Axe : AnimationSprite
{
	Enemy axe1;
    Enemy axe2;
    float axeLength = 140;
    Vec2 position;
    Sound sound;


	public Axe (Vec2 pPosition) : base ("assets/rotatingaxe.png", 5, 5)
	{
        position = pPosition;
        SetXY(pPosition.x, pPosition.y);
        SetCycle(0, 21);
        axe1 = new Enemy(20, pPosition);
        axe2 = new Enemy(20, pPosition);
        axe2.SetXY(pPosition.x, pPosition.y + 100);
        MyGame mygame = (MyGame)game;
        mygame.AddExistingMover(axe1);
        mygame.AddExistingMover(axe2);
        
        axe1.HideSprite();
        axe2.HideSprite();

        sound = new Sound("assets/Axe whoosh.wav");

    }
	public void Update()
	{
		//Animate(0.25f);
        Animate(0.25f);
        Vec2 axe1pos = new Vec2(Mathf.Cos((float)this.currentFrame / 21 * 2 * Mathf.PI + Mathf.PI / 2), Mathf.Sin((float)this.currentFrame / 21 * 2 * Mathf.PI + Mathf.PI / 2));
        axe1pos.Normalize();
        axe1pos *= axeLength;
        axe1pos += position;
        axe1pos += new Vec2(this.width / 2, this.height / 2);
        axe1.SetXY(axe1pos);

        Vec2 axe2pos = new Vec2(Mathf.Cos((float)this.currentFrame / 21 * 2 * Mathf.PI + 3 * Mathf.PI / 2), Mathf.Sin((float)this.currentFrame / 21 * 2 * Mathf.PI + 3 * Mathf.PI / 2));
        axe2pos.Normalize();
        axe2pos *= axeLength;
        axe2pos += position;
        axe2pos += new Vec2(this.width / 2, this.height / 2);
        axe2.SetXY(axe2pos);

        if (currentFrame == 11 || currentFrame == 1)
        {
            sound.Play(false, 0, 0.1f);
        }
    }
}
