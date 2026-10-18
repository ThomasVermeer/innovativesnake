using System;
using System.Collections.Generic;
using Raylib_cs;

class Game
{
    public const int CellSize = 20;
    public const int Cols = 40;
    public const int Rows = 30;
    public const int ScreenW = Cols * CellSize;
    public const int ScreenH = Rows * CellSize;

    public Entity Player;
    public List<Bot> Bots = new List<Bot>();
    public Dictionary<(int x, int y), Color> Trail = new Dictionary<(int x, int y), Color>();

    public float MoveTimer = 0f;
    public float MoveInterval = 0.15f;
    public int Level = 1;

    public bool GameOver = false;
    public bool PlayerWon = false;
    public bool DeathAnim = false;
    public float DeathTimer = 0f;
    public int DeathX = 0;
    public int DeathY = 0;

    public Game()
    {
        Reset();
    }

    public void Reset()
    {
        Player = new Entity(Cols / 2, Rows - 3, 0, -1, Color.SkyBlue, true);

        Bots.Clear();
        Bots.Add(new Bot(3, 3, 1, 0, Color.Red, BotStyle.Random, 11 + Level));
        Bots.Add(new Bot(Cols - 4, 3, 0, 1, Color.Green, BotStyle.Cautious, 22 + Level));
        Bots.Add(new Bot(Cols / 2, 3, -1, 0, Color.Orange, BotStyle.Aggressive, 33 + Level));

        Trail.Clear();
        Trail.Add((Player.X, Player.Y), Player.TrailColor);

        foreach (var b in Bots)
            Trail.Add((b.X, b.Y), b.TrailColor);

        MoveTimer = 0f;
        MoveInterval = Math.Max(0.05f, 0.15f - (Level - 1) * 0.01f);

        GameOver = false;
        PlayerWon = false;
        DeathAnim = false;
        DeathTimer = 0f;
    }

    public void HandleInput()
    {
        if (!Player.Alive || GameOver || DeathAnim) return;

        bool diagonal = Level >= 10;

        int wantX = Player.DirX;
        int wantY = Player.DirY;

        bool up = Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W);
        bool down = Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S);
        bool left = Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A);
        bool right = Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D);

        if (diagonal)
        {
            if (up && left) { wantX = -1; wantY = -1; }
            else if (up && right) { wantX = 1; wantY = -1; }
            else if (down && left) { wantX = -1; wantY = 1; }
            else if (down && right) { wantX = 1; wantY = 1; }
            else if (up) { wantX = 0; wantY = -1; }
            else if (down) { wantX = 0; wantY = 1; }
            else if (left) { wantX = -1; wantY = 0; }
            else if (right) { wantX = 1; wantY = 0; }
        }
        else
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
            { wantX = 0; wantY = -1; }
            else if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
            { wantX = 0; wantY = 1; }
            else if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A))
            { wantX = -1; wantY = 0; }
            else if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D))
            { wantX = 1; wantY = 0; }
        }

        // geen 180°
        if (wantX == -Player.DirX && wantY == -Player.DirY)
            return;

        Player.DirX = wantX;
        Player.DirY = wantY;
    }

    void StartDeath(int x, int y)
    {
        DeathX = x;
        DeathY = y;
        DeathTimer = 0f;
        DeathAnim = true;
        Player.Alive = false;
    }

    public void Update(float dt)
    {
        if (DeathAnim)
        {
            DeathTimer += dt;
            if (DeathTimer >= 3f)
            {
                DeathAnim = false;
                GameOver = true;
                PlayerWon = false;
            }
            return;
        }

        if (GameOver) return;

        HandleInput();

        MoveTimer += dt;
        if (MoveTimer >= MoveInterval)
        {
            MoveTimer = 0f;
            MoveAll();
            CheckWin();
        }
    }

    void MoveAll()
    {
        bool diagonal = Level >= 10;

        foreach (var bot in Bots)
        {
            if (bot.Alive)
                bot.ChooseDirection(Trail, Cols, Rows, Player.X, Player.Y, diagonal);
        }

        if (Player.Alive)
            TryMove(Player);

        foreach (var bot in Bots)
        {
            if (bot.Alive)
                TryMove(bot);
        }
    }


    void TryMove(Entity e)
    {
        int nx = e.X + e.DirX;
        int ny = e.Y + e.DirY;

        if (nx < 0 || ny < 0 || nx >= Cols || ny >= Rows || Trail.ContainsKey((nx, ny)))
        {
            e.Alive = false;
            if (e.IsPlayer)
                StartDeath(e.X, e.Y);
            return;
        }

        e.X = nx;
        e.Y = ny;
        Trail.Add((nx, ny), e.TrailColor);
    }


    void CheckWin()
    {
        if (!Player.Alive) return;

        int botsAlive = 0;
        foreach (var b in Bots)
            if (b.Alive) botsAlive++;

        if (botsAlive == 0)
        {
            GameOver = true;
            PlayerWon = true;
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        foreach (var cell in Trail)
        {
            Raylib.DrawRectangle(
                cell.Key.x * CellSize,
                cell.Key.y * CellSize,
                CellSize,
                CellSize,
                cell.Value
            );
        }

        if (Player.Alive && !DeathAnim)
            Raylib.DrawRectangle(Player.X * CellSize, Player.Y * CellSize, CellSize, CellSize, Player.Color);

        foreach (var bot in Bots)
        {
            if (bot.Alive)
                Raylib.DrawRectangle(bot.X * CellSize, bot.Y * CellSize, CellSize, CellSize, bot.Color);
        }

        if (DeathAnim)
        {
            int cx = DeathX * CellSize + CellSize / 2;
            int cy = DeathY * CellSize + CellSize / 2;
            float t = DeathTimer / 3f;
            int radius = (int)(10 + t * 40);
            Raylib.DrawCircleLines(cx, cy, radius, Color.Red);
            Raylib.DrawCircleLines(cx, cy, radius + 8, Color.Orange);
            Raylib.DrawRectangle(DeathX * CellSize, DeathY * CellSize, CellSize, CellSize, Color.Red);
        }

        Raylib.DrawText($"Level {Level}", 10, 10, 20, Color.White);
        if (Level >= 10)
            Raylib.DrawText("Diagonalen AAN (schuin met 2 toetsen)", 10, 40, 18, Color.Orange);
        else
            Raylib.DrawText("WASD / Pijltjes | Enter = opnieuw", 10, 40, 18, Color.Gray);

        if (GameOver)
        {
            if (PlayerWon)
                Raylib.DrawText("JE WINT! Enter = volgend level", 220, 280, 24, Color.Green);
            else
                Raylib.DrawText("GAME OVER - Enter = opnieuw", 220, 280, 24, Color.Yellow);
        }

        Raylib.EndDrawing();
    }
}