using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;
using GXPEngine;
using GXPEngine.Core;

public class Ball : EasyDraw
{
	// These four public static fields are changed from MyGame, based on key
	// (see Console):
	public static bool drawDebugLine = false;
	public static bool wordy = false;
	public float bounciness = 0.6f;
	// For ease of testing / changing, we assume every ball has the same acceleration (gravity):
	public Vec2 acceleration = new Vec2 (0, 0.7f);

	private int maxVelocity = 100;


	public Vec2 velocity;
	public Vec2 position;

	private int radius;
	public readonly bool moving;
    private readonly bool removable;


    Sound bounceSound;
	Sound bigBounceSound;
	Sound winSound;
	protected Sound loseSound;

	// Mass = density * volume.
	// In 2D, we assume volume = area (=all objects are assumed to have the same "depth")


	public float Mass {
		get {
			return radius * radius * _density;
		}
	}

	public bool IsMoving()
	{
		return moving;
	}

	private Vec2 _oldPosition;
	private Arrow _velocityIndicator;

	private float _density = 1;
	public Ball (int pRadius, Vec2 pPosition, Vec2 pVelocity=new Vec2(), bool pMoving=true, float pBounciness=0.6f, bool pRemovable=false) : base (pRadius*2 + 1, pRadius*2 + 1)
	{
		radius = pRadius;
		position = pPosition;
		velocity = pVelocity;
		moving = pMoving;
		bounciness = pBounciness;
		removable = pRemovable;

        position = pPosition;
		UpdateScreenPosition ();
		SetOrigin (radius, radius);


		bounceSound = new Sound("assets/normal bounce.mp3", looping:false);
		winSound = new Sound("assets/winsound.wav", looping:false);
        loseSound = new Sound("assets/loseSound.wav", looping: false);
		bigBounceSound = new Sound("assets/Jumppad bounce.wav");

        Draw (150, 150, 0);

		_velocityIndicator = new Arrow(position, new Vec2(0,0), 10);
		 //AddChild(_velocityIndicator);
		alpha = 0;
		

		if(!moving)
		{
			acceleration = new Vec2(0, 0);
		}

		MyGame mygame = (MyGame)game;
 

		if ((mygame.secondFinish && !mygame.secondPlayer) || (!mygame.secondFinish && mygame.secondPlayer))
		{
			throw new Exception("amount of finishes and players dont match up");
		}
    }

	public void SetRadius(int pRadius) // for the small size powerup probably
	{
		radius = pRadius;
	}

	public int GetRadius()
	{
		return radius;
	}

	public void SetXY(Vec2 pPosition)
	{
		position = pPosition;
    }

	public bool IsRemovable()
	{
		return removable;
	}

	void Draw(byte red, byte green, byte blue) {
		Fill (red, green, blue);
		Stroke (red, green, blue);
		Ellipse (radius, radius, 2*radius, 2*radius);
	}

	void UpdateScreenPosition() {
		x = position.x;
		y = position.y;
	}

