using Raylib_cs;

Raylib.InitWindow(800, 640, "Lockstep");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawText("Lockstep", 20, 20, 20, Color.White);
    Raylib.EndDrawing();
}

Raylib.CloseWindow();