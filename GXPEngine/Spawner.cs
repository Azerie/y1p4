using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;

class Spawner : GameObject 
{
	private int activeItem = 0;

	private int[] remainingUses = {3, 4, 2, 2, 4, 2, 1};

	Vec2 lineStart = new Vec2(-1, -1); // i use a point outside the screen as a way to know when we don't have a point selected

    Vec2 oldMouse = new Vec2(-1, -1);

    private bool addParticles = true;

    Sound sound;

    public Spawner() : base() 
	{
        sound = new Sound("assets/draw.wav", false, false);

	}

    public int[] GetRemainingUses()
    {
        return remainingUses;
    }

    public void SetRemainingUses(int[] array)
    { 
        if(array.Length == 7)
        {
            remainingUses = array;
        }
        else
        {
            Console.WriteLine("couldnt set remaining uses as array length is " + array.Length.ToString() + ", needs to be 7");
        }
    }

    public void SetActiveItem(int item) {
        activeItem = item;
    }

	public void Controls() 
	{
		if (Input.GetKeyDown(Key.A))
		{
			activeItem = 0;
		}
        if (Input.GetKeyDown(Key.S))
        {
            activeItem = 1;
            lineStart.SetXY(-1, -1);
        }
        if (Input.GetKeyDown(Key.D))
        {
            activeItem = 2;
            lineStart.SetXY(-1, -1);
        }
        // level making tools
        if (Input.GetKeyDown(Key.Z)) // spike
        {
            activeItem = 3;
            lineStart.SetXY(-1, -1);
        }
        if (Input.GetKeyDown(Key.P)) // player
        {
            activeItem = 4;
            lineStart.SetXY(-1, -1);
        }
        if (Input.GetKeyDown(Key.F)) // finish
        {
            activeItem = 5;
            lineStart.SetXY(-1, -1);
        }
        if (Input.GetKeyDown(Key.W)) // attempt at making the gravity line
        {
            activeItem = 6;
        }
        if (Input.GetKeyDown(Key.X))
        {
            activeItem = 7;
        }
        if (Input.GetKeyDown(Key.C))
        {
            activeItem = 8;
        }
        if (Input.GetKeyDown(Key.F))
        {
            addParticles = !addParticles;
        }

        if (Input.GetMouseButtonDown(0) && ((MyGame)parent)._paused && Input.mouseX > 140) // activate the item
        {
			if (activeItem == 7 || activeItem == 8 || remainingUses[activeItem] > 0) // bit cursed for now
			{
                switch (activeItem)
                {
                    case 0:
                        if (lineStart == new Vec2(-1, -1)) 
                        {
                            lineStart.SetXY(Input.mouseX, Input.mouseY);
                            sound.Play(volume:0.2f);
                        }
                        else
                        {
                            // ((MyGame)parent).AddLine(lineStart, new Vec2(Input.mouseX, Input.mouseY), removable:true);
                            Console.WriteLine("AddLine(new Vec2" + lineStart.ToString() + ", new Vec2(" + Input.mouseX.ToString() + ", " + Input.mouseY.ToString() + "));");
                            lineStart.SetXY(-1, -1);
                            remainingUses[activeItem] -= 1;
                        }
                        break;
                    case 1:
                        Ball Eraser = new Ball(10, new Vec2(Input.mouseX, Input.mouseY));
                        List<Ball> balls = Eraser.GetAllBallOverlaps();
                        Console.WriteLine(balls.Count);
                        for (int i = balls.Count - 1; i >= 0; i--) 
                        {
                            Console.WriteLine(balls[i]);
                            if ((balls[i]).IsRemovable())
                            {
                                ((MyGame)game).RemoveMover(balls[i]);
                            }
                        }
                        List<LineSegment> lines = Eraser.GetAllLineOverlaps();
                        Console.WriteLine(lines.Count);
                        for (int i = lines.Count - 1; i >= 0; i--)
                        {
                            LineSegment line = lines[i];
                            Console.WriteLine(line.removable);
                            if (line.removable)
                            {
                                ((MyGame)game).RemoveLine(line.start, line.end);
                            }
                        }
                        break;
                    case 2:
                        ((MyGame)parent).AddBomb(new Vec2(Input.mouseX, Input.mouseY), moving: false, removable:true);
                        Console.WriteLine("_movers.Add(new Bomb(new Vec2(" + Input.mouseX.ToString() + ", " + Input.mouseY.ToString() + ")));");
                        break;
                    case 3:
                        ((MyGame)parent).AddEnemy(20, new Vec2(Input.mouseX, Input.mouseY));
                        Console.WriteLine("_movers.Add(new Enemy(20, new Vec2(" + Input.mouseX.ToString() + ", " + Input.mouseY.ToString() + ")));");
                        break;
                    case 4:
                        ((MyGame)parent).AddPlayer(new Vec2(Input.mouseX, Input.mouseY));
                        Console.WriteLine("_movers.Add(new Player(30, new Vec2(" + Input.mouseX.ToString() + ", " + Input.mouseY.ToString() + ")));");
                        break;
                    case 5:
                        ((MyGame)parent).AddFinish(new Vec2(Input.mouseX, Input.mouseY));
                        Console.WriteLine("_movers.Add(new Finish(new Vec2(" + Input.mouseX.ToString() + ", " + Input.mouseY.ToString() + ")));");
                        break;
                    case 6:
                        if (lineStart == new Vec2(-1, -1))
                        {
                            lineStart.SetXY(Input.mouseX, Input.mouseY);
                        }
                        else
                        {
                            ((MyGame)parent).AddGLine(lineStart, new Vec2(Input.mouseX, Input.mouseY));
                            Console.WriteLine("AddLine(new Vec2" + lineStart.ToString() + ", new Vec2(" + Input.mouseX.ToString() + ", " + Input.mouseY.ToString() + "));");
                            lineStart.SetXY(-1, -1);
                            remainingUses[activeItem] -= 1;
                        }
                        break;
                    case 7:
                        ((MyGame)parent).AddChild(new Axe(new Vec2(Input.mouseX, Input.mouseY)));
                        break;
                    case 8:
                        ((MyGame)parent).AddExistingMover(new Collectable(new Vec2(Input.mouseX, Input.mouseY)));
                        break;
                }
				if(activeItem != 0 && activeItem != 6 && activeItem != 7 && activeItem != 8) { remainingUses[activeItem] -= 1; } // line is the only item that is not automatically used after one click
            } 
		}
        if (lineStart != new Vec2(-1, -1) && ((MyGame)parent)._paused)
        {
            ((MyGame)parent).RemoveLine(lineStart, oldMouse);
            if (activeItem == 0)
            {
                ((MyGame)parent).AddLine(lineStart, new Vec2(Input.mouseX, Input.mouseY), removable:true);
            } else
            {
                ((MyGame)parent).AddGLine(lineStart, new Vec2(Input.mouseX, Input.mouseY));
            }
            oldMouse = new Vec2(Input.mouseX, Input.mouseY);
        }
    }

	public void Update() 
	{
        
        // For a better setup: you should move all this hard code to a configurable Emitter class!
        if (addParticles)
        {
            if (Utils.Random(0, 10) == 0)
            { // on average, we spawn a particle every 10 frames
                Particle newParticle = new Particle("assets/placeholderCow.png", BlendMode.ADDITIVE, 4000);
                // An example of chaining:
                newParticle.SetColor(Color.Yellow, Color.Black).
                    SetScale(0, 0.1f).
                    SetVelocity(Utils.Random(-0.1f, 0.1f), Utils.Random(-0.1f, 0.1f));

                newParticle.SetXY(Utils.Random(0, game.width), Utils.Random(0, game.height));
                AddChild(newParticle);
            }
        }
    }
}
