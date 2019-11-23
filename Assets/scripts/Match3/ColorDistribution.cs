using UnityEngine;

namespace Match3
{
    public class ColorDistribution
    {
        public readonly GemColor Color;
        public readonly float Coef;
        public int Count;
        public float FloatCount;

        public ColorDistribution(GemColor color, float coef, float floatCount)
        {
            Color = color;
            Coef = coef;
            FloatCount = floatCount;
            Count = Mathf.FloorToInt(FloatCount);
        }

        public GemColor Pop()
        {
            Count--;
            return Color;
        }
    }
}