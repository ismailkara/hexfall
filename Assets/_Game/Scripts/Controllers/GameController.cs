﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public static Action<GameConfig> OnNewGame;
    public static Action OnGameOver;
    
    public GameConfig easy, hard;

    private void Awake()
    {
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
            newGame();
        }
    }
#endif

    public void newGame()
    {
        OnNewGame?.Invoke(hard);
    }

    public void bombExploded()
    {
        
    }

    public void noMoreMoves()
    {
        
    }
    void gameOver()
    {
        OnGameOver?.Invoke();
    }
}
