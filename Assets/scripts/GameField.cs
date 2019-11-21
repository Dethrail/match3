using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<Sequence> _sequences;

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
        FillBoardWithGems();
    }


    private void FillLogicBoard()
    {
        int totalCount = BoardControls.Width * BoardControls.Heigh;
        _sequences = new List<Sequence>();
        GemDistribution.InitializeDistribution(totalCount);

        _colorField = new GemColor[BoardControls.Width, BoardControls.Heigh];
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                GemColor left = (x > 0) ? _colorField[x - 1, y] : GemColor.None;
                GemColor bottom = (y > 0) ? _colorField[x, y - 1] : GemColor.None;
                _colorField[x, y] = GemDistribution.GetNextColorWithExcludes(left, bottom);
            }
        }

        PreventMatchingAfterGeneration();
    }

    private void PreventMatchingAfterGeneration()
    {
        int x = _colorField.GetUpperBound(0);
        int y = _colorField.GetUpperBound(1);

        GemColor current;
        GemColor comparable;
        do
        {
            current = _colorField[x, y];
            comparable = _colorField[--x, y];

//            if (x < 0)
//            {
//                x = _colorField.GetUpperBound(0);
//                y--;
//            }

            int newX;
            int newY;
            do
            {
                newX = _rnd.Next(1, _colorField.GetUpperBound(0));
                newY = _rnd.Next(1, _colorField.GetUpperBound(1));
            } while (_colorField[newX, newY] == current
                     || _colorField[newX - 1, newY] == current
                     || _colorField[newX, newY - 1] == current);

            //            int newX = 0;
//            int newY = 0;
//            while (_colorField[newX, newY] == current)
//            {
//                break;
//            }
            Debug.Log("swap " + x + ":" + y + " <=> " + newX + ":" + newY);
            Swap(x, y, newX, newY);
            return;
        } while (current == comparable);
    }

    private void FillBoardWithGems()
    {
        Debug.Log("0:0 = " + _colorField[0, 0] + "   "
                  + _colorField.GetUpperBound(0) + ":" + _colorField.GetUpperBound(1)
                  + _colorField[_colorField.GetUpperBound(0), _colorField.GetUpperBound(1)]);
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                if (_colorField[x, y] == GemColor.None)
                {
                    continue;
                }

                Gem gem = GemFactory.CreateGem(_colorField[x, y]);
                gem.transform.SetParent(transform, false);
            }
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

public class Sequence
{
    public List<Vector2Int> Gems = new List<Vector2Int>();
}