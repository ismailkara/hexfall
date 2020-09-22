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

    void handleTileDestroyed(List<Tile> tiles)
    {
        int index = 0;
        
        foreach (var tile in tiles)
        {
            index++;
            fillEmptySlot(tile.slot);
            tile.recycle();
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
        for (int i = 0; i < boardHeight; i++)
        {
            Slot lowerSlot = _board[slot.x, i];
            if (lowerSlot.tile == null)
            {
                
                
                bool found = false;
                for (int j = i + 1; j < boardHeight; j++)
                {
                    Slot upperSlot = _board[slot.x, j];
                    if (upperSlot.tile != null)
                    {
                        found = true;
                        lowerSlot.addTile(upperSlot.tile);
                        lowerSlot.tile.dropDown(j * .1f);
                        // lowerSlot.tile.resetPosition();
                        upperSlot.tile = null;
                        break;
                    }
                }

                if (!found)
                {
                    Tile tile = spawnTile(lowerSlot);
                    tile.dropFromAbove(lowerSlot.y * .1f);
                }
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
        return tile;
    }
    
    
}
