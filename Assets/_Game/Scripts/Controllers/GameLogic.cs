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

        if (_selected != null)
        {
            _selected.rotate(1);
        }
        
        
    }

    void handleDragDown()
    {
        if (_selected != null)
        {
            _selected.rotate(-1);
        }
    }

    #endregion
    
    public void anchorSelected(Anchor anchor)
    {
        if (_selected != null)
        {
            _selected.deselect();
        }
        _selected = anchor;
        _selected.select();
        // List<Tile> temp = new List<Tile>();
        // for (int i = 0; i < anchor.slots.Length; i++)
        // {
        //     Slot slot = anchor.slots[i];
        //     temp.Add(slot.tile);
        //     slot.tile = null;
        // }
        // OnTileDestroyed?.Invoke(temp);
    }

    public void calculateGoal(Slot[] slots)
    {
        List<Tile> destroyTiles = new List<Tile>();

        foreach (var slot in slots)
        {
            List<Tile> temp = calculateGoal(slot);
            foreach (var tile in temp)
            {
                if(!destroyTiles.Contains(tile)) destroyTiles.Add(tile);
            }
        }
        OnTileDestroyed?.Invoke(destroyTiles);
    }

    List<Tile> calculateGoal(Slot slot)
    {
        List<Tile> result = new List<Tile>();
        result.Add(slot.tile);
        foreach (var other in slot.neighbors)
        {
            if (other.tile.type == slot.tile.type)
            {
                result.Add(other.tile);
            }
        }

        if (result.Count < 3)
        {
            result.Clear();
        }

        return result;
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
