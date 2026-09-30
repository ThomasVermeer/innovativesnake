using System.Collections.Generic;
using Raylib_cs;

class Program
{
    const int CellSize = 20;
    const int Cols = 40;
    const int Rows = 30;
    const int ScreenW = Cols * CellSize;
    const int ScreenH = Rows * CellSize;

    static int playerX = Cols / 2;
    static int playerY = Rows - 3;
    static int dirX = 0;
    static int dirY = -1; // start omhoog

    static HashSet<(int x, int y)> trail = new HashSet<(int x, int y)>();
    static float moveTimer = 0f;
    static float moveInterval = 0.15f;

    static void Main()
    {
        Raylib.InitWindow(ScreenW, ScreenH, "Lockstep - Fase 1");
        Raylib.SetTargetFPS(60);

        trail.Add((playerX, playerY));

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            HandleInput();

            moveTimer += dt;
            if (moveTimer >= moveInterval)
            {
                moveTimer = 0f;
                MovePlayer();
            }

            Draw();
        }

        Raylib.CloseWindow();
    }

    static void HandleInput()
    {
        // Geen 180 graden bochten
        if ((Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W)) && dirY != 1)
        {
            dirX = 0; dirY = -1;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S)) && dirY != -1)
        {
            dirX = 0; dirY = 1;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A)) && dirX != 1)
        {
            dirX = -1; dirY = 0;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D)) && dirX != -1)
        {
            dirX = 1; dirY = 0;
        }
    }

    static void MovePlayer()
    {
        playerX += dirX;
        playerY += dirY;

        // Binnen het scherm houden (tijdelijk, later collision)
        if (playerX < 0) playerX = 0;
        if (playerY < 0) playerY = 0;
        if (playerX >= Cols) playerX = Cols - 1;
        if (playerY >= Rows) playerY = Rows - 1;

        trail.Add((playerX, playerY));
    }

    static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        // permanent spoor
        foreach (var cell in trail)
        {
            Raylib.DrawRectangle(
                cell.x * CellSize,
                cell.y * CellSize,
                CellSize,
                CellSize,
                new Color(40, 40, 80, 255)
            );
        }

        // speler
        Raylib.DrawRectangle(
            playerX * CellSize,
            playerY * CellSize,
            CellSize,
            CellSize,
            Color.SkyBlue
        );

        Raylib.DrawText("Fase 1: bewegen + permanent spoor", 10, 10, 20, Color.White);
        Raylib.DrawText("WASD / Pijltjes", 10, 40, 18, Color.Gray);

        Raylib.EndDrawing();
    }
}