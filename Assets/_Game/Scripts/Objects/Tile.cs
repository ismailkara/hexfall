using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image _faceImage;
    
    public RectTransform rect;
    public int type { get; private set; }

    public void init(BoardController boardController, GameConfig config)
    {
        type = Random.Range(0, config.colors.Length);
        _faceImage.color = config.colors[type];
    }
}
