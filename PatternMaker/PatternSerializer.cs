using Newtonsoft.Json;
using System.IO;

namespace PatternMaker
{
    public class PatternSerializer
    {
        public void Serialize(PatternModel model, string filename)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new PatternConverter());
            var serializer = JsonSerializer.Create(settings);

            using (var stream = new StreamWriter(filename))
            using (var writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, model);
            }
        }

        public PatternModel Deserialize(string filename)
        {
            using (var file = File.OpenText(filename))
            {
                PatternModel model;
                try
                {
                    var serializer = new JsonSerializer();
                    model = (PatternModel)serializer.Deserialize(file, typeof(PatternModel));
                } catch
                {
                    model = null;
                }
                return model;
            }
        }
    }
}
