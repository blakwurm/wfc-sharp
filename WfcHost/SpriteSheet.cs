using System;
using System.Drawing;
using System.Windows.Forms;

namespace WfcHost
{
    public partial class SpriteSheet : UserControl
    {
        private Image _source;
        private string _path;
        private int _tileWidth, _tileHeight, _atlasWidth, _sheetWidth, _sheetHeight;

        protected const int BorderSize = 1;
        protected readonly Brush BorderColor;

        private int[] _grid;

        public Size MapSize
        {
            get => new Size(_sheetWidth, _sheetHeight);
            set
            {
                _sheetWidth = value.Width;
                _sheetHeight = value.Height;
                _grid = new int[_sheetWidth * _sheetHeight];
                ScaleWindow();
            }
        }

        public Size TileSize
        {
            get => new Size(_tileWidth, _tileHeight);
            set
            {
                _tileWidth = value.Width;
                _tileHeight = value.Height;
                ScaleWindow();
            }
        }
        
        public string Path
        {
            get => _path;
            set
            {
                if (value == null) return;
                _path = value;
                LoadSprites(_path);
            }
        }
        
        public Image Source
        {
            get => _source;
            set
            {
                if (value == null) return;
                _source = value;
                LoadSprites(_source);
            }
        }

        public int[] Grid
        {
            get => _grid;
            set
            {
                if (value == null) return;
                _grid = value;
                Invalidate();
            }
        }
        
        public SpriteSheet()
        {
            InitializeComponent();
            
            BorderColor = Brushes.DimGray;
            _tileWidth = 32;
            _tileHeight = 32;
            _sheetWidth = 4;
            _sheetHeight = 4;
            
            _grid = new int[_sheetWidth * _sheetHeight];

            ScaleWindow();
        }

        public void ScaleWindow()
        {
            this.Size = new Size(
                BorderSize + ((BorderSize + _tileWidth) * _sheetWidth),
                BorderSize + ((BorderSize + _tileHeight) * _sheetHeight)
                );
            
            Invalidate();
        }

        private void DefaultGrid()
        {
            for (var x = 0; x < _grid.Length; x++)
            {
                _grid[x] = x % _atlasWidth;
                _grid[x]++;
            }
        }
        
        public void LoadSprites(Image source, int tileWidth, int tileHeight)
        {
            _source = source;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _atlasWidth = source.Width / tileWidth;
            
            DefaultGrid();

            ScaleWindow();
        }

        public void LoadSprites(string path, int tileWidth, int tileHeight)
        {
            var img = Image.FromFile(path);
            LoadSprites(img, tileWidth, tileHeight);
        }

        public void LoadSprites(string path)
        {
            LoadSprites(path, _tileWidth, _tileHeight);
        }

        public void LoadSprites(Image source)
        {
            LoadSprites(source, _tileWidth, _tileHeight);
        }

        private void DrawGrid(Graphics graphics)
        {
            Pen pen = new Pen(BorderColor, 1);
            for (var x = 0; x < _sheetWidth; x++)
            {
                graphics.DrawLine(pen, x * (_tileWidth + BorderSize), 0, x * (_tileWidth + BorderSize), Height);
            }
            graphics.DrawLine(pen, Width - 1, 0, Width - 1, Height);
            for (var y = 0; y < _sheetHeight; y++)
            {
                graphics.DrawLine(pen, 0, y * (_tileHeight + BorderSize), Width, y * (_tileHeight + BorderSize));
            }
            graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
        }

        private Rectangle GetSourceRect(int index)
        {
            index--;
            var x = index % _atlasWidth;
            var y = index / _atlasWidth;

            return new Rectangle(
                x * _tileWidth,
                y * _tileHeight,
                _tileWidth,
                _tileHeight
            );
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            DrawGrid(graphics);

            if (_source == null) return;

            
            for (var x = 0; x < _sheetWidth; x++)
            for (var y = 0; y < _sheetHeight; y++)
            {
                var index = _grid[x + y * _sheetWidth];

                var destRect = new Rectangle(BorderSize + (x * (_tileWidth + BorderSize)),
                    BorderSize + (y * (_tileHeight + BorderSize)), _tileWidth, _tileHeight);
                
                if (index < 1)
                {
                    graphics.FillRectangle(Brushes.Aqua, destRect);
                    continue;
                }
                
                var sourceRect = GetSourceRect((int)index);
                graphics.DrawImage(_source, destRect, sourceRect, GraphicsUnit.Pixel);
            }
        }
    }
}