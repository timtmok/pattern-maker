using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Input;
using System;

namespace PatternMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SQUARE_SIZE = 15;
        private const double SQUARE_OPACITY = 0.6;
        private const double SQUARE_THICKNESS = 0.2;
        private SolidColorBrush DEFAULT_FILL = Brushes.White;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
            UpdateGrid();
        }

        private void SetImage()
        {
            var zoomLevel = Convert.ToDouble(Zoom.Value) / 100;
            var sourceImage = GetModel()?.BGImage;

            if (sourceImage == null)
                return;

            int stride = sourceImage.PixelWidth * 4;
            byte[] pixelData = new byte[stride * sourceImage.PixelHeight];
            sourceImage.CopyPixels(pixelData, stride, 0);
            var resizedImage = BitmapSource.Create(sourceImage.PixelWidth, sourceImage.PixelHeight, sourceImage.DpiX / zoomLevel, sourceImage.DpiY / zoomLevel, sourceImage.Format, null, pixelData, stride);
            BGImage.Source = resizedImage;
        }

        private void UpdateGrid()
        {
            ViewModel model = GetModel();
            if (model == null)
                return;
            
            PatternCanvas.Children.Clear();

            for (int iRow = 0; iRow < model.Row; iRow++)
            {
                for (int iCol = 0; iCol < model.Col; iCol++)
                {
                    var rect = new Rectangle()
                    {
                        Stroke = Brushes.Black,
                        Fill = DEFAULT_FILL,
                        StrokeThickness = SQUARE_THICKNESS,
                        Width = SQUARE_SIZE,
                        Height = SQUARE_SIZE,
                        Opacity = SQUARE_OPACITY
                    };
                    rect.MouseLeftButtonUp += OnRectClick;
                    rect.MouseRightButtonUp += OnRectRightClick;
                    Canvas.SetBottom(rect, iRow * SQUARE_SIZE);
                    Canvas.SetRight(rect, iCol * SQUARE_SIZE);
                    PatternCanvas.Children.Add(rect);
                }
            }

            ScrollView.ScrollToEnd();
            ScrollView.ScrollToRightEnd();
        }

        private ViewModel GetModel()
        {
            return DataContext as ViewModel;
        }

        private void OnRectClick(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null)
                return;
            Color selectedColour = ColourPicker.SelectedColor.GetValueOrDefault(Colors.Black);
            rect.Fill = new SolidColorBrush(selectedColour);
        }

        private void OnRectRightClick(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null)
                return;
            rect.Fill = DEFAULT_FILL;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var browseDialog = new OpenFileDialog();
            var result = browseDialog.ShowDialog();
            if (result == true)
            {
                var model = GetModel();
                model.SetBGFilename(browseDialog.FileName);
                SetImage();
            }
        }

        private void Col_Updated(object sender, TextChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void Row_Updated(object sender, TextChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void Zoom_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetImage();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".bmp";
            var result = dialog.ShowDialog();
            if (result == false)
                return;
            var filename = dialog.FileName;
            ExportImage(filename);
        }

        private void ExportImage(string filename)
        {
            var actualHeight = PatternCanvas.RenderSize.Height;
            var actualWidth = PatternCanvas.RenderSize.Width;

            var renderToBitmap = new RenderTargetBitmap((int) actualWidth, (int)actualHeight, 96, 96, PixelFormats.Default);
            renderToBitmap.Render(PatternCanvas);
            var bitmapFrame = BitmapFrame.Create(renderToBitmap);
            BitmapEncoder pngEncoder = new BmpBitmapEncoder();
            pngEncoder.Frames.Add(bitmapFrame);
            using (var outFile = System.IO.File.OpenWrite(filename))
            {
                pngEncoder.Save(outFile);
            }
        }
    }
}
