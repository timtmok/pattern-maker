using Newtonsoft.Json;
using System.IO;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PatternMaker
{
    [JsonConverter(typeof(PatternConverter))]
    public class PatternModel
    {
        public static readonly int SQUARE_SIZE = 15;
        public static readonly double SQUARE_OPACITY = 0.6;
        public static readonly double SQUARE_THICKNESS = 0.2;
        public static readonly SolidColorBrush DEFAULT_FILL = Brushes.White;

        public int Row { get; set; }
        public int Col { get; set; }
        public Rectangle[,] Pattern { get; set; }
        public int Zoom { get; set; }

        public void Save(string filename)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new PatternConverter());
            var serializer = JsonSerializer.Create(settings);

            using (var stream = new StreamWriter(filename))
            using (var writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, this);
            }
        }
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
    }
}
