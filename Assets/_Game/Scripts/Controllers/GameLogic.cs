using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    
    public static Action<List<Slot>> OnTileDestroyed;

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
            _selected.rotate(1, 0);
        }
        
        
    }

    void handleDragDown()
    {
        if (_selected != null)
        {
            _selected.rotate(-1, 0);
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
        
        // List<Slot> temp = new List<Slot>();
        // for (int i = 0; i < anchor.slots.Length; i++)
        // {
        //     Slot slot = anchor.slots[i];
        //     temp.Add(slot);
        // }
        // OnTileDestroyed?.Invoke(temp);
    }

    public bool calculateGoal(List<Slot> slots)
    {
        Slot[] temp = new Slot[slots.Count];

        for (int i = 0; i < slots.Count; i++)
        {
            temp[i] = slots[i];
        }

        return calculateGoal(temp);
    }
    public bool calculateGoal(Slot[] slots)
    {
        List<Slot> destroySlots = new List<Slot>();

        foreach (var slot in slots)
        {
            List<Slot> temp = calculateGoal(slot);
            foreach (var tile in temp)
            {
                if(!destroySlots.Contains(tile)) destroySlots.Add(tile);
            }
        }

        if (destroySlots.Count != 0)
        {
            _selected = null;        
            OnTileDestroyed?.Invoke(destroySlots);
            return true;
        }

        return false;
    }

    List<Slot> calculateGoal(Slot slot)
    {
        List<Slot> result = new List<Slot>();
        result.Add(slot);
        foreach (var other in slot.neighbors)
        {
            if (other.tile.type == slot.tile.type)
            {
                result.Add(other);
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
        List<Slot> temp = new List<Slot>();
        temp.Add(s);
        for (int i = 0; i < s.neighbors.Count; i++)
        {
            Slot slot = s.neighbors[i];
            temp.Add(slot);
            slot.tile = null;
        }
        OnTileDestroyed?.Invoke(temp);

    }
}
