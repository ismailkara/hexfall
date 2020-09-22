using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    public GameObject tilePrefab;

    private Slot[,] _slots;
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
        GameController.OnNewGame += handleNewGame;
        BoardController.OnBoardReady += handleBoardReady;
    }

    #region event handlers

    void handleBoardReady(Slot[,] slots)
    {
        _slots = slots;
        fillBoard();
    }

    void handleNewGame(GameConfig config)
    {
        _currentConfig = config;
    }

    #endregion

    void fillBoard()
    {
        foreach (var slot in _slots)
        {
            slot.addTile(_tilePool.get<Tile>());
        }
    }

    public void anchorSelected(Anchor anchor)
    {
        for (int i = 0; i < anchor.slots.Length; i++)
        {
            Slot slot = anchor.slots[i];
            anchor.slots[i] = null;
            _tilePool.recycle(slot.tile);
        }
    }
    
}
