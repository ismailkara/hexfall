using System;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance;

    private const float CellSizeRatio = 1.1547005383792515290182975610039149112952035025402537520372046529f; // duzgun altıgenin en/boy oranı, 2/√3
    
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private RectTransform boardRect;
    
    private Tile[,] board;
    private ObjectPool pool;
    

    private GameConfig currentConfig;
    private void Awake()
    {
        Instance = this;
        init();
    }

    void init()
    {
        pool = new ObjectPool(tilePrefab);
        subscribeEvents();
    }
  

    void subscribeEvents()
    {
        GameController.OnNewGame += handleNewGame;
    }
    #region event handlers

    void handleNewGame(GameConfig config)
    {
        currentConfig = config;
        buildBoard();
    }

    #endregion

    void buildBoard()
    {
        board = new Tile[currentConfig.boardWidth, currentConfig.boardHeight];

        for (int i = 0; i < currentConfig.boardWidth; i++)
        {
            for (int j = 0; j < currentConfig.boardHeight; j++)
            {
                spawnTile(i, j);
            }
        }
    }

    void spawnTile(int i, int j)
    {
        Tile temp = pool.get<Tile>();
        
    }
}
