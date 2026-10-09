using Raylib_cs;

class Program
{
    static void Main()
    {
        Raylib.InitWindow(Game.ScreenW, Game.ScreenH, "Lockstep");
        Raylib.SetTargetFPS(60);

        Game game = new Game();

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();

            if (game.GameOver && Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                game.Reset();
            }
            else
            {
                game.Update(dt);
            }

            game.Draw();
        }

        Raylib.CloseWindow();
    }
}