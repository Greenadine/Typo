using System.Windows.Media;

namespace Typo.Model
{
    public class GraphModel
    {
        public PointCollection Points { get; set; }

        public string ColorName { get; set; } = "White";

        public GraphModel(PointCollection points)
        {
            Points = points;
            ColorName = "White";
        }
    }
}
