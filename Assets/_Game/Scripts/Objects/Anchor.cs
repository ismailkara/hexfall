using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    public RectTransform rect;
    
    public Slot[] slots { get; private set; }
    public List<Anchor> anchors { get; private set; }

    private Transform _slotHolder;

    public void onClick()
    {
        // foreach (var slot in slots)
        // {
        //     slot.transform.localScale = .9f * Vector3.one;
        // }
        GameLogic.Instance.anchorSelected(this);
    }

    public void init(Transform holder)
    {
        _slotHolder = holder;
        anchors = new List<Anchor>();
    }

    public void addAnchor(Anchor other)
    {
        if(!anchors.Contains(other)) anchors.Add(other);
    }
    public void addSlots(List<Slot> tiles)
    {
        slots = new Slot[3];
        tiles = arrangeSlots(tiles);
        for (int i = 0; i < 3; i++)
        {
            slots[i] = tiles[i];
            slots[i].addAnchor(this);
        }
    }

    //slot ların acısına gore sıralı olması gerekir
    List<Slot> arrangeSlots(List<Slot> tiles)
    {
        List<Slot> result = new List<Slot>();
        float minAngle = 9999;
        float maxAngle = -9999;

        Slot max = null;
        Slot min = null;

        foreach (var slot in tiles)
        {
            Vector2 distance = slot.rect.anchoredPosition - rect.anchoredPosition;
            float angle = Mathf.Atan2(distance.y, distance.x);
            if (maxAngle < angle)
            {
                maxAngle = angle;
                max = slot;
            }
            
            if (minAngle > angle)
            {
                minAngle = angle;
                min = slot;
            }
            
        }

        tiles.Remove(min);
        tiles.Remove(max);
        
        result.Add(min);
        result.Add(tiles[0]);
        result.Add(max);

        return result;
    }

    public void rotate(int direction, int index)
    {
        InputController.Instance.disableInput();
        transform.SetAsLastSibling();
        foreach (var slot in slots)
        {
            slot.tile.transform.SetParent(transform);
        }
        Vector3 eulerAngles = transform.localRotation.eulerAngles;
        Move move = new Move(gameObject);
        move.obj.endRotation = Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z + (direction * 120)));
        move.animTime = .5f;

        move.callback += () =>
        {
            foreach (var slot in slots)
            {
                slot.tile.transform.SetParent(_slotHolder);
            }
            
            bool match = GameLogic.Instance.calculateGoal(slots);
            
            index++;
            if (!match)
            {
                if (index < 3)
                {
                    Moves.instance.executeWithDelay(() =>
                    {
                        rotate(direction, index);
                        
                    }, .1f);
                }
                else
                {
                    InputController.Instance.enableInput();
                }
            }
        };
        
        
        rotateTilesLogic(direction);
        
        move.run();

    }

    void rotateTilesLogic(int direction)
    {
        List<Tile> temp = new List<Tile>();
        for (int i = 0; i < slots.Length; i++)
        {
            temp.Add(slots[i].tile);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            int index = (i + 3 - direction) % 3;
            slots[i].tile = temp[index];
        }
    }

    public void select()
    {
        foreach (var slot in slots)
        {
            slot.tile.setSelectActive(true);
        }
    }

    public void deselect()
    {
        foreach (var slot in slots)
        {
            slot.tile.setSelectActive(false);
        }
    }
}
