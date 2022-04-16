using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum State
    {
        Die,
        Moving,
        Idle,
        Attack,
        Skill,
        GetHit,
        Root,
    }
    public enum WorldObject
    {
        Unknown,
        Dog,
        Cube,
        Block,
    }

    public enum TileType
    {
        Empty,
        Wall,
    }
    public enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

}