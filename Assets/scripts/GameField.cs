using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    public GridLayoutGroup GridLayoutGroup;
    public GemDistribution GemDistribution;
    public BoardControls BoardControls;
    public Button Regenerate;

    public void Awake()
    {
        SetUpBoardSize();
        GenerateMockElementsByDisstribution();
        Regenerate.onClick.AddListener(OnRegenerateClick);
    }

    private void OnRegenerateClick()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    private void SetUpBoardSize()
    {
        if (BoardControls.Width <= BoardControls.Heigh)
        {
            GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GridLayoutGroup.constraintCount = BoardControls.Width;
        }
        else
        {
            GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            GridLayoutGroup.constraintCount = BoardControls.Heigh;
        }
    }

    private void GenerateMockElements()
    {
        for (int i = 0; i < BoardControls.Width * BoardControls.Heigh; i++)
        {
            Gem gem = GemFactory.CreateGem(GemColor.Blue);
            gem.transform.parent = transform;
        }
    }

    private void GenerateMockElementsByDisstribution()
    {
        int totalCount = BoardControls.Width * BoardControls.Heigh;
        GemDistribution.SetUpDistribution(totalCount);
        GenerateMockByColor(GemColor.Blue);
        GenerateMockByColor(GemColor.Red);
        GenerateMockByColor(GemColor.Yellow);
        GenerateMockByColor(GemColor.Purple);
        GenerateMockByColor(GemColor.Green);
    }

    private void GenerateMockByColor(GemColor gemColor)
    {
        for (int i = 0; i < GemDistribution.GetGemsCountByColor(gemColor); i++)
        {
            Gem gem = GemFactory.CreateGem(gemColor);
            gem.transform.parent = transform;
        }
    }
}