	public void Step () {
		velocity += acceleration; // euler goes whooosh

		if (velocity.Length() > maxVelocity) 
		{
			velocity = velocity.Normalized() * maxVelocity;
		}

		bool repeat = true;
		while (repeat) // we are not counting collisions at the start of the frame to avoid phasing through objects when things are not moving a lot
		{
			_oldPosition = position;
			position += velocity;

			CollisionInfo firstCollision = FindEarliestCollision();
			if (firstCollision != null)
			{
				ResolveCollision(firstCollision);
				if (firstCollision.timeOfImpact < 0.00001)
				{
                    repeat = true;
                }
				else
				{
					repeat = false;
				}
			}
            else
            {
                repeat = false;
            }
        }

		UpdateScreenPosition();
		ShowDebugInfo();

        //rotateeee
		if (this is Player)
		{
            rotation += velocity.x;
        }
        
		MyGame myGame = (MyGame)game;


		if (myGame.goals < 1)
		{
            myGame.Pause();
			Sprite sprite = new Sprite("assets/hud2.png");
			sprite.SetXY(myGame.width/2, myGame.height/2);
            myGame.gameOver.DrawSprite(sprite);
            myGame.gameOver.TextFont("Comic Sans MS", 15);
			myGame.gameOver.TextAlign(CenterMode.Center, CenterMode.Center);
            myGame.gameOver.Text("You won\nPress N to load the next level\n" + myGame.GetCollectedNumber().ToString() + "/" + (myGame.GetCollectableNumber() + myGame.GetCollectedNumber()).ToString() + " stars collected", game.width / 2, game.height / 2);
			int collected = myGame.GetCollectedNumber();

			switch (collected)
			{
				case 1:
                    Sprite star1 = new Sprite("assets/star.png");
                    star1.SetXY(myGame.width / 2 - 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star1);

                    Sprite star2 = new Sprite("assets/EmptyStarUI.png");
                    star2.SetXY(myGame.width / 2, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star2);

                    Sprite star3 = new Sprite("assets/EmptyStarUI.png");
                    star3.SetXY(myGame.width / 2 + 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star3);
					break;

				case 2:
                    Sprite star12 = new Sprite("assets/star.png");
                    star12.SetXY(myGame.width / 2 - 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star12);

                    Sprite star22 = new Sprite("assets/star.png");
                    star22.SetXY(myGame.width / 2, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star22);

                    Sprite star32 = new Sprite("assets/EmptyStarUI.png");
                    star32.SetXY(myGame.width / 2 + 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star32);
                    break;

				case 3:
                    Sprite star13 = new Sprite("assets/star.png");
                    star13.SetXY(myGame.width / 2 - 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star13);

                    Sprite star23 = new Sprite("assets/star.png");
                    star23.SetXY(myGame.width / 2, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star23);

                    Sprite star33 = new Sprite("assets/star.png");
                    star33.SetXY(myGame.width / 2 + 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star33);
                    break;

                default:
                    Sprite star10 = new Sprite("assets/EmptyStarUI.png");
                    star10.SetXY(myGame.width / 2 - 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star10);

                    Sprite star20 = new Sprite("assets/EmptyStarUI.png");
                    star20.SetXY(myGame.width / 2, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star20);

                    Sprite star30 = new Sprite("assets/EmptyStarUI.png");
                    star30.SetXY(myGame.width / 2 + 100, myGame.height / 2 + 90);
                    myGame.gameOver.DrawSprite(star30);
                    break;
			}

			// myGame.femboyBounce.visible = true;
			myGame.soundChannel.IsPaused = true;
			winSound.Play();
        }

    }

    public List<Ball> GetAllBallOverlaps()
    {
        MyGame myGame = (MyGame)game;
        List<Ball> cols = new List<Ball>();
        for (int i = 0; i < myGame.GetNumberOfMovers(); i++)
        {
            Ball mover = myGame.GetMover(i);
            if (mover != this)
            {
                if ((position - mover.position).Length() <= (radius + mover.GetRadius()))
                {
                    cols.Add(mover);
                }
            }
        }
        return cols;
    }

    public List<LineSegment> GetAllLineOverlaps()
    {
        MyGame myGame = (MyGame)game;
        List<LineSegment> cols = new List<LineSegment>();
        for (int i = 0; i < myGame.GetNumberOfLines(); i++)
        {
            LineSegment line = myGame.GetLine(i);
            Vec2 differenceVector = position - line.start;
            float distToLine = Mathf.Abs(differenceVector.Dot((line.end - line.start).Normal())) - radius;
            if (distToLine <= radius)
            {
                cols.Add(line);
            }
        }
        return cols;
    }

    CollisionInfo CheckAllBalls()
	{
        MyGame myGame = (MyGame)game;
        for (int i = 0; i < myGame.GetNumberOfMovers(); i++)
        {
            Ball mover = myGame.GetMover(i);
            if (mover != this)
            {
				CollisionInfo col = CheckOneBall(mover);
				if (col != null)
				{
					return col;
				}
            }
        }
		return null;
    }

	CollisionInfo CheckOneBall(Ball mover) // this is just quadratic formula from slides
	{
        Vec2 relativePosition = _oldPosition - mover.position;
        if (this.velocity.Length() != 0)
        {
            float a = (float)Math.Pow(this.velocity.Length(), 2);
            float b = 2 * relativePosition.Dot(this.velocity);
            float c = (float)(Math.Pow(relativePosition.Length(), 2) - Math.Pow((this.radius + mover.radius), 2));
            if (c < 0 && b < 0)
            {
                return new CollisionInfo((this._oldPosition - mover.position).Normalized(), mover, 0);
            }
            if (b * b - 4 * a * c >= 0)
            {
                float timeOfImpact = (float)((-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a));
                if (timeOfImpact > 0 && timeOfImpact <= 1)
                {
                    return new CollisionInfo(((this._oldPosition + this.velocity * timeOfImpact) - mover.position).Normalized(), mover, timeOfImpact);
                }
            }
        }
		return null;
    }

	CollisionInfo CheckAllLines()
	{
        MyGame myGame = (MyGame)game;
        for (int i = 0; i < myGame.GetNumberOfLines(); i++)
        {
            LineSegment line = myGame.GetLine(i);
            CollisionInfo col = CheckOneLine(line);
			if(col != null)
			{
				return col;
			}
        }
        return null;
    }

