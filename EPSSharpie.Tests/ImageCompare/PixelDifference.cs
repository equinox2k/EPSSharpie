using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace EPSSharpie.Tests.ImageCompare
{
    public readonly struct PixelDifference
    {
        public PixelDifference(Point position, int redDifference, int greenDifference, int blueDifference, int alphaDifference)
        {
            Position = position;
            RedDifference = redDifference;
            GreenDifference = greenDifference;
            BlueDifference = blueDifference;
            AlphaDifference = alphaDifference;
        }

        public PixelDifference(Point position, Rgba64 expected, Rgba64 actual) : this(position, actual.R - expected.R, actual.G - expected.G, actual.B - expected.B, actual.A - expected.A)
        {
        }

        public Point Position { get; }

        public int RedDifference { get; }
        public int GreenDifference { get; }
        public int BlueDifference { get; }
        public int AlphaDifference { get; }

        public override string ToString() => $"[Δ({this.RedDifference},{this.GreenDifference},{this.BlueDifference},{this.AlphaDifference}) @ ({this.Position.X},{this.Position.Y})]";
    }
}