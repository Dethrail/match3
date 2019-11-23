using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class GemFactory : MonoBehaviour
    {
        public List<Gem> GemPrefabs;
        private static GemFactory _instance;

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
}