﻿using System;
using GXPEngine;

public class Collectable : Ball
{
	Sprite sprite;
	public Collectable (Vec2 pPosition) : base (50, pPosition, pMoving:false)
	{
        sprite = new Sprite("assets/star.png", addCollider: false);
        AddChild(sprite);
        sprite.SetOrigin(90, 90);
    }
}
