using System.Drawing;

namespace Cient
{
    public class Product
    {
        public Color Color { get; set; }
        public string Type { get; set; }
        public string DesignType { get; set; }

        public override string ToString()
        {
            return $"{Type},{Color},{DesignType}";
        }
    }
}
