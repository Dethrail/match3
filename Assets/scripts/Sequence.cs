using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence
{
    public Vector2Int InitialPosition { get; }
    public Vector2Int Move { get; }
    public List<Vector2Int> Vertical = new List<Vector2Int>();
    public List<Vector2Int> Horizontal = new List<Vector2Int>();
    public int LongestSequence;

    public Sequence()
    {
    }

    public Sequence(Vector2Int position, Vector2Int move)
    {
        InitialPosition = position;
        Move = move;
    }
}