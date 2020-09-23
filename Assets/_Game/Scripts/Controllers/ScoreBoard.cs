using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    private const int Point = 5;

    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject miniScorePrefab;
    [SerializeField] private Transform pointHolder;

    private ObjectPool _scorePool;

    private int score;
    void Awake()
    {
        _scorePool = new ObjectPool(miniScorePrefab);
        subscribeEvents();
    }

    void subscribeEvents()
    {
        GameController.OnNewGame += handleNewGame;
        GameLogic.OnTileDestroyed += handleTilesDestroyed;
    }

    #region event handlers

    void handleNewGame(GameConfig config)
    {
        score = 0;
        updateScoreBoard();
    }
    void handleTilesDestroyed(List<Slot> slots)
    {
        int gainedScore = 0;
        foreach (var slot in slots)
        {
            if (slot.tile.type == TileType.Starred)
            {
                gainedScore += getScore(slot.tile);
            }
            else
            {
                gainedScore += getScore(slot.tile);;
            }
            spawnPoint(slot.tile);
        }

        if (score < 1000 && score + gainedScore >= 1000)
        {
            TileController.Instance.enableBomb();
        }

        score += gainedScore;
        updateScoreBoard();
        
    }
    

    #endregion

    void updateScoreBoard()
    {
        scoreText.text = score.ToString();
    }

    int getScore(Tile tile)
    {
        if (tile.type == TileType.Starred)
        {
            return 20;
        }

        return 5;
    }

    void spawnPoint(Tile tile)
    {
        Text point = _scorePool.get<Text>();
        point.transform.SetParent(pointHolder);
        point.transform.localScale = Vector3.one;
        
        point.transform.position = tile.transform.position;
        
        point.text = "+" + getScore(tile);
        
        Move move = new Move(point.gameObject);
        move.animTime = .3f;
        move.moveType = MoveType.WORLD;
        move.obj.endPosition += Vector3.up * 50;
        
        Move shrink = new Move(point.gameObject);
        shrink.animTime = .2f;
        shrink.obj.endScale = Vector3.zero;
        shrink.callback = () =>
        {
            _scorePool.recycle(point);
        };

        move.callback = () =>
        {
            shrink.run();
        };
        move.run();
    }
}
