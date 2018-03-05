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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel(PatternCanvas, OnRectClick, OnRectRightClick);
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
            var model = GetModel();
            if (model == null)
                return;

            model.InitializePattern();

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

            var model = GetModel();
            if (model == null)
                return;

            Color selectedColour = ColourPicker.SelectedColor.GetValueOrDefault(Colors.Black);
            rect.Fill = new SolidColorBrush(selectedColour);
        }

        private void OnRectRightClick(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null)
                return;
            rect.Fill = ViewModel.DEFAULT_FILL;
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

        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void Zoom_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetImage();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
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
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(bitmapFrame);
            using (var outFile = System.IO.File.OpenWrite(filename))
            {
                pngEncoder.Save(outFile);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".ptn";
            var result = dialog.ShowDialog();
            if (result == false)
                return;

            var filename = dialog.FileName;
            var model = GetModel();
            if (model == null)
                return;
            model.Save(filename);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Pattern | *.ptn";
            var result = dialog.ShowDialog();
            if (result == false)
                return;

            var filename = dialog.FileName;
            var model = GetModel();
            if (model == null)
                return;
            model.Load(filename);

            RowInput.Text = model.Row.ToString();
            ColInput.Text = model.Col.ToString();
            Zoom.Value = model.Zoom;
        }
    }
}