	CollisionInfo CheckOneLine(LineSegment line)
	{
        Vec2 startLine = line.start;
        Vec2 endLine = line.end;

        Vec2 oldDifferenceVector = _oldPosition - startLine;
        float a = oldDifferenceVector.Dot((endLine - startLine).Normal()) - radius; // names a and b are from slides week 4/5; the slides didn't bother naming them properly, so neither will i

        float b = (_oldPosition - position).Dot((endLine - startLine).Normal());

        float ballDistance = (position - startLine).Dot((endLine - startLine).Normal());

        float timeOfImpact = 2; // if neither of the two conditions below are true, the collision is not happening anyway
        if (a >= 0)
        {
            timeOfImpact = a / b;
        }
        else if (a >= -radius)
        {
            timeOfImpact = 0;
        }



		if (b > 0 && ballDistance <= radius && timeOfImpact <= 1) // this is stuff from slides again
		{

			Vec2 addDist = velocity * timeOfImpact;
			Vec2 pointOfImpact = _oldPosition + addDist;

			float distAlongLine = (pointOfImpact - startLine).Dot((endLine - startLine).Normalized());



			if (distAlongLine > 0 && distAlongLine < (startLine - endLine).Length())
			{
				if (a > -radius && a < 0)
				{
					return new CollisionInfo((endLine - startLine).Normal(), line, 0);
				}
				return new CollisionInfo((endLine - startLine).Normal(), line, timeOfImpact);
			}
		}
		else if (line.magnet == true)
		{
			Ellipse(line.end.x, line.end.y, (line.end-line.start).Length(), radius * 5);
			if (b > 0 && CalculateDistance(startLine, endLine, position) < radius * 5)
			{
				Vec2 lineVec = line.end - line.start;
				if ((endLine - position).Length() < (line.end - line.start).Length() && (startLine - position).Length() < (line.end - line.start).Length())
                {
					if ((position - lineVec.Normal()).Length() > (position - (lineVec.Normal() * -1)).Length())
					{
						float rotation = lineVec.Normal().GetAngleDegrees();
						acceleration.SetAngleDegrees(rotation);
				
					} else
					{
                        acceleration.SetAngleDegrees(90);
                    }
				}
            }
			else
			{
				acceleration.SetAngleDegrees(90);
			}
		}

        // line caps

        Ball lineCap = new Ball(0, startLine, pMoving:false);
        CollisionInfo capCol = CheckOneBall(lineCap);
		return capCol; // dont need to check for null since it's the last return anyway
    }

	CollisionInfo FindEarliestCollision() {
		MyGame myGame = (MyGame)game;
		// Check other balls:
		CollisionInfo ballCollision = CheckAllBalls();
		if (ballCollision != null) 
		{
			return ballCollision;
		}

        // check lines
        CollisionInfo lineCollision = CheckAllLines();
		return lineCollision;
	}

	void ResolveCollision(CollisionInfo col) {

		if (col.timeOfImpact == 0) // making sure we don't fall through stuff 
		{
			position = _oldPosition;
		}
		else
		{
			position -= velocity * (1 - col.timeOfImpact);
		}

		UpdateScreenPosition();
		if (col.other is Collectable)
		{
			MyGame myGame = (MyGame)game;
			myGame.RemoveMover((Collectable)col.other);
			myGame.AddToCollectedNumber();
		}
		else if (col.other is Ball)
		{
			Ball otherBall = (Ball)col.other;

			if (otherBall.IsMoving())
			{
				otherBall.velocity.Reflect((otherBall.bounciness + bounciness) / 2, col.normal);
				velocity.Reflect((otherBall.bounciness + bounciness) / 2, col.normal);

				/*if (col.timeOfImpact == 0)
				{
					// no newtons laws here so Step doesn't get stuck in an infinite loop
					otherBall.velocity.Reflect((otherBall.bounciness + bounciness) / 2, col.normal);
					velocity.Reflect((otherBall.bounciness + bounciness) / 2, col.normal);
				}
				else // newton is happy here (his laws work)
				{
					float cor = (otherBall.bounciness + bounciness) / 2; // cor is coefficient of reflection
					Vec2 massCenterVelocity = (this.velocity * this.Mass + otherBall.velocity * otherBall.Mass) / (this.Mass + otherBall.Mass);
					velocity = massCenterVelocity - cor * (this.velocity - massCenterVelocity);;
					otherBall.velocity = massCenterVelocity - cor * (otherBall.velocity - massCenterVelocity);
				}*/
			}
			else
			{
				velocity.Reflect((otherBall.bounciness + bounciness) / 2, col.normal);
			}
			if ((otherBall is Enemy && this is Player) || (this is Enemy && otherBall is Player))
			{
				MyGame myGame = (MyGame)game;
				myGame.RemovePlayer();
				myGame.Pause();
				myGame.soundChannel.IsPaused = true;
				loseSound.Play(volume:0.1f);

				myGame.gameOver.Text("Game Over\nPress R to restart the level", game.width / 2, game.height / 2);
			}
			// bombs are not family friendly so we will make them into jump pads instead
			// else if (this is Bomb || otherBall is Bomb)
			// {
			//     if (this is Bomb)
			//     {
			// 		((Bomb)this).Explode();
			//     }
			// 	else
			// 	{
			// 		((Bomb)otherBall).Explode();
			// 	}
			// }
			else if (this is Player && otherBall is Bomb)
			{
				this.velocity += new Vec2(0, -20);
				bigBounceSound.Play();
			}
			else if (otherBall is Finish && !(otherBall is Finish2) && this is Player && !(this is Player2))
			{
				((Finish)otherBall).PlayAnimation();
				Console.WriteLine("rabort");
				MyGame myGame = (MyGame)game;
				myGame.RemovePlayer();
				myGame.goals--;
			}
			else if ((otherBall is Finish2) && (this is Player2))
			{
                ((Finish2)otherBall).PlayAnimation();
                MyGame myGame = (MyGame)game;
				myGame.RemoveThisPlayer((Player)this);
				myGame.goals--;
			}
			else if (this is Egg)
			{
				this.Destroy();

			}
		}
		else
		{
            if (col.timeOfImpact != 0)
            {
                bounceSound.Play();
            }
			if (col.other is LineEscalator)
			{
				this.velocity += ((LineEscalator)col.other).collateralVec().Normalized() * ((LineEscalator)col.other).force;
			}
			velocity.Reflect(bounciness, col.normal);
			if ((this is Enemy && ((Enemy)this).IsDestroyedByWalls()) || this is Egg)
			{
				((MyGame)game).RemoveMover(this);
				this.Destroy();
				Console.WriteLine("remov");
			}
		}
    }

