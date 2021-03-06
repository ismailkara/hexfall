﻿using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    
    public RectTransform rect;
    public Tile tile;
    
    public int x { get; private set; }
    public int y { get; private set; }

    public List<Slot> neighbors { get; private set; }
    public List<Anchor> connectedAnchors { get; private set; }

    public void init(int i, int j)
    {
        x = i;
        y = j;
        neighbors = new List<Slot>();
        connectedAnchors = new List<Anchor>();
    }

    public void addAnchor(Anchor anchor)
    {
        if (!connectedAnchors.Contains(anchor))
        {
            connectedAnchors.Add(anchor);
        }
    }
    public void addNeighbor(Slot neighbor)
    {
        neighbors.Add(neighbor);
    }

    public void addTile(Tile t)
    {
        tile = t;
        if (t != null)
        {
            tile.slot = this;
            tile.transform.SetParent(transform);
        }
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
