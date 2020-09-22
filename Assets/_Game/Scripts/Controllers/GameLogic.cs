using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    
    public static Action<List<Tile>> OnTileDestroyed;

    private void Awake()
    {
        Instance = this;
    }
    
    
    public void anchorSelected(Anchor anchor)
    {
        List<Tile> temp = new List<Tile>();
        for (int i = 0; i < anchor.slots.Length; i++)
        {
            Slot slot = anchor.slots[i];
            temp.Add(slot.tile);
            slot.tile.recycle();
            slot.tile = null;
        }
        OnTileDestroyed?.Invoke(temp);
    }
}
