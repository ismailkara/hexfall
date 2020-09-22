using System;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public static TileController Instance;
    public GameObject tilePrefab;

    private Slot[,] _board;
    private ObjectPool _tilePool;
    private GameConfig _currentConfig;
    private void Awake()
    {
        Instance = this;
        init();
    }

    void init()
    {
        subscribeEvents();
        _tilePool = new ObjectPool(tilePrefab);
    }

    void subscribeEvents()
    {
        GameLogic.OnTileDestroyed += handleTileDestroyed;
        BoardController.OnBoardReady += handleBoardReady;
    }

    #region event handlers

    void handleTileDestroyed(List<Slot> slots)
    {
        for (var index = 0; index < slots.Count; index++)
        {
            Slot slot = slots[index];
            slot.tile.recycle();
            slot.tile = null;
            fillEmptySlot(slot);

        }
    }
    void handleBoardReady(GameConfig config, Slot[,] slots)
    {
        _currentConfig = config;
        _board = slots;
        fillBoard();
    }

  

    #endregion

    void fillBoard()
    {
        foreach (var slot in _board)
        {

            Tile tile = spawnTile(slot);
            tile.resetPosition();
        }
    }

    void fillEmptySlot(Slot slot)
    {
        int boardHeight = _board.GetLength(1);
        int horizontalIndex = slot.x;
        for (int i = slot.y; i < boardHeight; i++)
        {
            Slot lowerSlot = _board[horizontalIndex, i];

            if (lowerSlot.tile == null)
            {
                for (int j = i + 1; j < boardHeight; j++)
                {
                    Slot higherSlot = _board[horizontalIndex, j];
                    Debug.Log(lowerSlot.name + "  " + higherSlot.name);
                    if (higherSlot.tile != null)
                    {
                        lowerSlot.addTile(higherSlot.tile);
                        higherSlot.tile = null;
                        lowerSlot.tile.resetPosition();
                        break;
                    }
                }
                
            }
        }

    }

    void lowerTiles(Slot startSlot)
    {
        Debug.Log("lower tile");
        int boardHeight = _board.GetLength(1);
        
        for (int i = startSlot.y; i < boardHeight - 1; i++)
        {
            Slot lowerSlot = _board[startSlot.x, i];
            Slot higherSlot = _board[startSlot.x, i + 1];
            if (lowerSlot.tile == null)
            {
                lowerSlot.addTile(higherSlot.tile);
                higherSlot.tile = null;
                Debug.Log(lowerSlot.name);
                if(lowerSlot == null) lowerSlot.tile.resetPosition();
            }
        }
    }

    private Tile spawnTile(Slot slot)
    {
        
        int type = _currentConfig.getRandomType();
        Color color = _currentConfig.getColorOfType(type);

        Tile tile = _tilePool.get<Tile>();
        tile.setUp(slot, _tilePool);
        tile.setType(type, color);
        slot.addTile(tile);
        tile.transform.localScale = Vector3.one;
        return tile;
    }
    
    
}
