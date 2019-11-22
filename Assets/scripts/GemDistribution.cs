using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using Random = System.Random;

public class GemDistribution : MonoBehaviour
{
    public float BlueCoef = 1.0f;
    public float YellowCoef = 1.0f;
    public float RedCoef = 2.0f;
    public float PurpleCoef = 0.5f;
    public float GreenCoef = 0.1f;

    public ColorDistribution Blue;
    public ColorDistribution Yellow;
    public ColorDistribution Red;
    public ColorDistribution Purple;
    public ColorDistribution Green;

    private List<ColorDistribution> _distributions = new List<ColorDistribution>();
    private Random _rnd = new Random(9973);

    public float GetTotalWeight()
    {
        return BlueCoef + YellowCoef + RedCoef + PurpleCoef + GreenCoef;
    }

    public void InitializeDistribution(int totalCount)
    {
        _distributions.Clear();
        float distributionCoef = totalCount / GetTotalWeight();

        // calculate hard count
        Blue = new ColorDistribution(GemColor.Blue, BlueCoef, distributionCoef * BlueCoef);
        Yellow = new ColorDistribution(GemColor.Yellow, YellowCoef, distributionCoef * YellowCoef);
        Red = new ColorDistribution(GemColor.Red, RedCoef, distributionCoef * RedCoef);
        Purple = new ColorDistribution(GemColor.Purple, PurpleCoef, distributionCoef * PurpleCoef);
        Green = new ColorDistribution(GemColor.Green, GreenCoef, distributionCoef * GreenCoef);

        _distributions.Add(Blue);
        _distributions.Add(Yellow);
        _distributions.Add(Red);
        _distributions.Add(Purple);
        _distributions.Add(Green);

        _distributions = _distributions.OrderBy(x => x.Coef).ToList();

        FixRoundIssue(totalCount);
    }

    private void FixRoundIssue(int totalCount)
    {
        // fix round issue of distribution coefs
        float mantissa = 0;
        foreach (ColorDistribution colorDistribution in _distributions)
        {
            float currentMantissa = colorDistribution.FloatCount - colorDistribution.Count;

            mantissa += currentMantissa;
            colorDistribution.FloatCount -= currentMantissa;

            if (Mathf.RoundToInt(mantissa) > 0)
            {
                colorDistribution.FloatCount += mantissa;
                colorDistribution.Count = Mathf.RoundToInt(colorDistribution.FloatCount);
                mantissa = colorDistribution.FloatCount - colorDistribution.Count;
            }
        }

        if (totalCount != _distributions.Sum(x => x.Count))
        {
            Debug.LogError("Problem with distribution rounds and mantisa");
        }
    }

    public GemColor GetNextColorWithExcludes(GemColor left, GemColor bottom)
    {
        var collection = _distributions.Where(x => x.Count > 0 && x.Color != left && x.Color != bottom);
        collection = collection.Shuffle(_rnd);
        collection = collection.OrderByDescending(x => x.Count);

        foreach (ColorDistribution colorDistribution in collection)
        {
            return colorDistribution.Pop();
        }

        // can't found next item
        foreach (ColorDistribution colorDistribution in _distributions.Where(x => x.Count > 0))
        {
            return colorDistribution.Pop();
        }


        return GemColor.None;
    }
}