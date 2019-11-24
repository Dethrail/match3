﻿using System.Collections.Generic;
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

        FillLogicBoard();
        FillBoardWithGems();
    }


    public void FillLogicBoard()
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
                    Sequence seq = RunWave(new Sequence(), x, y, _colorField[x - 1, y]);
                    if (seq.Horizontal.Count > 1 || seq.Vertical.Count > 1)
                    {
                        left = _colorField[x - 1, y];
                    }
                }

                if (y > 0)
                {
                    Sequence seq = RunWave(new Sequence(), x, y, _colorField[x, y - 1]);
                    if (seq.Horizontal.Count > 1 || seq.Vertical.Count > 1)
                    {
                        bottom = _colorField[x, y - 1];
                    }
                }

                _colorField[x, y] = GemDistribution.GetNextColorWithExcludes(left, bottom);
            }
        }

        if (HasValidMove())
        {
            Debug.Log("has move");
        }

        _sequences = _sequences.OrderByDescending(seq => seq.LongestSequence).ToList();
        Debug.Log(_sequences[0].InitialPosition + " " + _sequences[0].Move + " = " +_sequences[0].LongestSequence);
        Debug.Log(_sequences.Count);
    }

    public void FillBoardWithGems()
    {
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
                gem.transform.SetParent(transform, false);
            }
        }
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


                if (y2 == 0 && _colorField[x1 + x2, y1 + y2] == color &&
                    !sequence.Horizontal.Contains(new Vector2Int(x1 + x2, y1 + y2)))
                {
                    sequence.Horizontal.Add(new Vector2Int(x1 + x2, y1 + y2));
                    sequence = RunWave(sequence, x1 + x2, y1 + y2, color);
                }


                if (x2 == 0 && _colorField[x1 + x2, y1 + y2] == color &&
                    !sequence.Vertical.Contains(new Vector2Int(x1 + x2, y1 + y2)))
                {
                    sequence.Vertical.Add(new Vector2Int(x1 + x2, y1 + y2));
                    sequence = RunWave(sequence, x1 + x2, y1 + y2, color);
                }
            }
        }

        return sequence;
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


        if (canMoveLeft)
        {
            Sequence seq = RunWave(new Sequence(new Vector2Int(x, y), new Vector2Int(x - 1, y)), x - 1, y,
                currentColor);
            if (seq.Horizontal.Count > 1 || seq.Vertical.Count > 1)
            {
                hasMatch = true;
                seq.LongestSequence = Mathf.Max(seq.Horizontal.Count, seq.Vertical.Count);
                _sequences.Add(seq);
            }
        }

        if (canMoveRight)
        {
            Sequence seq = RunWave(new Sequence(new Vector2Int(x, y), new Vector2Int(x + 1, y)), x + 1, y,
                currentColor);
            if (seq.Horizontal.Count > 1 || seq.Vertical.Count > 1)
            {
                hasMatch = true;
                seq.LongestSequence = Mathf.Max(seq.Horizontal.Count, seq.Vertical.Count);
                _sequences.Add(seq);
            }
        }

        if (canMoveUp)
        {
            Sequence seq = RunWave(new Sequence(new Vector2Int(x, y), new Vector2Int(x, y + 1)), x, y + 1,
                currentColor);
            if (seq.Horizontal.Count > 1 || seq.Vertical.Count > 1)
            {
                hasMatch = true;
                seq.LongestSequence = Mathf.Max(seq.Horizontal.Count, seq.Vertical.Count);
                _sequences.Add(seq);
            }
        }

        if (canMoveDown)
        {
            Sequence seq = RunWave(new Sequence(new Vector2Int(x, y), new Vector2Int(x, y - 1)), x, y - 1,
                currentColor);
            if (seq.Horizontal.Count > 1 || seq.Vertical.Count > 1)
            {
                hasMatch = true;
                seq.LongestSequence = Mathf.Max(seq.Horizontal.Count, seq.Vertical.Count);
                _sequences.Add(seq);
            }
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