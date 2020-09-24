using UnityEngine;

public class ClickListener : MonoBehaviour
{
    public void onNewGameClicked()
    {
        GameController.Instance.newGame();
    }
}
