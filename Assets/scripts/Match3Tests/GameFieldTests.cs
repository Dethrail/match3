using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Match3Tests
{
    public class GameFieldTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }

        [UnityTest]
        [TestCase(1000, ExpectedResult = null)]
        public IEnumerator TestingBoardGeneration(int iterations)
        {
            var go = new GameObject("GameBoard");
            GameField field = go.AddComponent<GameField>();
            BoardControls controls = go.AddComponent<BoardControls>();
            GemDistribution distribution = go.AddComponent<GemDistribution>();
            bool hasOnlyValidBoards = true;

            controls.Width = 10;
            controls.Heigh = 10;

            distribution.BlueCoef = 1.0f;
            distribution.YellowCoef = 1.0f;
            distribution.RedCoef = 2.0f;
            distribution.PurpleCoef = 0.5f;
            distribution.GreenCoef = 0.1f;

            field.BoardControls = controls;
            field.GemDistribution = distribution;

            for (int i = 0; i < iterations; i++)
            {
                field.GenerateBoard();
                if (!field.HasValidMove())
                {
                    hasOnlyValidBoards = false;
                }
            }

            Assert.IsTrue(hasOnlyValidBoards);
            yield return null;
        }
    }
}