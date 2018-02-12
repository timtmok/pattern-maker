using Newtonsoft.Json;
using System.IO;

namespace PatternMaker
{
    [JsonConverter(typeof(PatternConverter))]
    public class PatternModel
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Dot[,] DotPattern { get; set; }
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
    }
}
