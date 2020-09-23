using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    
    public static Action<List<Slot>> OnTileDestroyed;
    public static Action OnMatch;

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

    public bool calculateGoal(List<Anchor> anchors)
    {
        bool goal = false;
        List<Slot> destroySlots = new List<Slot>();
        foreach (var anchor in anchors)
        {
            bool match = anchor.slots[0].tile.color == anchor.slots[1].tile.color;
            match = match && anchor.slots[0].tile.color == anchor.slots[2].tile.color;

            if (match)
            {
                addSlot(ref destroySlots, anchor.slots[0]);
                addSlot(ref destroySlots, anchor.slots[1]);
                addSlot(ref destroySlots, anchor.slots[2]);
            }

            goal = goal || match;
        }

        OnTileDestroyed?.Invoke(destroySlots);
        return goal;
    }

    void addSlot(ref List<Slot> slots, Slot slot)
    {
        if (!slots.Contains(slot))
        {
            slots.Add(slot);
        }
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
        return calculateGoal(getAnchors(slots));
    }

    List<Slot> calculateGoal(Slot slot)
    {
        List<Slot> result = new List<Slot>();
        result.Add(slot);
        foreach (var other in slot.neighbors)
        {
            if (other.tile.color == slot.tile.color)
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

    List<Anchor> getAnchors(Slot[] slots)
    {
        List<Slot> temp = new List<Slot>();

        foreach (var slot in slots)
        {
            temp.Add(slot);
        }

        return getAnchors(temp);
    }
    List<Anchor> getAnchors(List<Slot> slots)
    {
        List<Anchor> result = new List<Anchor>();
        foreach (var slot in slots)
        {
            foreach (var anchor in slot.connectedAnchors)
            {
                if (!result.Contains(anchor))
                {
                    result.Add(anchor);
                }
            }
        }

        return result;
    }

    public void onMatch()
    {
        OnMatch?.Invoke();
    }
}
