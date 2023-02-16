using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WfcLib;

public struct Card
{
    static List<Card> _cards = new();
    
    public int[] Sockets;
    public ulong Id;
    
    private Card(ulong id, int[] sockets)
    {
        Id = id;
        Sockets = sockets;
    }

    public ulong[] GetConstraints()
    {
        var constraints = new ulong[8];
        for (var i = 0; i < 4; i++)
        {
            constraints[i*2] = GetConstraint(i);
            constraints[i*2+1] = WfcGenerator.Any;
        }

        return constraints;
    }

    public ulong GetConstraint(int direction)
    {
        var socket = Sockets[direction];
        direction = (direction + 2) % 4;
        
        var id = Id;
        return _cards.Where(x=> x.Sockets[direction] == socket && x.Id != id).Aggregate(0UL, (acc, x) => acc | x.Id);
    }
    
    public static Card WrapCard(ulong id, int[] sockets)
    {
        var card = new Card(id, sockets);
        _cards.Add(card);
        return card;
    }

    public static void ConstrainCards(WfcGenerator wfc)
    {
        foreach (Card card in _cards)
        {
            wfc.ConstrainCard((byte)card.Id.GetState(), card.GetConstraints());
        }
    }
}

namespace WfcHost
{
    public partial class Form1 : Form
    {
        private WfcGenerator wfc;
        private IEnumerator<ulong[,]> generator = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void FinishWfc()
        {
            if(generator == null || !generator.MoveNext()) ResetWfc();
            generator.MoveNext();
            ulong[,] grid = generator.Current!;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (generator.MoveNext()) { }
            MessageBox.Show("WFC finished in " + stopwatch.ElapsedMilliseconds + "ms");

            wfcVisualizer1.MapSize = new Size(grid.GetLength(1), grid.GetLength(0));
            wfcVisualizer1.Grid = grid;
        }

        private void Wfc()
        {
            if (generator == null || !generator.MoveNext())
            {
                ResetWfc();
            }

            var grid = generator.Current!;
            wfcVisualizer1.MapSize = new Size(grid.GetLength(1), grid.GetLength(0));
            wfcVisualizer1.Grid = grid;
        }

        private void DefaultGrid()
        {
            var sourceSize = wfcVisualizer1.Source.Size;
            var tileSize = wfcVisualizer1.TileSize;
            var gridSize = new Size(sourceSize.Width / tileSize.Width, sourceSize.Height / tileSize.Height);
            var grid = new ulong[gridSize.Height, gridSize.Width];
            wfcVisualizer1.MapSize = gridSize;
            for(var i = 0; i < gridSize.Width * gridSize.Height; i++)
                grid[i / gridSize.Width, i % gridSize.Width] = (1UL << i) + 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                wfcVisualizer1.Path = ofd.FileName;
                DefaultGrid();
            }

            Focus();
        }
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            Wfc();
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Wfc();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    Wfc();
                    break;
                case Keys.Escape:
                    ResetWfc();
                    break;
                case Keys.L:
                    LoadAtlas();
                    break;
                case Keys.F:
                    FinishWfc();
                    break;
            }
        }

        private void LoadAtlas()
        {
        }

        private void ResetWfc()
        {
            (wfc, _) = Utils.BuildWfc();
            generator = wfc.GenerateIncrementally(201, 201).GetEnumerator();
            Wfc();
        }
    }
}