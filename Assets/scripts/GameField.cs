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
    //private List<Vector2Int> _watchList;

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

        FindAllSequences();
    }

    private bool IsPartOfSequence(Vector2Int point)
    {
        foreach (Sequence sequence in _sequences)
        {
            if (sequence.Gems.Contains(point))
            {
                return true;
            }
        }

        return false;
    }

    private void FindAllSequences()
    {
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                if (IsPartOfSequence(new Vector2Int(x, y)))
                {
                    continue;
                }

                Sequence sequence = new Sequence();
                sequence.Gems.Add(new Vector2Int(x, y));
                RunWave(sequence, x, y, _colorField[x, y]);
                _sequences.Add(sequence);
            }
        }

        _sequences = _sequences.OrderByDescending(x => x.Gems.Count).ToList();
        for (int i = 0; i < _sequences.Count; i++)
        {
            Debug.Log(_sequences[i].Gems.Count);
        }
        //RunWave(new Sequence(), 0, 0, _colorField[0, 0]);
    }

    private Sequence RunWave(Sequence sequence, int x1, int y1, GemColor color)
    {
        for (int x2 = -1; x2 <= 1; x2++)
        {
            for (int y2 = -1; y2 <= 1; y2++)
            {
                // disable diagonal match
                if (Mathf.Abs(x2) == Mathf.Abs(y2))
                {
                    continue;
                }

                if (x1 + x2 < 0 || x1 + x2 > _colorField.GetUpperBound(0))
                {
                    continue;
                }

                if (y1 + y2 < 0 || y1 + y2 > _colorField.GetUpperBound(1))
                {
                    continue;
                }

                if (_colorField[x1 + x2, y1 + y2] == color && !sequence.Gems.Contains(new Vector2Int(x1 + x2, y1 + y2)))
                {
                    sequence.Gems.Add(new Vector2Int(x1 + x2, y1 + y2));
                    //Debug.Log((x1 + x2) + " " + (y1 + y2) + " = " + color);
                    sequence = RunWave(sequence, x1 + x2, y1 + y2, color);
                }
            }
        }

        return sequence;
    }

    private void FillLogicBoard()
    {
        int totalCount = BoardControls.Width * BoardControls.Heigh;
        _sequences = new List<Sequence>();
        //_watchList = new List<Vector2Int>(totalCount);
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
    }

    private void FillBoardWithGems()
    {
        foreach (GemColor gemColor in _colorField)
        {
            Gem gem = GemFactory.CreateGem(gemColor);
            gem.transform.SetParent(transform, false);
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