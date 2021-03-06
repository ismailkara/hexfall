﻿using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public static Action<GameConfig> OnNewGame;
    public static Action OnGameOver;
    
    public GameConfig gameConfig;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Instance = this;
    }

    private void Start()
    {
        newGame();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameOver();
        }
    }
#endif

    public void newGame()
    {
        OnNewGame?.Invoke(gameConfig);
    }
    
    

    public void bombExploded()
    {
        gameOver();
    }

    void gameOver()
    {
        OnGameOver?.Invoke();
    }
}
