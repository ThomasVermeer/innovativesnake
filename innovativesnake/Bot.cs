using System;
using System.Collections.Generic;
using Raylib_cs;

enum BotStyle
{
    Random,
    Cautious,
    Aggressive
}

class Bot : Entity
{
    public BotStyle Style;
    public Random Rng;

    public Bot(int x, int y, int dirX, int dirY, Color color, BotStyle style, int seed)
        : base(x, y, dirX, dirY, color, false)
    {
        Style = style;
        Rng = new Random(seed);
    }

    public void ChooseDirection(HashSet<(int x, int y)> trail, int cols, int rows, int playerX, int playerY)
    {
        if (!Alive) return;

        List<(int dx, int dy)> options = new List<(int dx, int dy)>
    {
        (0, -1), (0, 1), (-1, 0), (1, 0)
    };

        // geen 180°
        options.RemoveAll(o => o.dx == -DirX && o.dy == -DirY);

        List<(int dx, int dy)> safe = new List<(int dx, int dy)>();
        foreach (var o in options)
        {
            int nx = X + o.dx;
            int ny = Y + o.dy;
            if (nx >= 0 && ny >= 0 && nx < cols && ny < rows && !trail.Contains((nx, ny)))
                safe.Add(o);
        }

        if (safe.Count == 0)
            return; // geen zet → TryMove maakt Alive = false

        bool straightSafe = false;
        foreach (var s in safe)
        {
            if (s.dx == DirX && s.dy == DirY)
            {
                straightSafe = true;
                break;
            }
        }

        if (Style == BotStyle.Cautious)
        {
            if (straightSafe)
                return; // blijf rechtdoor

            var pick = safe[Rng.Next(safe.Count)];
            DirX = pick.dx;
            DirY = pick.dy;
            return;
        }

        if (Style == BotStyle.Random)
        {
            // 75% rechtdoor als dat kan, anders random safe
            if (straightSafe && Rng.Next(100) < 75)
                return;

            var pick = safe[Rng.Next(safe.Count)];
            DirX = pick.dx;
            DirY = pick.dy;
            return;
        }

        // Aggressive: kies safe richting die dichter bij speler komt
        (int dx, int dy) best = safe[0];
        int bestScore = int.MaxValue;

        foreach (var s in safe)
        {
            int nx = X + s.dx;
            int ny = Y + s.dy;
            int dist = Math.Abs(nx - playerX) + Math.Abs(ny - playerY);
            if (dist < bestScore)
            {
                bestScore = dist;
                best = s;
            }
        }

        DirX = best.dx;
        DirY = best.dy;
    }
}