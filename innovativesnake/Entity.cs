using Raylib_cs;

class Entity
{
    public int X;
    public int Y;
    public int DirX;
    public int DirY;
    public Color Color;
    public bool Alive = true;
    public bool IsPlayer;

    public Entity(int x, int y, int dirX, int dirY, Color color, bool isPlayer)
    {
        X = x;
        Y = y;
        DirX = dirX;
        DirY = dirY;
        Color = color;
        IsPlayer = isPlayer;
    }
}