using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

    public static T Sample<T>(this IEnumerable<T> source)
    {
        var enumerable = source as T[] ?? source.ToArray();
        return enumerable.ElementAt(new Random().Next(enumerable.Length));
    }

    public static (int, int) FindLeastEntropy(this HashSet<(int, int, ulong)> stack)
    {
        var leastEntropy = stack.OrderBy(x => x.Item3.BitCount()).First().Item3.BitCount();
        var item = stack.Where(x=> x.Item3.BitCount() == leastEntropy).Sample();
        return (item.Item1, item.Item2);
    }

    public static (int, int) FindLeastEntropy(this ulong[,] arr)
    {
        byte count = 255;
        var indices = new List<(int, int)>();
        
        for(int y = 0; y < arr.GetLength(1); y++)
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            var c = arr[y, x].BitCount();
            if (c < count && c > 1)
            {
                count = c;
                indices.Clear();
            }

            if (c == count)
            {
                indices.Add((x, y));
            }
        }

        if (indices.Count < 1)
        {
            return (-1, -1);
        }

        return indices.Sample();
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
        #if NETCOREAPP
            if (System.Runtime.Intrinsics.X86.Popcnt.X64.IsSupported)
                return (byte)System.Runtime.Intrinsics.X86.Popcnt.X64.PopCount(value);
        #endif
        
        ulong result = value - ((value >> 1) & 0x5555555555555555UL);
        result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
        return (byte)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
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