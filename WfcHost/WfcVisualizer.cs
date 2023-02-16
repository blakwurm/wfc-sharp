using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WfcLib;

namespace WfcHost;

public partial class WfcVisualizer : SpriteSheet
{
    private ulong[,] _grid;
    private SpriteSheet _modal;
    
    public new ulong[,] Grid
    {
        get => _grid;
        set
        {
            _grid = value;
            if (_grid == null) return;
            base.Grid = _grid.Cast<ulong>().Select(x => x.BitCount() > 1 ? 0 : x.GetState() + 1).ToArray();
        }
    }
    
    public WfcVisualizer()
    {
        InitializeComponent();
    }

    private int[] GetPossibleStates(ulong states)
    {
        var possibleStates = new List<int>();

        var i = 1;
        while (states > 0)
        {
            if((states & 1) == 1)
                possibleStates.Add(i);
            states >>= 1;
            i++;
        }
        while(possibleStates.Count % 4 > 0 && possibleStates.Count > 4)
            possibleStates.Add(0);

        
        return possibleStates.ToArray();
    }
    
    private void ToggleModal(Point location)
    {
        if (_modal != null)
        {
            _modal.Hide();
            this.Controls.Remove(_modal);
            _modal.Dispose();
            _modal = null;
            return;
        }
        
        var tileX = location.X / (TileSize.Width + BorderSize);
        var tileY = location.Y / (TileSize.Height + BorderSize);
        
        var states = GetPossibleStates(_grid[tileY, tileX]);
        _modal = new SpriteSheet
        {
            Source = Source,
            TileSize = TileSize,
            MapSize = new Size(states.Length > 4 ? 4 : states.Length, states.Length > 4 ? states.Length / 4 : 1),
            Grid = states,
            Location = location
        };
        
        this.Controls.Add(_modal);
        _modal.BringToFront();
    }

    private void WfcVisualizer_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        ToggleModal(e.Location);
    }
}