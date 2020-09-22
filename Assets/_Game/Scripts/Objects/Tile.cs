using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public RectTransform rect;
    public Slot slot;// { get; private set; }
    public int type { get; private set; }

    private const float DropTime = 1.5f;
    private const float DropDistance = 600f;

    [SerializeField] private GameObject select;
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
        setSelectActive(false);
        _tilePool.recycle(this);
    }

    public void dropFromAbove(float delay)
    {
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        transform.localPosition = Vector3.up * DropDistance;
        
        Move drop = new Move(gameObject);
        drop.moveType = MoveType.LOCAL;

        drop.animTime = DropTime * 1.7f;
        drop.delay = delay;
        

        drop.obj.endLocalPosition = Vector3.zero;

        drop.run();
    }
    public void dropDown(float delay)
    {
        Move drop = new Move(gameObject);
        drop.moveType = MoveType.LOCAL;

        drop.animTime = DropTime;
        drop.delay = delay;
        
        drop.obj.endLocalPosition = Vector3.zero;

        drop.run();
    }
    public void resetPosition()
    {
        
        transform.localPosition = Vector3.zero;
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
    }

    public void setSelectActive(bool active)
    {
        @select.gameObject.SetActive(active);
    }
    public void onClick()
    {
        // GameLogic.Instance.slotClicked(slot);
    }
}
