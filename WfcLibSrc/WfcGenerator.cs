using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WfcLib;

public class WfcGenerator
{
    public const ulong Any = ~0UL;
    private List<ulong[]> constraints;
    private byte _len;

    public byte CardCount => _len;

    public WfcGenerator()
    {
        constraints = new List<ulong[]>();
        _len = 0;
    }

    public ulong CreateCard()
    {
        var arr = Enumerable.Repeat(~(ulong)0, 8).ToArray();
        constraints.Add(arr);
        return (ulong)1 << _len++;
    }

    public void ConstrainCard(byte card, ulong[] localConstraints)
    {
        constraints[card] = localConstraints;
    }

    private HashSet<(int, int, ulong)> ConstrainNeighbors(ulong[,] grid, int x, int y, int width, int height)
    {
        var affectedCells = new HashSet<(int, int, ulong)>(new TupleComparer());
        var localConstraints = constraints[grid[y, x].GetState()];
        var offsets = new (int, int)[]
        {
            ( 0, -1),
            ( 1, -1),
            ( 1,  0),
            ( 1,  1),
            ( 0,  1),
            (-1,  1),
            (-1,  0),
            (-1, -1),
        };

        for (int i = 0; i < 8; i++)
        {
            var (dX, dY) = offsets[i];
            var tX = x + dX;
            var tY = y + dY;

            if (tX < 0 || tX >= width ||
                tY < 0 || tY >= height)
            {
                continue;
            }

            var org = grid[tY, tX];
            var res = (grid[tY, tX] &= localConstraints[i]);
            if (res == 0)
            {
                return null;
            }
            affectedCells.Add((tX, tY, res));
        }

        return affectedCells;
    }

    public IEnumerable<ulong[,]> GenerateIncrementally(int width, int height, bool infinite = false)
    {
        Debug.Assert(width > 3 && height > 3, "Minimum grid size is 3x3");
        
        var any = (ulong)1 << _len;
        any--;

        var grid = new ulong[height, width];
        grid.Fill(any);

        grid[width / 2, height / 2] = any.Collapse();
        var stack = ConstrainNeighbors(grid, width / 2, width / 2, width, height);
        var explored = new HashSet<(int, int, ulong)>(new TupleComparer());
        explored.Add((width / 2, height / 2, grid[width / 2, height / 2]));

        yield return grid;

        while (true)
        {
            if (stack.Count == 0)
            {
                if(infinite)
                    yield return grid;

                yield break;
            }
            var (x, y) = stack.FindLeastEntropy();

            grid[y, x] = grid[y, x].Collapse();
            explored.Add((x, y, grid[x, y]));

            var affectedCells = ConstrainNeighbors(grid, x, y, width, height);
            if (affectedCells == null)
            {
                grid.Fill(any);
                stack.Clear();
                continue;
            }
            affectedCells.UnionWith(stack);
            stack = affectedCells;
            stack.ExceptWith(explored);

            yield return grid;
        }
    }

    public int[,] Generate(int width, int height)
    {
        var grid = new ulong[height, width];
        
        foreach(var iteration in GenerateIncrementally(width, height))
        {
            grid = iteration;
        }
        
        return grid.AsIndices();
    }
}

class TupleComparer : IEqualityComparer<(int, int, ulong)>
{
    public bool Equals((int, int, ulong) x, (int, int, ulong) y)
    {
        return x.Item1 == y.Item1 && x.Item2 == y.Item2;
    }

    public int GetHashCode((int, int, ulong) obj)
    {
        return obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
    }
}