using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemFactory : MonoBehaviour
{
    public List<Gem> GemPrefabs;
    public static GemFactory _instance;

    public void Awake()
    {
        _instance = this;
    }

    public static Gem CreateGem(GemColor color)
    {
        Gem gem = Instantiate(_instance.GemPrefabs.First(x => x.Color == color));
        return gem;
    }
}