using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PatternMaker
{
    internal class ViewModel
    {
        public static readonly int SQUARE_SIZE = 15;
        public static readonly double SQUARE_OPACITY = 0.6;
        public static readonly double SQUARE_THICKNESS = 0.2;
        public static readonly SolidColorBrush DEFAULT_FILL = Brushes.White;

        private readonly Canvas _patternCanvas;
        private PatternModel _patternModel;

        public BitmapImage BGImage { get; set; }

        private string bGFilename;
        private MouseButtonEventHandler _clickEvent;
        private MouseButtonEventHandler _rightClickEvent;

        public static Rectangle CreateRectangle()
        {
            return new Rectangle()
            {
                Stroke = Brushes.Black,
                Fill = DEFAULT_FILL,
                StrokeThickness = SQUARE_THICKNESS,
                Width = SQUARE_SIZE,
                Height = SQUARE_SIZE,
                Opacity = SQUARE_OPACITY
            };
        }

        public ViewModel(Canvas patternCanvas, MouseButtonEventHandler onRectClick, MouseButtonEventHandler onRectRightClick)
        {
            _patternCanvas = patternCanvas;
            _patternModel = new PatternModel();
            _clickEvent = onRectClick;
            _rightClickEvent = onRectRightClick;
            Row = 96;
            Col = 256;
            Zoom = 100;
        }

        public string GetBGFilename()
        {
            return bGFilename;
        }

        public void SetBGFilename(string value)
        {
            bGFilename = value;
            BGImage = new BitmapImage();
            BGImage.BeginInit();
            BGImage.UriSource = new Uri(value);
            BGImage.EndInit();
        }

        public int Row { get { return _patternModel.Row; } set { _patternModel.Row = value; } }
        public int Col { get { return _patternModel.Col; } set { _patternModel.Col = value; } }
        public int Zoom { get { return _patternModel.Zoom; } set { _patternModel.Zoom = value; } }

        public void InitializePattern()
        {
            _patternModel.DotPattern = new Dot[Row, Col];
            _patternCanvas.Children.Clear();
            for (int iRow = 0; iRow < Row; iRow++)
            {
                for (int iCol = 0; iCol < Col; iCol++)
                {
                    var rect = CreateRectangle();
                    rect.MouseLeftButtonUp += ClickEvent(iRow, iCol);
                    rect.MouseRightButtonUp += RightClickEvent(iRow, iCol);

                    Canvas.SetBottom(rect, iRow * SQUARE_SIZE);
                    Canvas.SetRight(rect, iCol * SQUARE_SIZE);

                    _patternModel.DotPattern[iRow, iCol] = new Dot(DEFAULT_FILL.ToString());
                    _patternCanvas.Children.Add(rect);
                }
            }
        }

        internal void Save(string filename)
        {
            var serializer = new PatternSerializer();
            serializer.Serialize(_patternModel, filename);
        }

        internal void Load(string filename)
        {
            var patternConverter = new PatternConverter();
            var brushConverter = new BrushConverter();
            var serializer = new PatternSerializer();

            _patternModel = serializer.Deserialize(filename);
            _patternCanvas.Children.Clear();

            for (int iRow = 0; iRow < _patternModel.Row; iRow++)
            {
                for (int iCol = 0; iCol < _patternModel.Col; iCol++)
                {
                    var rect = CreateRectangle();
                    try
                    {
                        rect.Fill = (Brush)brushConverter.ConvertFromString(_patternModel.DotPattern[iRow, iCol].Colour);
                    }
                    catch (NotSupportedException)
                    {
                        rect.Fill = DEFAULT_FILL;
                    }
                    rect.MouseLeftButtonUp += ClickEvent(iRow, iCol);
                    rect.MouseRightButtonUp += RightClickEvent(iRow, iCol);

                    Canvas.SetBottom(rect, iRow * SQUARE_SIZE);
                    Canvas.SetRight(rect, iCol * SQUARE_SIZE);

                    _patternCanvas.Children.Add(rect);
                }
            }
        }

        private MouseButtonEventHandler RightClickEvent(int row, int col)
        {
            return (sender, args) =>
            {
                var clicked = sender as Rectangle;
                if (clicked == null)
                    return;

                _rightClickEvent(sender, args);
                _patternModel.DotPattern[row, col].Colour = clicked.Fill.ToString();
            };
        }

        private MouseButtonEventHandler ClickEvent(int row, int col)
        {
            return (sender, args) =>
            {
                var clicked = sender as Rectangle;
                if (clicked == null)
                    return;

                _clickEvent(sender, args);
                _patternModel.DotPattern[row, col].Colour = clicked.Fill.ToString();
            };
        }
    }
}