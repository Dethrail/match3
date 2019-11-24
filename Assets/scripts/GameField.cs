﻿using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    public GridLayoutGroup GridLayoutGroup;
    public GemDistribution GemDistribution;
    public BoardControls BoardControls;
    public Button Regenerate;

    private GemColor[,] _colorField;

    public void Awake()
    {
        SetUpBoardSize();
        Regenerate.onClick.AddListener(OnRegenerateClick);
    }

    private void OnRegenerateClick()
    {
        SetUpBoardSize();
    }


    public void SetUpBoardSize()
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
        FillBoardWithGems();
    }


    public void FillLogicBoard()
    {
        int totalCount = BoardControls.Width * BoardControls.Heigh;
        GemDistribution.InitializeDistribution(totalCount);

        _colorField = new GemColor[BoardControls.Width, BoardControls.Heigh];
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                // todo: check sequence instead of left/bottom
                GemColor left = (x > 0) ? _colorField[x - 1, y] : GemColor.None;
                GemColor bottom = (y > 0) ? _colorField[x, y - 1] : GemColor.None;

                _colorField[x, y] = GemDistribution.GetNextColorWithExcludes(left, bottom);
//                Debug.Log(x + ":" + y + " " + left + " " + bottom + " = " + _colorField[x, y]);
            }
        }
    }

    public void FillBoardWithGems()
    {
//        Debug.Log("0:0 = " + _colorField[0, 0] + "   "
//                  + _colorField.GetUpperBound(0) + ":" + _colorField.GetUpperBound(1)
//                  + _colorField[_colorField.GetUpperBound(0), _colorField.GetUpperBound(1)]);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Start from left(x) to right and from down(y) to up
        for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
        {
            for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
            {
                if (_colorField[x, y] == GemColor.None)
                {
                    continue;
                }

                Gem gem = GemFactory.CreateGem(_colorField[x, y]);
                //gem.Text.text = x + ":" + y;
                gem.transform.SetParent(transform, false);
            }
        }
    }

    public bool HasValidMove()
    {
        return true;
    }
}