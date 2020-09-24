using UnityEngine;

public class ClickListener : MonoBehaviour
{
    public void onNewGameClicked()
    {
        GameController.Instance.newGame();
    }

    public void onEndGameClicked()
    {
        GameController.Instance.bombExploded();
    }
}
