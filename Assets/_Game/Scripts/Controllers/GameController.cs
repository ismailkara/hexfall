using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public static Action<GameConfig> OnNewGame;

    public GameConfig easy, hard;

    private void Start()
    {
        newGame();
    }

    void newGame()
    {
        OnNewGame?.Invoke(hard);
    }
}
