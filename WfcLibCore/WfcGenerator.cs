namespace WfcLib;

public class WfcGenerator
{
    private List<ulong[]> constraints;
    private byte _len;

    public WfcGenerator()
    {
        constraints = new List<ulong[]>();
        _len = 0;
    }

    public ulong CreateCard()
    {
        var arr = new ulong[8];
        Array.Fill(arr, ~(ulong)0);
        return (ulong)1 << _len++;
    }

    public void ConstrainCard(byte card, ulong[] localConstraints)
    {
        constraints[card] = localConstraints;
    }

    private bool ConstrainNeighbors(ulong[,] grid, int x, int y, int width, int height)
    {
        var localConstraints = constraints[grid[x, y].GetState()];
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

            if ((grid[tX, tY] &= localConstraints[i]) == 0)
            {
                return false;
            }
        }

        return true;
    }

    public int[,] gen(int width, int height)
    {
        var any = (ulong)1 << _len;
        any--;

        var grid = new ulong[width, height];
        grid.Fill(any);

        while (true)
        {
            var (x, y) = grid.FindLeastEntropy();
            if (x < 0)
            {
                break;
            } 
            
            grid[x, y] = grid[x, y].Collapse();

            if (!ConstrainNeighbors(grid, x, y, width, height))
            {
                grid.Fill(any);
            }
        }

        return grid.AsIndices();
    }
}