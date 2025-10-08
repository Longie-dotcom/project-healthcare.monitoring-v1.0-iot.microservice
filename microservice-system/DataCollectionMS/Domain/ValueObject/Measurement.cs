using Newtonsoft.Json.Linq;

namespace Domain.ValueObject
{
    public class Measurement
    {
        public string Type { get; private set; }
        public JToken Value { get; private set; }
        public string Unit { get; private set; } = "";

        protected Measurement() { }

        public Measurement(string type, JToken value, string unit)
        {
            Type = type;
            Value = value;
            Unit = unit;
        }
    }
}
