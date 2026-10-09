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
    public HashSet<(int x, int y)> Trail = new HashSet<(int x, int y)>();

    public float MoveTimer = 0f;
    public float MoveInterval = 0.15f;

    public bool GameOver = false;
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
        Trail.Clear();
        Trail.Add((Player.X, Player.Y));
        MoveTimer = 0f;
        GameOver = false;
        DeathAnim = false;
        DeathTimer = 0f;
    }

    public void HandleInput()
    {
        if (!Player.Alive || GameOver || DeathAnim) return;

        if ((Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W)) && Player.DirY != 1)
        {
            Player.DirX = 0;
            Player.DirY = -1;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S)) && Player.DirY != -1)
        {
            Player.DirX = 0;
            Player.DirY = 1;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A)) && Player.DirX != 1)
        {
            Player.DirX = -1;
            Player.DirY = 0;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D)) && Player.DirX != -1)
        {
            Player.DirX = 1;
            Player.DirY = 0;
        }
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
            }
            return;
        }

        if (GameOver) return;

        HandleInput();

        MoveTimer += dt;
        if (MoveTimer >= MoveInterval)
        {
            MoveTimer = 0f;
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        int nextX = Player.X + Player.DirX;
        int nextY = Player.Y + Player.DirY;

        if (nextX < 0 || nextX >= Cols || nextY < 0 || nextY >= Rows)
        {
            StartDeath(Player.X, Player.Y);
            return;
        }

        if (Trail.Contains((nextX, nextY)))
        {
            StartDeath(nextX, nextY);
            return;
        }

        Player.X = nextX;
        Player.Y = nextY;
        Trail.Add((nextX, nextY));
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        foreach (var cell in Trail)
        {
            Raylib.DrawRectangle(
                cell.x * CellSize,
                cell.y * CellSize,
                CellSize,
                CellSize,
                new Color(40, 40, 80, 255)
            );
        }

        if (Player.Alive && !DeathAnim)
        {
            Raylib.DrawRectangle(
                Player.X * CellSize,
                Player.Y * CellSize,
                CellSize,
                CellSize,
                Player.Color
            );
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

        Raylib.DrawText("Fase 4: classes (klaar voor bots)", 10, 10, 20, Color.White);
        Raylib.DrawText("WASD / Pijltjes", 10, 40, 18, Color.Gray);

        if (GameOver)
        {
            Raylib.DrawText("GAME OVER", 280, 280, 28, Color.Yellow);
            Raylib.DrawText("Druk op Enter om opnieuw te spelen", 205, 315, 18, Color.White);
        }

        Raylib.EndDrawing();
    }
}