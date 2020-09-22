using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    
    public RectTransform rect;
    public Tile tile;
    
    public int type { get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }

    public List<Slot> neighbors { get; private set; }

    public void init(int i, int j)
    {
        x = i;
        y = j;
        neighbors = new List<Slot>();
    }

    public void addNeighbor(Slot neighbor)
    {
        neighbors.Add(neighbor);
    }

    public void addTile(Tile t)
    {
        
        tile = t;
        tile.slot = this;
        tile.transform.SetParent(transform);
    }

    void removeTile()
    {
        if (tile == null)
        {
            return;
        }
        tile.slot = null;
        tile = null;
    }
}
