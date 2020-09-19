using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance;

    private const float TileSizeRatio = 1.1547005383792515290182975610039149112952035025402537520372046529f; // duzgun altıgenin boy/en oranı, 2/√3
    
    [SerializeField] private GameObject tilePrefab, anchorPrefab;
    [SerializeField] private RectTransform slotHolder, anchorHolder;
    
    
    
    private Slot[,] _board;
    private List<Anchor> _anchors;
    private ObjectPool _slotPool, _anchorPool;
    private Vector2 _tileSize;
    private Vector2 _boardOffet;
    

    private GameConfig currentConfig;
    private void Awake()
    {
        Instance = this;
        init();
    }

    void init()
    {
        _slotPool = new ObjectPool(tilePrefab);
        _anchorPool = new ObjectPool(anchorPrefab);
        subscribeEvents();
    }
  

    void subscribeEvents()
    {
        GameController.OnNewGame += handleNewGame;
    }
    #region event handlers

    void handleNewGame(GameConfig config)
    {
        currentConfig = config;
        calculateCellSize();
        buildBoard();
        buildAnchors();
        registerSlotsToAnchors();
    }

    #endregion

    void buildBoard()
    {
        _board = new Slot[currentConfig.boardWidth, currentConfig.boardHeight];

        for (int i = 0; i < currentConfig.boardWidth; i++)
        {
            for (int j = 0; j < currentConfig.boardHeight; j++)
            {
                Slot slot = spawnTile(i, j);
                _board[i, j] = slot;
            }
        }
    }

    void buildAnchors()
    {
        _anchors = new List<Anchor>();
        
        foreach (var slot in _board)
        {
            bool evenColumn = slot.x % 2 == 0;
            
            bool bottomRow = slot.y == 0;
            bool topRow = slot.y == _board.GetLength(1) - 1;
            
            bool leftColumn = slot.x == 0;
            bool rightColumn = slot.x == _board.GetLength(0) - 1;

            bool bottomAndOddColumn = bottomRow && !evenColumn;
            bool topAndEvenColumn = topRow && evenColumn;

            bool notEdge = !topAndEvenColumn && !bottomAndOddColumn;
            
            
            Vector2 anchorPosition;

            Anchor anchor = null;
            if (!leftColumn)
            {
                if (!topAndEvenColumn && !bottomAndOddColumn)
                {
                    anchor = _anchorPool.get<Anchor>();
                    anchor.transform.SetParent(anchorHolder);
                    
                    anchorPosition = slot.rect.anchoredPosition + (.5f * _tileSize.x * Vector2.left);
                    
                    anchor.rect.anchoredPosition = anchorPosition;
                    
                    _anchors.Add(anchor);
                }
                
            }

            if (!rightColumn)
            {
                if (notEdge)
                {
                    anchor = _anchorPool.get<Anchor>();
                    anchor.transform.SetParent(anchorHolder);
                    
                    anchorPosition = slot.rect.anchoredPosition + (.5f * _tileSize.x * Vector2.right);
                    
                    anchor.rect.anchoredPosition = anchorPosition;
                    
                    _anchors.Add(anchor);

                }

            }
            
        }
    }


    void registerSlotsToAnchors()
    {
        float distanceTreshold = _tileSize.x * .55f; // tile ın eninin yarısı altıgenin kosesinin merkeze uzaklıgı kadar. Float esitlemek guvenilir olmadıgı icin .5 degil .55 aldım.
        distanceTreshold *= distanceTreshold; // daha performanslı oldugu icin magnetude degil sqrtMagnitude karsılastırıyorum
        List<Slot> temp;
        foreach (var anchor in _anchors)
        {
            temp = new List<Slot>();
            int index = 0;
            foreach (var slot in _board)
            {
                Vector2 distance = slot.rect.anchoredPosition - anchor.rect.anchoredPosition;

                if (distance.sqrMagnitude < distanceTreshold)
                {
                    temp.Add(slot);
                    index++;
                    if (index == 3)
                    {
                        break;
                    }
                }
            }
            anchor.addSlots(temp);
        }
    }

    Slot spawnTile(int i, int j)
    {
        Slot temp = _slotPool.get<Slot>();
        temp.init(this, currentConfig, i, j);
        temp.transform.SetParent(slotHolder);
        
        temp.rect.sizeDelta = _tileSize;

        temp.rect.anchoredPosition = calculatePosition(i, j);
        
        return temp;
    }

    Vector2 calculatePosition(int i, int j)
    {
        Vector2 position = Vector2.zero;
        
        position = new Vector2(i * .75f, j) * _tileSize;
        
        position += (i % 2) * .5f * _tileSize.y * Vector2.down; // tek indexteki stunlarn pozisyonlarını asagıya itiyor
        
        
        // buradan sonrakiler board da ortalamak icin, anchor larını  merkezde tutmak istedim o yuzden elle ayarladım 
        // yatay ortalamak
        position += _tileSize.x *  _boardOffet.x * .25f * Vector2.left; 
        position += _tileSize.x * .5f * Vector2.right;
        
        
        position += _tileSize.y *  _boardOffet.y * .5f * Vector2.down; 
        position += _tileSize.y * Vector2.up;
        
        return position;
    }
    

    //board alanına sıgacak en buyuk tile'ı hesaplama
    void calculateCellSize()
    {
        float possibleMaximumWidth = 0;
        
        
        //yatayda kontrol
        possibleMaximumWidth = calculateMaxWidth();
        float heightCalculatedWidth = CalculateMaxWidthForHeight();

        if (possibleMaximumWidth > heightCalculatedWidth)
        {
            possibleMaximumWidth = heightCalculatedWidth;
        }


        _tileSize = new Vector2(possibleMaximumWidth, possibleMaximumWidth / TileSizeRatio);
    }

    float calculateMaxWidth()
    {
        //altıgenin bir kenarı 1 birim olarak alırsak

        float doubleColumnWidth = 3; //her 2 stunun ekledigi genislik

        float boardWidth = Mathf.Floor((float) currentConfig.boardWidth / 2) * doubleColumnWidth;
        boardWidth += .5f; // sonuncu cıkıntısı
        boardWidth += (currentConfig.boardWidth % 2) * 1.5f; // tek sayıda stun varsa en sona eklenecek uzunluk;

        _boardOffet.x = boardWidth;

        return 2 * slotHolder.rect.width / boardWidth; // x2 cunku bir kenar 1 birim uzunlugunda olursa altıgeni cevreleyen imajın genisiligi 2 olur
    }

    private float CalculateMaxWidthForHeight()
    {
        //oyunun oynanabilmesi icin en az iki stun gerekir, 1 stun olma durumunu hesaplamadım o yuzden


        float boardHeight = currentConfig.boardHeight + .5f; // .5 sonuncunun cıkıntısı
        
        _boardOffet.y = boardHeight;
        
        float tileHeight = slotHolder.rect.height / boardHeight;
        float tileWidth = tileHeight * TileSizeRatio;
        

        return tileWidth;
    }


}
