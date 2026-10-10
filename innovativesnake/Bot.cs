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

    public void ChooseDirection(HashSet<(int x, int y)> trail, int cols, int rows)
    {
        if (!Alive) return;

        List<(int dx, int dy)> options = new List<(int dx, int dy)>
        {
            (0, -1), (0, 1), (-1, 0), (1, 0)
        };

        // geen 180 graden
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
        {
            // geen veilige zet → random (doodlopend)
            if (options.Count > 0)
            {
                var pick = options[Rng.Next(options.Count)];
                DirX = pick.dx;
                DirY = pick.dy;
            }
            return;
        }

        if (Style == BotStyle.Random)
        {
            var pick = safe[Rng.Next(safe.Count)];
            DirX = pick.dx;
            DirY = pick.dy;
        }
        else if (Style == BotStyle.Cautious)
        {
            // blijf rechtdoor als dat kan
            bool straightOk = false;
            foreach (var s in safe)
            {
                if (s.dx == DirX && s.dy == DirY)
                {
                    straightOk = true;
                    break;
                }
            }
            if (!straightOk)
            {
                var pick = safe[Rng.Next(safe.Count)];
                DirX = pick.dx;
                DirY = pick.dy;
            }
        }
        else // Aggressive: kies random safe (later uit te breiden naar speler zoeken)
        {
            var pick = safe[Rng.Next(safe.Count)];
            DirX = pick.dx;
            DirY = pick.dy;
        }
    }
}