using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gem : MonoBehaviour
{
    public Image Foreground;
    public Image Background;
    public Vector2Int Position;
    public GemColor Color;
    public Text Text;
}

public enum GemColor
{
    None = 0,
    Blue = 1,
    Yellow,
    Red,
    Purple,
    Green,
}