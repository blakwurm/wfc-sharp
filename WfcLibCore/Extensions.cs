namespace WfcLib;

public static class Extensions
{
    public static void Fill<T>(this T[,] arr, T value)
    {
        for(int x = 0; x < arr.GetLength(0); x++)
        {
            for(int y = 0; y < arr.GetLength(1); y++)
            {
                arr[x, y] = value;
            }
        }
    }

    public static (int, int) FindLeastEntropy(this ulong[,] arr)
    {
        byte count = 255;
        var index = (-1, -1);
        
        for(int y = 0; y < arr.GetLength(1); y++)
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            var c = arr[x, y].BitCount();
            if (c < count && c > 1)
            {
                count = c;
                index = (x, y);
            }
        }

        return index;
    }

    public static int[,] AsIndices(this ulong[,] arr)
    {
        var width = arr.GetLength(0);
        var height = arr.GetLength(1);
        var ret = new int[width, height];
        for(int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            ret[x, y] = arr[x, y].GetState();
        }

        return ret;
    }

    public static byte BitCount(this ulong value)
    {
        if (System.Runtime.Intrinsics.X86.Popcnt.X64.IsSupported)
            return (byte)System.Runtime.Intrinsics.X86.Popcnt.X64.PopCount(value);

        byte count = 0;
        while (value > 0)
        {
            count += (byte)(value & 1);
            value >>= 1;
        }

        return count;
    }

    public static int GetState(this ulong value)
    {
        int count = 0;
        while (value > 0)
        {
            if (value == 1) return count;
            value >>= 1;
            count++;
        }

        return -1;
    }

    public static int SampleStates(this ulong value)
    {
        int[] indices = Enumerable.Range(0, 64).Where(i => ((value >> i) & 1) == 1).ToArray();
        return indices[(new Random()).Next(indices.Length)];
    }

    public static ulong Collapse(this ulong value)
    {
        return (ulong)1 << value.SampleStates();
    }
}