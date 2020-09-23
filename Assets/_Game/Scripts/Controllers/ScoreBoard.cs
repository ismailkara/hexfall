using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    private const int Point = 5;
    void Awake()
    {
        subscribeEvents();
    }

    void subscribeEvents()
    {
        GameLogic.OnTileDestroyed += handleTilesDestroyed;
    }

    #region event handlers

    void handleTilesDestroyed(List<Slot> slots)
    {
        int gainedScore = 0;
        foreach (var slot in slots)
        {
            
        }
    }
    

    #endregion
    // Update is called once per frame
    void Update()
    {
        
    }
}
