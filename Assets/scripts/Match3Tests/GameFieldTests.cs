using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Match3Tests
{
    public class GameFieldTests
    {
        [UnityTest]
        public IEnumerator TestingBoardGeneration()
        {
            var go = new GameObject("GameBoard");
            GameField field = go.AddComponent<GameField>();
            BoardControls controls = go.AddComponent<BoardControls>();
            GemDistribution distribution = go.AddComponent<GemDistribution>();
            GridLayoutGroup grid = go.AddComponent<GridLayoutGroup>();

            controls.Width = 10;
            controls.Heigh = 10;

            distribution.BlueCoef = 1.0f;
            distribution.YellowCoef = 1.0f;
            distribution.RedCoef = 2.0f;
            distribution.PurpleCoef = 0.5f;
            distribution.GreenCoef = 0.1f;

            field.BoardControls = controls;
            field.GemDistribution = distribution;
            field.GridLayoutGroup = grid;

            for (int i = 0; i < 10; i++)
            {
                field.FillLogicBoard();
                Debug.Log(field.HasValidMove());
                Assert.IsTrue(field.HasValidMove());
            }

            Assert.AreEqual("GameBoard", go.name);
            yield return null;
        }
    }
}