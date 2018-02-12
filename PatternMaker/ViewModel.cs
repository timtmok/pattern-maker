using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PatternMaker
{
    internal class ViewModel
    {
        private readonly Canvas _patternCanvas;
        private PatternModel _patternModel;

        public BitmapImage BGImage { get; set; }

        private string bGFilename;
        private MouseButtonEventHandler _clickEvent;
        private MouseButtonEventHandler _rightClickEvent;

        public ViewModel(Canvas patternCanvas)
        {
            _patternCanvas = patternCanvas;
            _patternModel = new PatternModel();
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

        public void InitializePattern(MouseButtonEventHandler OnRectClick, MouseButtonEventHandler OnRectRightClick)
        {
            _patternModel.Pattern = new Rectangle[Row, Col];
            _patternCanvas.Children.Clear();
            _clickEvent = OnRectClick;
            _rightClickEvent = OnRectRightClick;
            for (int iRow = 0; iRow < Row; iRow++)
            {
                for (int iCol = 0; iCol < Col; iCol++)
                {
                    var rect = PatternModel.CreateRectangle();
                    rect.MouseLeftButtonUp += _clickEvent;
                    rect.MouseRightButtonUp += _rightClickEvent;

                    Canvas.SetBottom(rect, iRow * PatternModel.SQUARE_SIZE);
                    Canvas.SetRight(rect, iCol * PatternModel.SQUARE_SIZE);

                    _patternModel.Pattern[iRow, iCol] = rect;
                    _patternCanvas.Children.Add(rect);
                }
            }
        }

        internal void Save(string filename)
        {
            _patternModel.Save(filename);
        }

        internal void Load(string filename)
        {
            var patternConverter = new PatternConverter();
            using (var file = File.OpenText(filename))
            {
                var serializer = new JsonSerializer();
                var newModel = (PatternModel)serializer.Deserialize(file, typeof(PatternModel));
                _patternModel = newModel;
            }

            _patternCanvas.Children.Clear();
            for (int iRow = 0; iRow < _patternModel.Row; iRow++)
            {
                for (int iCol = 0; iCol < _patternModel.Col; iCol++)
                {
                    var rect = _patternModel.Pattern[iRow, iCol];
                    rect.MouseLeftButtonUp += _clickEvent;
                    rect.MouseRightButtonUp += _rightClickEvent;

                    Canvas.SetBottom(rect, iRow * PatternModel.SQUARE_SIZE);
                    Canvas.SetRight(rect, iCol * PatternModel.SQUARE_SIZE);

                    _patternCanvas.Children.Add(rect);
                }
            }
        }
    }
}