	//code not neccecary but im keeping it here just in case

/*    Vec2 Pull(LineSegment line, float pullStrength = 1)
    {
        // Vector from 'a' to 'b'
        Vec2 ab = line.end - line.start;

        // Vector from 'a' to 'p'
        Vec2 ap = position - line.start;

        // Project 'ap' onto 'ab'
        float abLengthSquared = ab.Dot(ab);
        float projectionScalar = ap.Dot(ab) / abLengthSquared;

        // Find nearest point on the line to 'p'
        Vec2 nearest = line.start + projectionScalar * ab;

        // Calculate vector from player's position to the closest point on the line
        Vec2 toClosest = nearest - position;

        // Calculate distance to the closest point
        float distClosest = toClosest.Length();
        // Apply pulling force towards the closest point on the line
        if (distClosest > 0)
        {
            // Calculate pull direction
            Vec2 pullDirection = toClosest.Normalized();

            // Calculate pull magnitude based on distance
            float pullMagnitude = Mathf.Min(pullStrength / distClosest, pullStrength) * pullStrength;

            // Calculate the resulting pull vector (direction * magnitude)
            Vec2 pullVector = pullDirection * pullMagnitude;
			

            // Create an arrow object to visualize the pull (for demonstration)
            Arrow arrow = new Arrow(position, pullDirection, pullMagnitude);
            AddChild(arrow);

            // Return the pull vector (direction * magnitude)
            return pullVector;
        }

        // If no pull is applied (unlikely in this function), return zero vector
        return nearest;
    }*/


    public static float CalculateDistance(Vec2 a, Vec2 b, Vec2 p)
    {
        // Vector from 'a' to 'b'
        Vec2 ab = b - a;

        // Vector from 'a' to 'p'
        Vec2 ap = p - a;

        // Project 'ap' onto 'ab'
        float abLengthSquared = ab.Dot(ab);
        float projectionScalar = ap.Dot(ab) / abLengthSquared;

        // Find nearest point on the line to 'p'
        Vec2 nearest = a + projectionScalar * ab;

        // Calculate distance from 'p' to the nearest point on the line
        float distance = Mathf.Sqrt((p.x - nearest.x) * (p.x - nearest.x) + (p.y - nearest.y) * (p.y - nearest.y));
     

        return distance;
    }



    void BoundaryWrapAround() {
		if (position.x < 0) {
			position.x += game.width;
		}
		if (position.x > game.width) {			
			position.x -= game.width;
		}
		if (position.y < 0) {
			position.y += game.height;
		}
		if (position.y > game.height) {
			position.y -= game.height;
		}
	}

	void ShowDebugInfo() {
		if (drawDebugLine) {
			((MyGame)game).DrawLine (_oldPosition, position);
		}
		_velocityIndicator.startPoint = position;
		_velocityIndicator.vector = velocity;
	}
}

