using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    public RectTransform rect;
    
    public Tile[] slots;

    public void addSlots(List<Tile> tiles)
    {
        slots = new Tile[3];
        tiles = arrangeSlots(tiles);
        for (int i = 0; i < 3; i++)
        {
            slots[i] = tiles[i];
        }
    }

    //slot ların acısına gore sıralı olması gerekir
    List<Tile> arrangeSlots(List<Tile> tiles)
    {
        List<Tile> result = new List<Tile>();
        float minAngle = 9999;
        float maxAngle = -9999;

        Tile max = null;
        Tile min = null;

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
}
