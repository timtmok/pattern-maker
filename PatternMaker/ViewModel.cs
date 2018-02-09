using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PatternMaker
{
    internal class ViewModel
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public BitmapImage BGImage { get; set; }

        private List<Rectangle>[][] _pattern;
        private string bGFilename;

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

        public ViewModel()
        {
            Row = 120;
            Col = 192;
        }
    }
}