using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image _faceImage;
    
    public RectTransform rect;
    public int type { get; private set; }
    public int x;// { get;  set; }
    public int y;// { get;  set; }

    public void init(BoardController boardController, GameConfig config, int i, int j)
    {
        type = Random.Range(0, config.colors.Length);
        _faceImage.color = config.colors[type];

        x = i;
        y = j;
    }
}
