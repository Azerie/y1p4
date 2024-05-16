using System;
using GXPEngine;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

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
    public Sprite backgrHUD;
    public EasyDraw hud2;

    public EasyDraw wood;

    public Sound backgroundMusic;
    public SoundChannel soundChannel;

    public AnimationSprite femboyBounce;

    public bool secondPlayer;
    public bool secondFinish;
    public int goals;

    private int collected;

    Player2 player2;
    Finish2 finish2;


    //TODO: ATTEMPT TO FIX EGG PNG ISSUE


    public MyGame() : base(1920, 1080, false, false)
    {
        background = new AnimationSprite("assets/background3.png", 1, 1);
        AddChild(background);

        wood = new EasyDraw(1920, 1080);
        AddChild(wood);

        _lineContainer = new Canvas(width, height);
        AddChild(_lineContainer);

        targetFps = 60;

        _movers = new List<Ball>();
        _lines = new List<LineSegment>();
        
        _spawner = new Spawner();
        AddChild(_spawner);

        gameOver = new EasyDraw(game.width, game.height);
        AddChild(gameOver);


        backgrHUD = new Sprite("assets/hud.png");
        backgrHUD.SetXY(-10, -50);
        AddChild(backgrHUD);

        HUD = new EasyDraw(game.width, game.height);
        AddChild(HUD);

        Sprite linehud = new Sprite("assets/line.png");
        linehud.SetXY(70, 280);
        Sprite itemhud = new Sprite("assets/eraser.png");
        itemhud.SetXY(70, 430);
        Sprite jumphud = new Sprite("assets/jumppad2.png");
        jumphud.width = 50;
        jumphud.height = 50;
        jumphud.SetXY(70, 580);

        HUD.AddChild(linehud);
        HUD.AddChild(itemhud);
        HUD.AddChild(jumphud);


        Sprite star1 = new Sprite("assets/EmptyStarUI.png");
        star1.SetXY(50, 140);
        Sprite star2 = new Sprite("assets/EmptyStarUI.png");
        star2.SetXY(130, 140);
        Sprite star3 = new Sprite("assets/EmptyStarUI.png");
        star3.SetXY(210, 140);

        HUD.AddChild(star1);
        HUD.AddChild(star2);
        HUD.AddChild(star3);

        Sprite A = new Sprite("assets/kb_a.png");
        A.SetXY(40, 320);
        A.SetScaleXY(.5f, .5f);
        Sprite D = new Sprite("assets/kb_s.png");
        D.SetXY(40, 480);
        D.SetScaleXY(.5f, .5f);
        Sprite S = new Sprite("assets/kb_d.png");
        S.SetXY(40, 630);
        S.SetScaleXY(.5f, .5f);

        HUD.AddChild(A);
        HUD.AddChild(D);
        HUD.AddChild(S);

        femboyBounce = new AnimationSprite("assets/femboy-bounce.png", 8, 8, addCollider:false);
        AddChild(femboyBounce);

        backgroundMusic = new Sound("assets/music.wav", looping:true);

        soundChannel = (SoundChannel)backgroundMusic.Play(false, 0, .2f);

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
    public void AddEnemy(int radius, Vec2 position, Vec2 velocity = new Vec2(), bool pDestroyedByWalls = false)
    {
        Ball newBall = new Enemy(radius, position, velocity, pDestroyedByWalls: pDestroyedByWalls);
        _movers.Add(newBall);
        AddChild(newBall);
    }

    public void AddMover(int radius, Vec2 position, Vec2 velocity= new Vec2(), bool moving=true, float bounciness=0.6f, bool removable=false)
    {
		Ball newBall = new Ball(radius, position, velocity, moving, bounciness, removable);
        _movers.Add(newBall);
		AddChild(newBall);
    }

    public void AddExistingMover(Ball ball) {
        _movers.Add(ball);
        AddChild(ball);
    }

    public void AddBomb(Vec2 position, Vec2 velocity = new Vec2(), bool moving = false, bool removable = false)
    {
        Ball newBall = new Bomb(position, velocity, moving, pRemovable: removable);
        _movers.Add(newBall);
        AddChild(newBall);
    }

    
    public void AddEgg(int radius, Vec2 position, Vec2 velocity = new Vec2(), bool pDestroyedByWalls = false, bool moving = false)
    {
        Ball newBall = new Egg(radius, position, velocity, pDestroyedByWalls: pDestroyedByWalls, pMoving: moving);
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

    public int GetCollectableNumber()
    {
        int a = 0;
        foreach (Ball mover in _movers)
        {
            if (mover is Collectable)
            {
				a += 1;
            }
        }
		return a;
    }

    public int GetCollectedNumber()
    {
        return collected;
    }

    public void AddToCollectedNumber() { collected++; }

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

	
	public void AddLine (Vec2 start, Vec2 end, bool removable=false, bool visible = true) {
		LineSegment line = new LineSegment (start, end, 0xffffffff, 4, pRemovable:removable);
        if (visible != true)
        {
            line.color = 0x00ffff00;
        }
        AddChild(line);
        _lines.Add(line);

        LineSegment lineBack = new LineSegment(end, start, 0xffffffff, 4, pRemovable:removable);
        if (visible != true)
        {
            lineBack.color = 0x00ffff00;
        }
        AddChild(lineBack);
        _lines.Add (lineBack);
        //TODO: max line length
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
        soundChannel.IsPaused = false;
        
        foreach (Ball mover in _movers) {
			mover.Destroy();
		}
		_movers.Clear();
		foreach (LineSegment line in _lines) {
			line.Destroy();
		}
		_lines.Clear();
        for(int i = GetChildCount() - 1; i >= 0; i--)
        {
            GameObject obj = GetChildren()[i];
            if (obj is Axe) { RemoveChild(obj); }
        }
        for (int i = wood.GetChildCount() - 1; i >= 0; i--)
        {
            GameObject obj = wood.GetChildren()[i];
            wood.RemoveChild(obj);
        }



        gameOver.ClearTransparent();
		Pause();
        femboyBounce.visible = false;
        HUD.visible = true;
        backgrHUD.visible = true;
        collected = 0;


        // boundary:
        /* AddLine (new Vec2 (width, height), new Vec2 (0, height));
         AddLine (new Vec2 (0, height), new Vec2 (0, 0));
         AddLine (new Vec2 (0, 0), new Vec2 (width, 0));
         AddLine (new Vec2 (width, 0), new Vec2 (width, height));*/

        AddLine(new Vec2(1801, 1074), new Vec2(1803, 433), visible:false);
        AddLine(new Vec2(1803, 433), new Vec2(1686, 145), visible:false);
        AddLine(new Vec2(1686, 145), new Vec2(1471, 3), visible: false);
        AddLine(new Vec2(1471, 3), new Vec2(446, 4), visible: false);
        AddLine(new Vec2(446, 4), new Vec2(241, 142), visible: false);
        AddLine(new Vec2(241, 142), new Vec2(142, 389), visible: false);
        AddLine(new Vec2(148, 378), new Vec2(146, 1072), visible: false);
        AddLine(new Vec2(width, height), new Vec2(0, height), visible: false);

        string lvlNumStr;

        lvlNumStr = ConfigurationManager.AppSettings.Get("levelsNumber");
        int lvlNum = Convert.ToInt16(lvlNumStr);

        if (sceneNumber <= lvlNum && sceneNumber != 0)
        {
            String line;
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader("levels/" + sceneNumber.ToString() + ".txt"); // cursed btw but i'm lazy
            //Read the first line of text
            line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                //write the line to console window
                if (line.Length > 0) 
                {
                    string[] level = line.Split(' ');
                    int itemType = Convert.ToInt16(level[0]);
                    switch(itemType)
                    {
                        case 0:
                            _movers.Add(new Player(36, new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 1:
                            _movers.Add(new Finish(new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 2:
                            _movers.Add(new Collectable(new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 3:
                            AddChild(new Axe(new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 4:
                            _movers.Add(new Player2(40, new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 5:
                            _movers.Add(new Finish2(new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 6:
                            _movers.Add(new Enemy(20, new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 7:
                            Vec2 lineStart = new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]));
                            Vec2 lineEnd = new Vec2(Convert.ToInt16(level[3]), Convert.ToInt16(level[4]));
                            AddLine(lineStart, lineEnd, visible:false);
                            break;
                        case 8:
                            _movers.Add(new Bomb(new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2]))));
                            break;
                        case 9:
                            _movers.Add(new ShootingEnemy(new Vec2(Convert.ToInt16(level[1]), Convert.ToInt16(level[2])), new Vec2(Convert.ToInt16(level[3]), Convert.ToInt16(level[4]))));
                            break;
                    }
                }
                //Read the next line
                line = sr.ReadLine();
            }
            //close the file
            sr.Close();
            // Console.ReadLine();
        }
        else
        {
            int[] itemUses = new int[] { 99, 99, 99, 99, 1, 1, 1 };
            // AddEscalator(new Vec2(1000, 540), new Vec2(200, 540), reverse:true);
            // AddEscalator(new Vec2(1010, 540), new Vec2(1800, 540));
            _spawner.SetRemainingUses(itemUses);
        }

        switch (sceneNumber) {
			case 1: // pretty much sandbox
                /*_movers.Add(new Player(30, new Vec2(200, 200)));
                _movers.Add(new Finish(new Vec2(800, 800)));
                _movers.Add(new Collectable(new Vec2(400, 400)));
                _movers.Add(new Collectable(new Vec2(500, 500)));
                _movers.Add(new Collectable(new Vec2(600, 600)));
                AddChild(new Axe(new Vec2(800, 200)));*/
                

				int[] itemUses = new int[] { 1, 0, 0, 0, 0, 0, 0 }; // this being declared here might break something but it shouldn't
                _spawner.SetRemainingUses(itemUses);
                break;
            case 2: // test level
                itemUses = new int[] { 3, 3, 0, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w1 = new Sprite("assets/wood1.png", addCollider: false);
                w1.SetXY(580, -400);
                wood.AddChild(w1);

                Sprite w2 = new Sprite("assets/wood2.png", addCollider: false);
                w2.SetXY(300, 200);
                wood.AddChild(w2);

                Sprite w3 = new Sprite("assets/wood3.png", addCollider: false);
                w3.SetXY(210, 500);
                wood.AddChild(w3);

                Sprite w4 = new Sprite("assets/wood3.png", addCollider: false);
                w4.SetXY(210, -500);
                wood.AddChild(w4);

                Sprite w5 = new Sprite("assets/wood2.png", addCollider: false);
                w5.SetXY(950, 350);
                wood.AddChild(w5);

                /*_movers.Add(new Player(30, new Vec2(700, 100)));
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
                _movers.Add(new Finish2(new Vec2(width-100, 500)));*/
                break;
            case 3:
                itemUses = new int[] { 2, 4, 2, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w6 = new Sprite("assets/wood2.png", addCollider: false);
                w6.SetXY(600, 450);
                wood.AddChild(w6);

                /*_movers.Add(new Player(30, new Vec2(1750, 150)));
                AddLine(new Vec2(1600, 0), new Vec2(1600, 300));
                AddLine(new Vec2(1300, 457), new Vec2(1300, 1080));
                AddLine(new Vec2(950, 0), new Vec2(950, 520));
                _movers.Add(new Finish(new Vec2(350, 1000)));*/
                break;
            case 4:
                itemUses = new int[] { 3, 5, 2, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w7 = new Sprite("assets/wood1.png", addCollider: false);
                w7.SetXY(1300, -300);
                w7.Mirror(true,false);
                wood.AddChild(w7);

                Sprite w8 = new Sprite("assets/wood2.png", addCollider: false);
                w8.SetXY(850, -300);
                wood.AddChild(w8);

                Sprite w9 = new Sprite("assets/wood3.png", addCollider: false);
                w9.SetXY(760, 450);
                wood.AddChild(w9);

                Sprite w10 = new Sprite("assets/wood2.png", addCollider: false);
                w10.rotation = 90;
                w10.SetXY(850, 580);
                wood.AddChild(w10);

                /*_movers.Add(new Finish(new Vec2(1273, 993)));
                AddLine(new Vec2(1050, 634), new Vec2(1050, 1080));
                AddLine(new Vec2(750, 0), new Vec2(750, 524));
                AddLine(new Vec2(613, 295), new Vec2(750, 176));
                AddLine(new Vec2(400, 0), new Vec2(400, 100));
                AddLine(new Vec2(400, 450), new Vec2(400, 1080));
                _movers.Add(new Player(30, new Vec2(666, 50)));*/
                break;
            case 5:
                itemUses = new int[] { 2, 4, 2, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w11 = new Sprite("assets/wood2.png", addCollider: false);
                w11.SetXY(700, 330);
                wood.AddChild(w11);

                /*_movers.Add(new Player(30, new Vec2(109, 645)));
                AddLine(new Vec2(711, 712), new Vec2(714, 1069));
                _movers.Add(new Finish(new Vec2(1297, 1000)));
                _movers.Add(new Enemy(20, new Vec2(1080, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1150, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1220, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1400, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1470, 1030)));
                _movers.Add(new Enemy(20, new Vec2(1540, 1030)));
                _movers.Add(new Bomb(new Vec2(84, 1048)));*/
                break;
            case 6:
                itemUses = new int[] { 2, 4, 2, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w12 = new Sprite("assets/wood3.png", addCollider: false);
                w12.rotation = 30;
                w12.SetXY(920, 400);
                wood.AddChild(w12);

                Sprite w13 = new Sprite("assets/wood2.png", addCollider: false);
                w13.rotation = 320;
                w13.SetXY(880, 700);
                wood.AddChild(w13);

                /*AddLine(new Vec2(106, 319), new Vec2(999, 649));
                _movers.Add(new Player(30, new Vec2(286, 242)));
                AddLine(new Vec2(115, 278), new Vec2(1445, 825));
                _movers.Add(new Bomb(new Vec2(1443, 816)));
                //_movers.Add(new Finish(new Vec2(1400, 333)));
                _movers.Add(new ShootingEnemy(new Vec2(width/2, 200), new Vec2(width/2, 1920)));*/
                break;
            case 7:
                itemUses = new int[] { 5, 8, 3, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w14 = new Sprite("assets/wood3.png", addCollider: false);
                w14.rotation = 90;
                w14.SetXY(1180, 380);
                wood.AddChild(w14);

                Sprite w15 = new Sprite("assets/wood3.png", addCollider: false);
                w15.rotation = 90;
                w15.SetXY(1600, 650);
                wood.AddChild(w15);

                Sprite w16 = new Sprite("assets/wood3.png", addCollider: false);
                w16.rotation = 90;
                w16.SetXY(1950, 430);
                wood.AddChild(w16);

                Sprite w17 = new Sprite("assets/wood1.png", addCollider: false);
                w17.SetXY(930, -330);
                w17.Mirror(true, false);
                wood.AddChild(w17);

                /*AddLine(new Vec2(943, 6), new Vec2(941, 619));
                AddLine(new Vec2(1438, 6), new Vec2(1438, 232));
                AddLine(new Vec2(1435, 233), new Vec2(1334, 318));
                AddLine(new Vec2(1334, 318), new Vec2(1547, 318));
                AddLine(new Vec2(1436, 230), new Vec2(1547, 315));
                AddLine(new Vec2(1654, 660), new Vec2(1654, 1002));
                AddLine(new Vec2(1538, 662), new Vec2(1539, 999));
                _movers.Add(new Enemy(20, new Vec2(1728, 640)));
                _movers.Add(new Enemy(20, new Vec2(1650, 642)));
                _movers.Add(new Enemy(20, new Vec2(1587, 646)));
                _movers.Add(new Enemy(20, new Vec2(1533, 651)));
                _movers.Add(new Enemy(20, new Vec2(1459, 656)));
                AddLine(new Vec2(120, 564), new Vec2(351, 563));
                AddLine(new Vec2(411, 572), new Vec2(129, 981));
                _movers.Add(new Enemy(20, new Vec2(369, 515)));
                _movers.Add(new Enemy(20, new Vec2(437, 559)));
                _movers.Add(new Enemy(20, new Vec2(470, 632)));
                _movers.Add(new Player(30, new Vec2(1508, 153)));
                _movers.Add(new Finish(new Vec2(235, 506)));*/
                break;
            case 8:
                itemUses = new int[] { 4, 6, 2, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w18 = new Sprite("assets/wood1.png", addCollider: false);
                w18.SetXY(730, 370);
                w18.Mirror(true, false);
                wood.AddChild(w18);

                Sprite w19 = new Sprite("assets/wood2.png", addCollider: false);
                w19.rotation = 30;
                w19.SetXY(1670, 0);
                wood.AddChild(w19);

                Sprite w20 = new Sprite("assets/wood2.png", addCollider: false);
                w20.rotation = 330;
                w20.SetXY(-40, 0);
                wood.AddChild(w20);

                break;
            case 9:
                itemUses = new int[] { 4, 6, 2, 0, 0, 0, 0 };
                _spawner.SetRemainingUses(itemUses);

                Sprite w21 = new Sprite("assets/wood1.png", addCollider: false);
                w21.SetXY(850, 620);
                w21.rotation = 90;
                w21.Mirror(false, true);
                wood.AddChild(w21);

                Sprite w22 = new Sprite("assets/wood2.png", addCollider: false);
                w22.rotation = 90;
                w22.SetXY(860, 160);
                wood.AddChild(w22);

                Sprite w23 = new Sprite("assets/wood3.png", addCollider: false);
                w23.rotation = 90;
                w23.SetXY(920, 320);
                wood.AddChild(w23);

                break;
            default: // level making
                itemUses = new int[] { 99, 99, 99, 99, 1, 1, 1 };
                // AddEscalator(new Vec2(1000, 540), new Vec2(200, 540), reverse:true);
                // AddEscalator(new Vec2(1010, 540), new Vec2(1800, 540));
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

    void WriteLevelToFile()
    {
        int lvlNum = Convert.ToInt16(ConfigurationManager.AppSettings.Get("levelsNumber")) + 1;
        string path = "levels/" + lvlNum.ToString() + ".txt";
        File.Create(path).Close();
        Configuration config=ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings["levelsNumber"].Value = lvlNum.ToString();
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
        StreamWriter sw = new StreamWriter(path);
        
        foreach (Ball mover in _movers)
        {
            if(mover.IsMoving()) 
            {
                mover.x = Mathf.Round(mover.x);
                mover.y = Mathf.Round(mover.y);
            }
            if(mover is Player) { sw.WriteLine("0 " + mover.x.ToString() + " " + mover.y.ToString()); }
            else if (mover is Finish) { sw.WriteLine("1 " + mover.x.ToString() + " " + mover.y.ToString()); }
            else if (mover is Collectable) { sw.WriteLine("2 " + mover.x.ToString() + " " + mover.y.ToString()); }
            else if (mover is Player2) { sw.WriteLine("4 " + mover.x.ToString() + " " + mover.y.ToString()); }
            else if (mover is Finish2) { sw.WriteLine("5 " + mover.x.ToString() + " " + mover.y.ToString()); }
            else if (mover is Enemy) { sw.WriteLine("6 " + mover.x.ToString() + " " + mover.y.ToString()); }
            else if (mover is Bomb) { sw.WriteLine("8 " + mover.x.ToString() + " " + mover.y.ToString()); }
        }
        foreach (LineSegment line in _lines)
        {
            sw.WriteLine("7 " + line.start.x.ToString() + " " + line.start.y.ToString() + " " + line.end.x.ToString() + " " + line.end.y.ToString());
        }
        for (int i = GetChildCount() - 1; i > 0; i--)
        {
            GameObject obj = GetChildren()[i];
            if (obj is Axe) { sw.WriteLine("3 " + obj.x.ToString() + " " + obj.y.ToString()); }
        }
        sw.Close();
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

        Console.WriteLine("Press R to restart the level");
        Console.WriteLine("Press N to load the next level");
        Console.WriteLine("Press A/S/D to select ability controlled by mouse");
        Console.WriteLine("Press left mouse button to activate the ability");
        Console.WriteLine("A - draw a line segment (first click selects the start, second click selects the end)");
        Console.WriteLine("S - Eraser");
        Console.WriteLine("D - spawns a jump pad");
        Console.WriteLine("Level making tools:");
        Console.WriteLine("P - place a player");
        Console.WriteLine("Z - place a spike");
        Console.WriteLine("F - place finish");
        Console.WriteLine("X - place an axe");
        Console.WriteLine("C - place a star");
        Console.WriteLine("Press Q to save the level to a file");
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
        else if (Input.GetKeyDown (Key.N)) {
            _startSceneNumber += 1;
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
            backgrHUD.visible = false;
        }

        if (Input.GetKeyDown(Key.Q))
        {
            WriteLevelToFile();
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
		// HUD.Fill (100, 100, 100, alpha:100);
        // HUD.Stroke (0, 0, 0);
        // HUD.Ellipse (120, 640, 80, 80);
        // HUD.Ellipse (120, 740, 80, 80);
        // HUD.Ellipse (120, 840, 80, 80);
        if(Input.GetMouseButtonDown(0)) {
            if(Input.mouseX > 60 && Input.mouseX < 140) {
                if(Input.mouseY > 640 && Input.mouseY < 720) {
                    _spawner.SetActiveItem(0);
                }
                else if(Input.mouseY > 740 && Input.mouseY < 820) {
                    _spawner.SetActiveItem(1);
                }
                else if(Input.mouseY > 840 && Input.mouseY < 920) {
                    _spawner.SetActiveItem(2);
                }
            }
        }
        int[] uitext = _spawner.GetRemainingUses();
        HUD.Fill(255, 255, 255);
        HUD.TextFont("Comic Sans MS", 30);
        HUD.color = 0xff452D09;
        HUD.Text("Level " + _startSceneNumber.ToString(), 90, 130);
        HUD.TextFont("Comic Sans MS", 20);
        HUD.Text(uitext[0].ToString() + "x", 140, 360);
        HUD.Text(uitext[1].ToString() + "x", 140, 510);
        HUD.Text(uitext[2].ToString() + "x", 140, 660);


       
    }

	static void Main() {
		new MyGame().Start();
	}
}