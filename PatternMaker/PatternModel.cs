using Newtonsoft.Json;

namespace PatternMaker
{
    [JsonConverter(typeof(PatternConverter))]
    public class PatternModel
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Dot[,] DotPattern { get; set; }
        public int Zoom { get; set; }
    }
}
