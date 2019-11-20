using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public float GetTotalWeight()
    {
        return BlueCoef + YellowCoef + RedCoef + PurpleCoef + GreenCoef;
    }

    private bool _invalidate = true;

    public void InitializeDistribution(int totalCount)
    {
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

        _invalidate = false;
    }

    public int GetGemsCountByColor(GemColor color)
    {
        if (_invalidate)
        {
            Debug.LogError("Need to call SetUpDistribution before use");
            return 0;
        }

        return _distributions.First(x => x.Color == color).Count;
    }
}

public class ColorDistribution
{
    public GemColor Color;
    public float Coef;
    public int Count;
    public float FloatCount;

    public ColorDistribution(GemColor color, float coef, float floatCount)
    {
        Color = color;
        Coef = coef;
        FloatCount = floatCount;
        Count = Mathf.FloorToInt(FloatCount);
    }
}