using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public static TileController Instance;
    public GameObject tilePrefab;

    private Slot[,] _board;
    private List<Anchor> _anchors;
    private ObjectPool _tilePool;
    private GameConfig _currentConfig;

    private int _movingTileCount = 0;
    private List<Slot> _movingTiles;
    private List<Tile> _tiles;

    private Dice _dice;
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
        GameController.OnGameOver += handleGameOver;
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
    void handleBoardReady(GameConfig config, Slot[,] slots, List<Anchor> anchors)
    {
        _anchors = anchors;
        _tiles = new List<Tile>();
        
        _dice = new Dice();
        _dice.add(TileType.Normal, 90);
        _dice.add(TileType.Starred, 10);
        
        _currentConfig = config;
        _board = slots;
        fillBoard();
        fillColors();
        
    }

    void handleGameOver()
    {
        foreach (var tile in _tiles)
        {
            tile.recycle();
        }
    }

  

    #endregion

    void fillBoard()
    {
        foreach (var slot in _board)
        {

            Tile tile = spawnEmptyTile(slot);
            tile.resetPosition();
        }
    }

    //baslangıcta match durumu olmaması icin komsuları kontrol ederek renkleri dolduruyorum
    void fillColors()
    {
        foreach (var anchor in _anchors)
        {

            foreach (var slot in anchor.slots)
            {
                if (slot.tile.color != - 1)
                {
                    continue;
                }
                List<int> bannedColors = new List<int>();

                foreach (var slotAnchor in slot.connectedAnchors)
                {
                    bannedColors.Add(slotAnchor.calculateBannedColor());
                }
                int color = getRandomIntExcludeBanned(_currentConfig.colors.Length, bannedColors);
                // Debug.Log("banned color " + bannedColor + "  " + color + "   " + (bannedColor == color));
                
                slot.tile.setColor(color, _currentConfig.getColorOfType(color));
            }
            
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


    private Tile spawnEmptyTile(Slot slot)
    {
        int type = -1;

        Tile tile = _tilePool.get<Tile>();
        tile.setUp(slot, _tilePool, getRandomType());
        tile.setColor(type, Color.white);
        slot.addTile(tile);
        tile.transform.localScale = Vector3.one;
        
        _tiles.Add(tile);
        return tile;
    }
    private Tile spawnTile(Slot slot)
    {
        
        int type = _currentConfig.getRandomType();
        Color color = _currentConfig.getColorOfType(type);

        Tile tile = _tilePool.get<Tile>();
        tile.setUp(slot, _tilePool, getRandomType());
        tile.setColor(type, color);
        slot.addTile(tile);
        tile.transform.localScale = Vector3.one;
        
        _tiles.Add(tile);
        return tile;
    }

    public void enableBomb()
    {
        _dice.add(TileType.Bomb, 2);
    }

    TileType getRandomType()
    {
        return _dice.roll<TileType>();
    }

    private int getRandomIntExcludeBanned(int max, List<int> banned)
    {
        List<int> pool = new List<int>();

        for (int i = 0; i < max; i++)
        {
            if(!banned.Contains(i)) pool.Add(i);
        }

        return pool.GetRandom();
    }
    
}
