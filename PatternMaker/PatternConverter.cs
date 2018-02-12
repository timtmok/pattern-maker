using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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
            patternModel.Zoom = jObject.GetValue("Zoom").Value<int>();
            patternModel.DotPattern = new Dot[patternModel.Row, patternModel.Col];

            var jArray = jObject.GetValue("Pattern").Value<JArray>();

            foreach (JObject item in jArray.Children())
            {
                var row = item.GetValue("Row").Value<int>();
                var col = item.GetValue("Col").Value<int>();
                var colour = item.GetValue("Fill").Value<string>();
                var dot = new Dot(colour);

                if (row < 0 || row >= patternModel.Row || col < 0 || col >= patternModel.Col)
                    continue;

                patternModel.DotPattern[row, col] = dot;
            }

            return patternModel;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var patternModel = value as PatternModel;
            if (patternModel == null)
                return;

            var pattern = new JObject
            {
                ["Row"] = patternModel.Row,
                ["Col"] = patternModel.Col,
                ["Zoom"] = patternModel.Zoom
            };

            var rects = new JArray();
            for (int iRow = 0; iRow < patternModel.Row; iRow++)
            {
                for (int iCol = 0; iCol < patternModel.Col; iCol++)
                {
                    var rect = new JObject();
                    rect["Row"] = iRow;
                    rect["Col"] = iCol;
                    rect["Fill"] = patternModel.DotPattern[iRow, iCol].Colour;
                    rects.Add(rect);
                }
            }

            pattern["Pattern"] = rects;
            serializer.Serialize(writer, pattern);
        }
    }
}