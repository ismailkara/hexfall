using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    
    public static Action<List<Tile>> OnTileDestroyed;

    private Anchor _selected;

    private void Awake()
    {
        Instance = this;

        InputController.OnDragUp += handleDragUp;
        InputController.OnDragDown += handleDragDown;
    }
    

    #region event handlers

    void handleDragUp()
    {
        Debug.Log("drag up");

        if (_selected != null)
        {
            _selected.rotate(1);
        }
        
        
    }

    void handleDragDown()
    {
        Debug.Log("drag down");
        if (_selected != null)
        {
            _selected.rotate(-1);
        }
    }

    #endregion
    
    public void anchorSelected(Anchor anchor)
    {
        _selected = anchor;
        Debug.Log("anchor selected");
        // List<Tile> temp = new List<Tile>();
        // for (int i = 0; i < anchor.slots.Length; i++)
        // {
        //     Slot slot = anchor.slots[i];
        //     temp.Add(slot.tile);
        //     slot.tile = null;
        // }
        // OnTileDestroyed?.Invoke(temp);
    }

    public void slotClicked(Slot s)
    {
        List<Tile> temp = new List<Tile>();
        temp.Add(s.tile);
        for (int i = 0; i < s.neighbors.Count; i++)
        {
            Slot slot = s.neighbors[i];
            temp.Add(slot.tile);
            slot.tile = null;
        }
        OnTileDestroyed?.Invoke(temp);

    }
}
