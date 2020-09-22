using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public RectTransform rect;
    public Slot slot { get; private set; }
    public int type { get; private set; }
    
    [SerializeField] private Image _faceImage;

    private ObjectPool _tilePool;
    
    

    public void setUp(Slot slot, ObjectPool pool)
    {
        _tilePool = pool;
        this.slot = slot;
    }

    public void setType(int t, Color color)
    {
        type = t;
        _faceImage.color = color;
    }

    public void recycle()
    {
        _tilePool.recycle(this);
    }
}
