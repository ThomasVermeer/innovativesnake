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
    static int dirY = -1;

    static HashSet<(int x, int y)> trail = new HashSet<(int x, int y)>();
    static float moveTimer = 0f;
    static float moveInterval = 0.15f;
    static bool gameOver = false;

    static void Main()
    {
        Raylib.InitWindow(ScreenW, ScreenH, "Lockstep - Fase 3 collision");
        Raylib.SetTargetFPS(60);

        trail.Add((playerX, playerY));

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();

            if (!gameOver)
            {
                HandleInput();

                moveTimer += dt;
                if (moveTimer >= moveInterval)
                {
                    moveTimer = 0f;
                    MovePlayer();
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                ResetGame();
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
            dirX = 0;
            dirY = -1;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S)) && dirY != -1)
        {
            dirX = 0;
            dirY = 1;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A)) && dirX != 1)
        {
            dirX = -1;
            dirY = 0;
        }
        else if ((Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D)) && dirX != -1)
        {
            dirX = 1;
            dirY = 0;
        }
    }

    static void MovePlayer()
    {
        int nextX = playerX + dirX;
        int nextY = playerY + dirY;

        // Botsing met de rand
        if (nextX < 0 || nextX >= Cols || nextY < 0 || nextY >= Rows)
        {
            gameOver = true;
            return;
        }

        // Botsing met het bestaande spoor
        if (trail.Contains((nextX, nextY)))
        {
            gameOver = true;
            return;
        }

        playerX = nextX;
        playerY = nextY;

        trail.Add((playerX, playerY));
    }

    static void ResetGame()
    {
        playerX = Cols / 2;
        playerY = Rows - 3;
        dirX = 0;
        dirY = -1;

        trail.Clear();
        trail.Add((playerX, playerY));

        moveTimer = 0f;
        gameOver = false;
    }

    static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        // Permanent spoor
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

        // Speler
        if (!gameOver)
        {
            Raylib.DrawRectangle(
                playerX * CellSize,
                playerY * CellSize,
                CellSize,
                CellSize,
                Color.SkyBlue
            );
        }

        Raylib.DrawText("Fase 3: bewegen + collision", 10, 10, 20, Color.White);
        Raylib.DrawText("WASD / Pijltjes", 10, 40, 18, Color.Gray);

        if (gameOver)
        {
            Raylib.DrawText("GAME OVER", 280, 280, 28, Color.Yellow);
            Raylib.DrawText("Druk op Enter om opnieuw te spelen", 205, 315, 18, Color.White);
        }

        Raylib.EndDrawing();
    }
}