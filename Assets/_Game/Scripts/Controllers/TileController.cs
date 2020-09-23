using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public static TileController Instance;
    public GameObject tilePrefab;

    private Slot[,] _board;
    private ObjectPool _tilePool;
    private GameConfig _currentConfig;

    private int _movingTileCount = 0;
    private List<Slot> _movingTiles;
    private void Awake()
    {
        Instance = this;
        init();
    }

    void init()
    {
        
        subscribeEvents();
        _tilePool = new ObjectPool(tilePrefab);
        _movingTiles = new List<Slot>();
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

        }

        for (var index = 0; index < slots.Count; index++)
        {
            Slot slot = slots[index];
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
        // return;
        int boardHeight = _board.GetLength(1);
        int horizontalIndex = slot.x;
        for (int i = slot.y; i < boardHeight; i++)
        {
            Slot lowerSlot = _board[horizontalIndex, i];

            if (lowerSlot.tile == null)
            {
                bool found = false;
                for (int j = i + 1; j < boardHeight; j++)
                {
                    Slot higherSlot = _board[horizontalIndex, j];
                    if (higherSlot.tile != null)
                    {
                        lowerSlot.addTile(higherSlot.tile);
                        higherSlot.tile = null;
                        _movingTileCount++;
                        _movingTiles.Add(lowerSlot);
                        lowerSlot.tile.dropDown( i * .1f, tileMoveEndCallback);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Tile tile = spawnTile(lowerSlot);
                    _movingTileCount++;
                    _movingTiles.Add(lowerSlot);
                    tile.dropFromAbove(i * .1f, tileMoveEndCallback);
                }
            }
        }
    }

    void tileMoveEndCallback()
    {
        _movingTileCount--;
        if (_movingTileCount == 0)
        {
            Moves.instance.executeWithDelay(() =>
            {
                bool goal = GameLogic.Instance.calculateGoal(_movingTiles);
                if (!goal)
                {
                    InputController.Instance.enableInput();
                }
            }, .3f);
            
        }
    }


    private Tile spawnTile(Slot slot)
    {
        
        int type = _currentConfig.getRandomType();
        Color color = _currentConfig.getColorOfType(type);

        Tile tile = _tilePool.get<Tile>();
        tile.setUp(slot, _tilePool, getRandomType());
        tile.setType(type, color);
        slot.addTile(tile);
        tile.transform.localScale = Vector3.one;
        return tile;
    }


    TileType getRandomType()
    {
        return (TileType) Random.Range(0, 3);
    }
}
