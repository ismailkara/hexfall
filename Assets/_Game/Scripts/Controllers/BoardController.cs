using System;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private RectTransform boardRect;

    private Tile[,] board;
    

    private GameConfig currentConfig;
    private void Awake()
    {
        Instance = this;
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
    }

    void spawnTile()
    {
        
    }
}
