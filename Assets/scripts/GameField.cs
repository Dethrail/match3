using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameField : MonoBehaviour
{
    public GridLayoutGroup GridLayoutGroup;
    public GemDistribution GemDistribution;
    public BoardControls BoardControls;
    public Button Regenerate;

    private GemColor[,] _colorField;

    private Random _rnd = new Random();

    public void Awake()
    {
        SetUpBoardSize();
        Regenerate.onClick.AddListener(OnRegenerateClick);
    }

    private void OnRegenerateClick()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        SetUpBoardSize();
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

        FillLogicBoard();
    }

    private void FillLogicBoard()
    {
        int totalCount = BoardControls.Width * BoardControls.Heigh;
        GemDistribution.InitializeDistribution(totalCount);

        _colorField = new GemColor[BoardControls.Width, BoardControls.Heigh];
        int k = 0;
        foreach (ColorDistribution colorDistribution in GemDistribution.GetDistributions())
        {
            for (int i = 0; i < colorDistribution.Count; i++)
            {
                _colorField[k / BoardControls.Heigh, k % BoardControls.Heigh] = colorDistribution.Color;
                k++;
            }
        }

        Shuffle();

        foreach (GemColor gemColor in _colorField)
        {
            Gem gem = GemFactory.CreateGem(gemColor);
            gem.transform.parent = transform;
            gem.transform.localScale = Vector3.one;
        }
    }

    // Bogosort
    private void Shuffle()
    {
        if (_rnd == null)
        {
            _rnd = new Random();
        }

        for (int x1 = 0; x1 <= _colorField.GetUpperBound(0); x1++)
        {
            for (int y1 = 0; y1 <= _colorField.GetUpperBound(1); y1++)
            {
                int x2 = _rnd.Next(0, _colorField.GetUpperBound(0) - 1);
                int y2 = _rnd.Next(0, _colorField.GetUpperBound(1) - 1);

                Swap(x1, y1, x2, y2);
            }
        }
    }

    private void Swap(int x1, int y1, int x2, int y2)
    {
        GemColor temp = (GemColor) _colorField.GetValue(x1, y1);
        _colorField.SetValue((GemColor) _colorField.GetValue(x2, y2), x1, y1);
        _colorField.SetValue(temp, x2, y2);
    }
}