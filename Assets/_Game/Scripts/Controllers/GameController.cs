using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public static Action<GameConfig> OnNewGame;

    public GameConfig easy, hard;

    private void Start()
    {
        newGame();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
#endif

    void newGame()
    {
        OnNewGame?.Invoke(hard);
    }
}
