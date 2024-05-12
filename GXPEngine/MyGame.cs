using System;
using GXPEngine;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MyGame : Game
{	
	bool _stepped = false;
	public bool _paused = true;
	int _stepIndex = 0;
	int _startSceneNumber = 1;
    AnimationSprite background;

    readonly Canvas _lineContainer = null;
    readonly List<Ball> _movers;
    readonly List<LineSegment> _lines;
    readonly Spawner _spawner;

	public EasyDraw gameOver;
	public EasyDraw HUD;

    Sound backgroundMusic;

    public AnimationSprite femboyBounce;

    public bool secondPlayer;
    public bool secondFinish;
    public int goals;

    Player2 player2;
    Finish2 finish2;

    public MyGame() : base(1920, 1080, false, false)
    {
        background = new AnimationSprite("assets/background3.png", 1, 1);
        AddChild(background);

        _lineContainer = new Canvas(width, height);
        AddChild(_lineContainer);

        targetFps = 60;

        _movers = new List<Ball>();
        _lines = new List<LineSegment>();
        
        _spawner = new Spawner();
        AddChild(_spawner);

        gameOver = new EasyDraw(game.width, game.height);
        AddChild(gameOver);

        HUD = new EasyDraw(game.width, game.height);
        AddChild(HUD);
        Sprite linehud = new Sprite("assets/placeholderline.png");
        linehud.SetXY(30, 610);
        Sprite itemhud = new Sprite("assets/placeholderCow.png");
        itemhud.SetXY(30, 710);
        Sprite jumphud = new Sprite("assets/placeholderJumppad.png");
        jumphud.SetXY(40, 820);

        HUD.AddChild(linehud);
        HUD.AddChild(itemhud);
        HUD.AddChild(jumphud);

        femboyBounce = new AnimationSprite("assets/femboy-bounce.png", 8, 8, addCollider:false);
        AddChild(femboyBounce);

        backgroundMusic = new Sound("assets/music.wav", looping:true);
        backgroundMusic.Play();

        LoadScene(_startSceneNumber);


        PrintInfo();
    }

    public int GetNumberOfLines() {
		return _lines.Count;
	}

	public LineSegment GetLine(int index) {
		if (index >= 0 && index < _lines.Count) {
			return _lines [index];
		}
		return null;	
	}

	public int GetNumberOfMovers() {
		return _movers.Count;
	}

	public Ball GetMover(int index) {
		if (index >= 0 && index < _movers.Count) {
			return _movers [index];
		}
		return null;
	}

    public void RemoveMover(int index)
    {
        if (index >= 0 && index < _movers.Count)
        {
			RemoveChild(_movers[index]);
			_movers.Remove(_movers[index]);
        }
    }

    public void RemoveMover(Ball toRemove)
    {
        RemoveChild(toRemove);
        _movers.Remove(toRemove);
    }

    public void AddMover(int radius, Vec2 position, Vec2 velocity= new Vec2(), bool moving=true, float bounciness=0.6f, byte greenness=200)
    {
		Ball newBall = new Ball(radius, position, velocity, moving, bounciness, greenness);
        _movers.Add(newBall);
		AddChild(newBall);
    }

    public void AddEnemy(int radius, Vec2 position, Vec2 velocity = new Vec2(), bool pDestroyedByWalls=false)
    {
        Ball newBall = new Enemy(radius, position, velocity, pDestroyedByWalls:pDestroyedByWalls);
        _movers.Add(newBall);
        AddChild(newBall);
    }

    public void AddBomb(Vec2 position, Vec2 velocity = new Vec2(), bool moving=false)
    {
        Ball newBall = new Bomb(position, velocity, moving);
        _movers.Add(newBall);
        AddChild(newBall);
    }

    public void AddPlayer(Vec2 position, Vec2 velocity = new Vec2(), bool moving=true)
    {
        Ball newBall = new Player(30, position);
        _movers.Add(newBall);
        AddChild(newBall);
    }

    public void AddFinish(Vec2 position)
    {
        Ball newBall = new Finish(position);
        _movers.Add(newBall);
        AddChild(newBall);
    }

    public Player GetPlayer()
    {
        foreach (Ball mover in _movers)
        {
            if (mover is Player)
            {
				return (Player)mover;
            }
        }
		return null;
    }

    public void RemovePlayer()
    {
        foreach (Ball mover in _movers)
        {
            //TODO: remove all players
            if (mover is Player)
            {
                _movers.Remove(mover);
                mover.Destroy();
                break;
            }
            if (mover is Player2)
            {
                _movers.Remove(mover);
                mover.Destroy();
                break;
            }
        }
    }

    public void RemoveThisPlayer(Player player)
    {
            if (player is Player)
            {
                _movers.Remove(player);
                player.Destroy();
            }
    }


    public void DrawLine(Vec2 start, Vec2 end) {
		_lineContainer.graphics.DrawLine(Pens.White, start.x, start.y, end.x, end.y);
	}

	
	public void AddLine (Vec2 start, Vec2 end) {
		LineSegment line = new LineSegment (start, end, 0xff00ff00, 4);
        AddChild(line);
        _lines.Add(line);

        LineSegment lineBack = new LineSegment(end, start, 0xff00ff00, 4);
        AddChild(lineBack);
        _lines.Add (lineBack);
	}

    public void AddEscalator (Vec2 start, Vec2 end, bool reverse=false) {
		LineEscalator line = new LineEscalator (start, end, reverse);
        AddChild(line);
        _lines.Add(line);

        LineEscalator lineBack = new LineEscalator(end, start, reverse);
        AddChild(lineBack);
        _lines.Add (lineBack);
	}

    public void AddGLine(Vec2 start, Vec2 end)
    {
        //TODO: visual indication
        //TODO: improve feel
        LineSegment gLine = new LineSegment(start, end, 0xffffff00, 4);
        AddChild(gLine);
        _lines.Add(gLine);

        LineSegment gLineBack = new LineSegment(end, start, 0xffffff00, 4);
        AddChild(gLineBack);
        _lines.Add(gLineBack);

        float length = (end - start).Length();
        Vec2 line = end - start;


    }

    //TODO: make an eraser
    public void RemoveLine(Vec2 start, Vec2 end)
    {
        // Find and remove the forward line segment
        LineSegment lineToRemove = _lines.Find(line => line.start == start && line.end == end);
        if (lineToRemove != null)
        {
            RemoveChild(lineToRemove);
            _lines.Remove(lineToRemove);
        }

        // Find and remove the backward line segment
        LineSegment lineBackToRemove = _lines.Find(line => line.start == end && line.end == start);
        if (lineBackToRemove != null)
        {
            RemoveChild(lineBackToRemove);
            _lines.Remove(lineBackToRemove);
        }
    }

    public void Pause()
	{
		_paused = true;
	}

    public void UnPause()
    {
        _paused = false;
    }

	void LoadScene(int sceneNumber) {
		_startSceneNumber = sceneNumber;
        // remove previous scene:
        
        foreach (Ball mover in _movers) {
			mover.Destroy();
		}
		_movers.Clear();
		foreach (LineSegment line in _lines) {
			line.Destroy();
		}
		_lines.Clear();


		gameOver.ClearTransparent();
		Pause();
        femboyBounce.visible = false;
        HUD.visible = true;
        


        // boundary:
        AddLine (new Vec2 (width, height), new Vec2 (0, height));
		AddLine (new Vec2 (0, height), new Vec2 (0, 0));
		AddLine (new Vec2 (0, 0), new Vec2 (width, 0));
		AddLine (new Vec2 (width, 0), new Vec2 (width, height));

		switch (sceneNumber) {
			case 1: // pretty much sandbox
                _movers.Add(new Player(30, new Vec2(200, 300)));
                _movers.Add(new Finish(new Vec2(770, 570)));

				int[] itemUses = new int[] { 5, 5, 5, 0, 0, 0 , 1 }; // this being declared here might break something but it shouldn't
                _spawner.SetRemainingUses(itemUses);
                break;
            case 2: // test level
                itemUses = new int[] { 5, 5, 5, 0, 0, 0, 1 };
                _spawner.SetRemainingUses(itemUses);

                _movers.Add(new Player(30, new Vec2(700, 100)));
                _movers.Add(new Player2(30, new Vec2(width-700, 100)));
                _movers.Add(new Enemy(20, new Vec2(650, 500)));
                _movers.Add(new Enemy(20, new Vec2(700, 500)));
                _movers.Add(new Enemy(20, new Vec2(750, 500)));
                _movers.Add(new Enemy(20, new Vec2(250, 400)));
                _movers.Add(new Enemy(20, new Vec2(275, 450)));
                _movers.Add(new Enemy(20, new Vec2(300, 500)));
                AddLine(new Vec2(200, 400), new Vec2(200, 600));
                AddLine(new Vec2(550,0), new Vec2(550, 250));
                _movers.Add(new Finish(new Vec2(100, 500)));
                _movers.Add(new Finish2(new Vec2(width-100, 500)));
                break;
            case 3:
                itemUses = new int[] { 5, 5, 5, 0, 0, 0, 1 };
                _spawner.SetRemainingUses(itemUses);

                _movers.Add(new Player(30, new Vec2(1750, 150)));
                AddLine(new Vec2(1600, 0), new Vec2(1600, 300));
                AddLine(new Vec2(1300, 457), new Vec2(1300, 1080));
                AddLine(new Vec2(950, 0), new Vec2(950, 520));
                _movers.Add(new Finish(new Vec2(350, 1000)));
                break;
            case 4:
                itemUses = new int[] { 5, 5, 5, 0, 0, 0, 1 };
                _spawner.SetRemainingUses(itemUses);

                _movers.Add(new Finish(new Vec2(1273, 993)));
                AddLine(new Vec2(1050, 634), new Vec2(1050, 1080));
                AddLine(new Vec2(750, 0), new Vec2(750, 524));
                AddLine(new Vec2(613, 295), new Vec2(750, 176));
                AddLine(new Vec2(400, 0), new Vec2(400, 100));
                AddLine(new Vec2(400, 450), new Vec2(400, 1080));
                _movers.Add(new Player(30, new Vec2(666, 50)));
                break;
            case 5:
                itemUses = new int[] { 5, 5, 5, 0, 0, 0, 1 };
                _spawner.SetRemainingUses(itemUses);

                _movers.Add(new Player(30, new Vec2(109, 645)));
                AddLine(new Vec2(711, 712), new Vec2(714, 1069));
                _movers.Add(new Finish(new Vec2(1297, 1000)));
                _movers.Add(new Enemy(20, new Vec2(1080, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1150, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1220, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1400, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1470, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1540, 1030)));
                _movers.Add(new Bomb(new Vec2(84, 1048)));
                break;
            default: // level making
                itemUses = new int[] { 99, 99, 99, 99, 1, 1, 1 };
                AddEscalator(new Vec2(1000, 540), new Vec2(200, 540), reverse:true);
                AddEscalator(new Vec2(1010, 540), new Vec2(1800, 540));
                _spawner.SetRemainingUses(itemUses);
                break;
        }
		_stepIndex = -1;
		foreach (Ball b in _movers) {
			AddChild(b);
		}

        finish2 = FindObjectOfType<Finish2>();
        player2 = FindObjectOfType<Player2>();
      
        if (finish2 != null)
        {
            secondFinish = true;
        }
        else
        {
            secondFinish = false;
        }

        if (player2 != null)
        {
            secondPlayer = true;
            goals = 2;
        } else
        {
            goals = 1;
            secondPlayer = false;
        }

    }

	/****************************************************************************************/

	void PrintInfo() {
        /*Console.WriteLine("Hold spacebar to slow down the frame rate.");
		Console.WriteLine("Use arrow keys and backspace to set the gravity.");
		Console.WriteLine("Press S to toggle stepped mode.");
		Console.WriteLine("Press P to toggle pause.");
		Console.WriteLine("Press D to draw debug lines.");
		Console.WriteLine("Press C to clear all debug lines.");
		Console.WriteLine("Press R to reset scene, and numbers to load different scenes.");
		Console.WriteLine("Press B to toggle high/low bounciness.");
		Console.WriteLine("Press W to toggle extra output text.");*/

        Console.WriteLine("Press A/S/D to select ability controlled by mouse");
        Console.WriteLine("Press left mouse button to activate the ability");
        Console.WriteLine("A - draw a line segment (first click selects the start, second click selects the end)");
        Console.WriteLine("S - spawns a small ball (ignore this one probably idk why we would need it)");
        Console.WriteLine("D - spawns a jump pad");
        Console.WriteLine("Level making tools:");
        Console.WriteLine("P - place a player");
        Console.WriteLine("Z - place a spike");
        Console.WriteLine("F - place finish");
        Console.WriteLine("Press a number to select level (0 is empty level for level making)");




    }

	void HandleInput() {
		// targetFps = Input.GetKey(Key.SPACE) ? 5 : 60;
		/*if (Input.GetKeyDown (Key.UP)) {
			Ball.acceleration.SetXY (0, -1);
		}
		if (Input.GetKeyDown (Key.DOWN)) {
			Ball.acceleration.SetXY (0, 1);
		}
		if (Input.GetKeyDown (Key.LEFT)) {
			Ball.acceleration.SetXY (-1, 0);
		}
		if (Input.GetKeyDown (Key.RIGHT)) {
			Ball.acceleration.SetXY (1, 0);
		}
		if (Input.GetKeyDown (Key.BACKSPACE)) {
			Ball.acceleration.SetXY (0, 0);
		}
		if (Input.GetKeyDown (Key.S)) {
			_stepped ^= true;
		}
		if (Input.GetKeyDown (Key.D)) {
			Ball.drawDebugLine ^= true;
		}
		if (Input.GetKeyDown(Key.P))
		{
			_paused ^= true;
		}
		if (Input.GetKeyDown(Key.B))
		{
			Ball.bounciness = 1.5f - Ball.bounciness;
		}
		if (Input.GetKeyDown(Key.W)) {
			Ball.wordy ^= true;
		}
		if (Input.GetKeyDown (Key.C)) {
			_lineContainer.graphics.Clear (Color.Black);
		}*/
		if (Input.GetKeyDown (Key.R)) {
			LoadScene (_startSceneNumber);
		}
		for (int i = 0; i < 10; i++)
		{
			if (Input.GetKeyDown(48 + i))
			{
				LoadScene(i);
			}
		}

        if (Input.GetKeyDown(Key.SPACE))
        {
            UnPause();
            HUD.visible = false;
        }
    }

	void StepThroughMovers() {
		if (_stepped) { // move everything step-by-step: in one frame, only one mover moves
			_stepIndex++;
			if (_stepIndex >= _movers.Count) {
				_stepIndex = 0;
			}
			if (_movers [_stepIndex].moving) {
				_movers [_stepIndex].Step ();
			}
		} else { // move all movers every frame
			for (int i=GetNumberOfMovers(); i >= 0; i--) {
				Ball mover = GetMover(i);
				if (mover != null && mover.moving) {
					mover.Step ();
				}
			}
		}
	}

    void Update () {
		HandleInput();
        femboyBounce.NextFrame();
		if (!_paused) {
			StepThroughMovers ();
		}
        else
        {
            _spawner.Controls();
        }
		HUD.ClearTransparent();
		HUD.Fill (100, 100, 100, alpha:100);
        HUD.Stroke (0, 0, 0);
        HUD.Ellipse (60, 640, 80, 80);
        HUD.Ellipse (60, 740, 80, 80);
        HUD.Ellipse (60, 840, 80, 80);
        int[] uitext = _spawner.GetRemainingUses();
        HUD.Fill(255, 255, 255);
        HUD.Text(uitext[0].ToString() + "x", 80, 680);
        HUD.Text(uitext[1].ToString() + "x", 80, 780);
        HUD.Text(uitext[2].ToString() + "x", 80, 880);


    }

	static void Main() {
		new MyGame().Start();
	}
}