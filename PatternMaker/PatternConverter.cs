using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PatternMaker
{
    public class PatternConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PatternModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var patternModel = new PatternModel();
            var jObject = JObject.Load(reader);

            patternModel.Row = jObject.GetValue("Row").Value<int>();
            patternModel.Col = jObject.GetValue("Col").Value<int>();
            patternModel.Pattern = new Rectangle[patternModel.Row, patternModel.Col];
            patternModel.Zoom = jObject.GetValue("Zoom").Value<int>();

            var jArray = jObject.GetValue("Pattern").Value<JArray>();
            var brushConverter = new BrushConverter();

            foreach (JObject item in jArray.Children())
            {
                var rect = PatternModel.CreateRectangle();
                var row = item.GetValue("Row").Value<int>();
                var col = item.GetValue("Col").Value<int>();

                if (row < 0 || row >= patternModel.Row || col < 0 || col >= patternModel.Col)
                    continue;

                rect.Fill = (Brush) brushConverter.ConvertFromString(item.GetValue("Fill").Value<string>());
                patternModel.Pattern[row, col] = rect;
            }

            return patternModel;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var patternModel = value as PatternModel;
            if (patternModel == null)
                return;

            var brushConverter = new BrushConverter();
            var pattern = new JObject();
            pattern["Row"] = patternModel.Row;
            pattern["Col"] = patternModel.Col;
            pattern["Zoom"] = patternModel.Zoom;

            var rects = new JArray();
            for (int iRow = 0; iRow < patternModel.Row; iRow++)
            {
                for (int iCol = 0; iCol < patternModel.Col; iCol++)
                {
                    var rect = new JObject();
                    rect["Row"] = iRow;
                    rect["Col"] = iCol;
                    rect["Fill"] = brushConverter.ConvertToString(patternModel.Pattern[iRow, iCol].Fill);
                    rects.Add(rect);
                }
            }

            pattern["Pattern"] = rects;
            serializer.Serialize(writer, pattern);
        }
    }
}