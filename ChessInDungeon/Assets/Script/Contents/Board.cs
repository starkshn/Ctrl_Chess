using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    static Board s_instance = new Board();
    public static Board Instance { get { return s_instance; } }

	public Define.TileType[,] Tile { get; private set; }
	
	public int Size { get; private set; }

	public int DestZ { get; private set; }
	public int DestX { get; private set; }

	public void BoardInitialize(int size, int destZ, int destX, UnitController unit)
	{
        Tile = new Define.TileType[size, size];

        Size = 0;
		DestZ = 0;
		DestX = 0;

		Size = size;
		DestZ = destZ;
		DestX = destX;

		//Tile[DestZ, DestX] = Define.TileType.Wall;
		//Tile[unit.PosZ, unit.PosX] = Define.TileType.Wall;

		//Debug.Log($"unit name : {unit.gameObject.name} board My Pos :  { Tile[unit.PosZ, unit.PosX] } TileDestPos { Tile[DestZ, DestX] }");
	}

	

	
}
