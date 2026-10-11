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
    public HashSet<(int x, int y)> Trail = new HashSet<(int x, int y)>();

    public float MoveTimer = 0f;
    public float MoveInterval = 0.15f;

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
        Bots.Add(new Bot(3, 3, 1, 0, Color.Red, BotStyle.Random, 11));
        Bots.Add(new Bot(Cols - 4, 3, 0, 1, Color.Green, BotStyle.Cautious, 22));
        Bots.Add(new Bot(Cols / 2, 3, -1, 0, Color.Orange, BotStyle.Aggressive, 33));

        Trail.Clear();
        Trail.Add((Player.X, Player.Y));
        foreach (var b in Bots)
            Trail.Add((b.X, b.Y));

        MoveTimer = 0f;
        GameOver = false;
        PlayerWon = false;
        DeathAnim = false;
        DeathTimer = 0f;
    }

    public void HandleInput()
    {
        if (!Player.Alive || GameOver || DeathAnim) return;

        if ((Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W)) && Player.DirY != 1)
        { Player.DirX = 0; Player.DirY = -1; }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S)) && Player.DirY != -1)
        { Player.DirX = 0; Player.DirY = 1; }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A)) && Player.DirX != 1)
        { Player.DirX = -1; Player.DirY = 0; }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D)) && Player.DirX != -1)
        { Player.DirX = 1; Player.DirY = 0; }
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
        foreach (var bot in Bots)
        {
            if (bot.Alive)
                bot.ChooseDirection(Trail, Cols, Rows, Player.X, Player.Y);
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

        if (nx < 0 || ny < 0 || nx >= Cols || ny >= Rows || Trail.Contains((nx, ny)))
        {
            e.Alive = false;
            if (e.IsPlayer)
                StartDeath(e.X, e.Y);
            return;
        }

        e.X = nx;
        e.Y = ny;
        Trail.Add((nx, ny));
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
            Raylib.DrawRectangle(cell.x * CellSize, cell.y * CellSize, CellSize, CellSize, new Color(40, 40, 80, 255));
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

        Raylib.DrawText("Fase 4: 3 bots (random / cautious / aggressive)", 10, 10, 20, Color.White);
        Raylib.DrawText("WASD / Pijltjes | Enter = opnieuw", 10, 40, 18, Color.Gray);

        if (GameOver)
        {
            if (PlayerWon)
                Raylib.DrawText("JE WINT!", 300, 280, 28, Color.Green);
            else
                Raylib.DrawText("GAME OVER", 280, 280, 28, Color.Yellow);

            Raylib.DrawText("Druk op Enter om opnieuw te spelen", 205, 315, 18, Color.White);
        }

        Raylib.EndDrawing();
    }
}