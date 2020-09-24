using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPopup;

    private void Awake()
    {
        GameController.OnNewGame += handleNewGame;
        GameController.OnGameOver += handleGameOver;
    }

    #region event handlers

    void handleNewGame(GameConfig config)
    {
        gameOverPopup.SetActive(false);
    }
    void handleGameOver()
    {
        Moves.instance.executeWithDelay(() =>
        {
            gameOverPopup.SetActive(true);
        }, .6f);
    }
    

    #endregion
    
    
}
