using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    public GridLayoutGroup GridLayoutGroup;
    public GemDistribution GemDistribution;
    public BoardControls BoardControls;
    public Button Regenerate;

    private GemColor[,] _colorField;
    private Gem[,] _gemsField;

    private List<Sequence> _sequences;

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

        GenerateBoard();

        FillBoardWithGems();
        MarkBestMove();
    }

    private bool HasAutoMatch()
    {
        List<Sequence> sequences = new List<Sequence>();
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                Sequence seq = new Sequence(new Vector2Int(x, y));
                RunWave(seq, x, y, true, true, _colorField[x, y]);
                sequences.Add(seq);
            }
        }

        return sequences.Count(x => x.LongestSequence > 2) != 0;
    }

    public void GenerateBoard()
    {
        int i = 0;
        do
        {
            i++;
            if (i > 100)
            {
                Debug.LogError("cant create board using this weight and board sizes");
                return;
            }

            FillLogicBoard();
        } while (HasAutoMatch() || !HasValidMove());
    }

    private void FillLogicBoard()
    {
        int totalCount = BoardControls.Width * BoardControls.Heigh;
        GemDistribution.InitializeDistribution(totalCount);
        _sequences = new List<Sequence>();

        _colorField = new GemColor[BoardControls.Width, BoardControls.Heigh];
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                GemColor left = GemColor.None;
                GemColor bottom = GemColor.None;
                if (x > 0)
                {
                    Sequence seq = RunWave(new Sequence(), x - 1, y, true, true, _colorField[x - 1, y]);
                    if (seq.LongestSequence > 1)
                    {
                        left = _colorField[x - 1, y];
                    }
                }

                if (y > 0)
                {
                    Sequence seq = RunWave(new Sequence(), x, y - 1, true, true, _colorField[x, y - 1]);
                    if (seq.LongestSequence > 1)
                    {
                        bottom = _colorField[x, y - 1];
                    }
                }

                _colorField[x, y] = GemDistribution.GetNextColorWithExcludes(left, bottom);
            }
        }

        // calculate sequences
        HasValidMove();

        _sequences = _sequences.OrderByDescending(seq => seq.LongestSequence).ToList();
    }

    private void MarkBestMove(int i, Color color)
    {
        _gemsField[_sequences[i].InitialPosition.x, _sequences[i].InitialPosition.y].Background.color = color;
        if (_sequences[i].Horizontal.Count == _sequences[i].LongestSequence)
        {
            foreach (Vector2Int vector2Int in _sequences[i].Horizontal)
            {
                _gemsField[vector2Int.x, vector2Int.y].Background.color = color;
            }
        }
        else if (_sequences[i].Vertical.Count == _sequences[i].LongestSequence)
        {
            foreach (Vector2Int vector2Int in _sequences[i].Vertical)
            {
                _gemsField[vector2Int.x, vector2Int.y].Background.color = color;
            }
        }
    }

    public void MarkBestMove()
    {
        if (_sequences.Count > 1)
        {
            MarkBestMove(1, Color.blue);
        }

        MarkBestMove(0, Color.green);
    }

    public void FillBoardWithGems()
    {
        _gemsField = new Gem[BoardControls.Width, BoardControls.Heigh];
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
                gem.Position = new Vector2Int(x, y);
                gem.transform.SetParent(transform, false);
                _gemsField[x, y] = gem;
            }
        }
    }

    private Sequence RunWave(Sequence sequence, int x1, int y1, bool isHorizontal, bool isVertical, GemColor color)
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

                if (isVertical && x2 == 0 && _colorField[x1 + x2, y1 + y2] == color &&
                    !sequence.Vertical.Contains(new Vector2Int(x1 + x2, y1 + y2)))
                {
                    sequence.Vertical.Add(new Vector2Int(x1 + x2, y1 + y2));
                    sequence = RunWave(sequence, x1 + x2, y1 + y2, false, true, color);
                }

                if (isHorizontal && y2 == 0 && _colorField[x1 + x2, y1 + y2] == color &&
                    !sequence.Horizontal.Contains(new Vector2Int(x1 + x2, y1 + y2)))
                {
                    sequence.Horizontal.Add(new Vector2Int(x1 + x2, y1 + y2));
                    sequence = RunWave(sequence, x1 + x2, y1 + y2, true, false, color);
                }
            }
        }

        return sequence;
    }

    private bool CheckForMatch(int x0, int y0, int x1, int y1, GemColor color)
    {
        bool hasMatch = false;
        Sequence sequence = new Sequence(new Vector2Int(x0, y0), new Vector2Int(x1, y1));
        sequence = RunWave(sequence, x1, y1, true, true, color);
        if (sequence.LongestSequence > 1)
        {
            hasMatch = true;
            _sequences.Add(sequence);
        }

        return hasMatch;
    }

    private bool HasMatch(int x, int y)
    {
        bool hasMatch = false;
        GemColor currentColor = _colorField[x, y];
        _colorField[x, y] = GemColor.None;
        bool canMoveLeft = x > 0;
        bool canMoveRight = x < _colorField.GetUpperBound(0);
        bool canMoveUp = y < _colorField.GetUpperBound(1);
        bool canMoveDown = y > 0;


        if (canMoveLeft && currentColor != _colorField[x - 1, y])
        {
            hasMatch = CheckForMatch(x, y, x - 1, y, currentColor);
        }

        if (canMoveRight && currentColor != _colorField[x + 1, y])
        {
            hasMatch = CheckForMatch(x, y, x + 1, y, currentColor);
        }

        if (canMoveUp && currentColor != _colorField[x, y + 1])
        {
            hasMatch = CheckForMatch(x, y, x, y + 1, currentColor);
        }

        if (canMoveDown && currentColor != _colorField[x, y - 1])
        {
            hasMatch = CheckForMatch(x, y, x, y - 1, currentColor);
        }

        _colorField[x, y] = currentColor;
        return hasMatch;
    }

    public bool HasValidMove()
    {
        bool hasMove = false;
        for (int x = 0; x <= _colorField.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= _colorField.GetUpperBound(1); y++)
            {
                if (HasMatch(x, y))
                {
                    hasMove = true;
                }
            }
        }

        return hasMove;
    }
}