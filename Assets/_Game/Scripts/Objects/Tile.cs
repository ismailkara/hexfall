using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public RectTransform rect;
    public Slot slot;// { get; private set; }
    public int color { get; private set; }
    public TileType type { get; private set; }

    private const float DropTime = .4f;
    private const float DropDistance = 800f;

    [SerializeField] private GameObject select, star;
    [SerializeField] private Image faceImage;
    [SerializeField] private Text bombCount;

    private ObjectPool _tilePool;
    private int _countDown;


    private void Awake()
    {
        GameLogic.OnMatch += onMatch;
    }

    public void setUp(Slot slot, ObjectPool pool, TileType t)
    {
        type = t;
        
        star.SetActive(type == TileType.Starred);
        bombCount.gameObject.SetActive(type == TileType.Bomb);
        
        if (t == TileType.Bomb)
        {
            _countDown = 6;
            updateBomb();
        }
        
        _tilePool = pool;
        this.slot = slot;
    }

    public void setType(int t, Color color)
    {
        this.color = t;
        faceImage.color = color;
    }

    void onMatch()
    {
        if (type == TileType.Bomb)
        {
            _countDown--;
            updateBomb();
        }
    }
    public void recycle()
    {
        // Destroy(gameObject);
        setSelectActive(false);
        _tilePool.recycle(this);
    }

    public void dropFromAbove(float delay, Action callback)
    {
        
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        transform.localPosition = Vector3.up * DropDistance;
        
        Move drop = new Move(gameObject);
        drop.moveType = MoveType.LOCAL;

        drop.animTime = DropTime * 1.2f;
        drop.delay = delay;
        

        drop.obj.endLocalPosition = Vector3.zero;

        drop.callback += () =>
        {
            callback?.Invoke();
        };
        
        drop.run();
    }
    public void dropDown(float delay, Action callback)
    {
        Move drop = new Move(gameObject);
        drop.moveType = MoveType.LOCAL;

        drop.animTime = DropTime;
        drop.delay = delay;
        
        drop.obj.endLocalPosition = Vector3.zero;

        drop.callback += () =>
        {
            callback?.Invoke();
        };
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
        select.gameObject.SetActive(active);
    }
    public void onClick()
    {
        // GameLogic.Instance.slotClicked(slot);
    }

    void updateBomb()
    {
        
        bombCount.text = _countDown.ToString();
    }
}
public enum TileType {Normal, Starred, Bomb}