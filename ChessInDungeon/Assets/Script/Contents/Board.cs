using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static Board s_instance = null;
    public static Board Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<Board>();
                Debug.Log("New Board!");
            }
            return s_instance;
        }
    }

	UnitController _unit;
   
    public int Size { get; private set; }

    public Define.TileType[,] Tile { get;  private set; }

    public void BoardInitialize(int boardSize, int targetDestZ, int targetDestX, UnitController unit)
	{
		_unit = unit;

        Debug.Log($" {_unit.gameObject.name}ÀÇ Å¸°Ù Z : {targetDestZ} Å¸°Ù X : {targetDestX}");

        Size = boardSize;

        if (Tile == null)
        {
            Tile = new Define.TileType[Size, Size];
            Debug.Log("Create Tile!");
        }
        
        SetTileData(targetDestZ, targetDestX, _unit);
    }

    void SetTileData(int unitDestZ, int unitDestX, UnitController unit)
    {
        _unit = unit;
        Tile[_unit.PosZ, _unit.PosX] = Define.TileType.InUnit;
        Tile[unitDestZ, unitDestX] = Define.TileType.InUnit;
    }

}
