using Raylib_cs;

class Entity
{
    public int X;
    public int Y;
    public int DirX;
    public int DirY;
    public Color Color;      // kop (helder)
    public Color TrailColor; // spoor (donkerder)
    public bool Alive = true;
    public bool IsPlayer;

    public Entity(int x, int y, int dirX, int dirY, Color color, bool isPlayer)
    {
        X = x;
        Y = y;
        DirX = dirX;
        DirY = dirY;
        IsPlayer = isPlayer;

        // kop = heldere kleur
        Color = color;

        // spoor = donkerdere versie van dezelfde kleur
        TrailColor = new Color(
            (byte)(color.R / 2),
            (byte)(color.G / 2),
            (byte)(color.B / 2),
            (byte)255
        );
    }
}