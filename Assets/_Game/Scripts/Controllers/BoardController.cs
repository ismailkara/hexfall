using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance;

    private const float TileSizeRatio = 1.1547005383792515290182975610039149112952035025402537520372046529f; // duzgun altıgenin boy/en oranı, 2/√3
    
    [SerializeField] private GameObject tilePrefab, anchorPrefab;
    [SerializeField] private RectTransform slotHolder, anchorHolder;
    
    
    
    private Tile[,] board;
    private ObjectPool slotPool, anchorPool;
    private Vector2 tileSize;
    private Vector2 boardOffet;
    

    private GameConfig currentConfig;
    private void Awake()
    {
        Instance = this;
        init();
    }

    void init()
    {
        slotPool = new ObjectPool(tilePrefab);
        anchorPool = new ObjectPool(anchorPrefab);
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
    }

    #endregion

    void buildBoard()
    {
        board = new Tile[currentConfig.boardWidth, currentConfig.boardHeight];

        for (int i = 0; i < currentConfig.boardWidth; i++)
        {
            for (int j = 0; j < currentConfig.boardHeight; j++)
            {
                Tile tile = spawnTile(i, j);
                board[i, j] = tile;
            }
        }
    }

    void buildAnchors()
    {
        foreach (var slot in board)
        {
            bool evenColumn = slot.x % 2 == 0;
            
            bool bottomRow = slot.y == 0;
            bool topRow = slot.y == board.GetLength(1) - 1;
            
            bool leftColumn = slot.x == 0;
            bool rightColumn = slot.x == board.GetLength(0) - 1;

            bool bottomAndOddColumn = bottomRow && !evenColumn;
            bool topAndEvenColumn = topRow && evenColumn;

            bool notEdge = !topAndEvenColumn && !bottomAndOddColumn;
            
            
            Vector2 anchorPosition;

            Anchor anchor = null;
            if (!leftColumn)
            {
                if (!topAndEvenColumn && !bottomAndOddColumn)
                {
                    anchor = anchorPool.get<Anchor>();
                    anchor.transform.SetParent(anchorHolder);
                    
                    anchorPosition = slot.rect.anchoredPosition + (.5f * tileSize.x * Vector2.left);
                    
                    anchor.rect.anchoredPosition = anchorPosition;
                }
                
            }

            if (!rightColumn)
            {
                if (notEdge)
                {
                    anchor = anchorPool.get<Anchor>();
                    anchor.transform.SetParent(anchorHolder);
                    
                    anchorPosition = slot.rect.anchoredPosition + (.5f * tileSize.x * Vector2.right);
                    
                    anchor.rect.anchoredPosition = anchorPosition;
                    
                }

            }
            
        }
    }

    Tile spawnTile(int i, int j)
    {
        Tile temp = slotPool.get<Tile>();
        temp.init(this, currentConfig, i, j);
        temp.transform.SetParent(slotHolder);
        
        temp.rect.sizeDelta = tileSize;

        temp.rect.anchoredPosition = calculatePosition(i, j);
        
        return temp;
    }

    Vector2 calculatePosition(int i, int j)
    {
        Vector2 position = Vector2.zero;
        
        position = new Vector2(i * .75f, j) * tileSize;
        
        position += (i % 2) * .5f * tileSize.y * Vector2.down; // tek indexteki stunlarn pozisyonlarını asagıya itiyor
        
        
        // buradan sonrakiler board da ortalamak icin, anchor larını  merkezde tutmak istedim o yuzden elle ayarladım 
        // yatay ortalamak
        position += tileSize.x *  boardOffet.x * .25f * Vector2.left; 
        position += tileSize.x * .5f * Vector2.right;
        
        
        position += tileSize.y *  boardOffet.y * .5f * Vector2.down; 
        position += tileSize.y * Vector2.up;
        
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


        tileSize = new Vector2(possibleMaximumWidth, possibleMaximumWidth / TileSizeRatio);
    }

    float calculateMaxWidth()
    {
        //altıgenin bir kenarı 1 birim olarak alırsak

        float doubleColumnWidth = 3; //her 2 stunun ekledigi genislik

        float boardWidth = Mathf.Floor((float) currentConfig.boardWidth / 2) * doubleColumnWidth;
        boardWidth += .5f; // sonuncu cıkıntısı
        boardWidth += (currentConfig.boardWidth % 2) * 1.5f; // tek sayıda stun varsa en sona eklenecek uzunluk;

        boardOffet.x = boardWidth;

        return 2 * slotHolder.rect.width / boardWidth; // x2 cunku bir kenar 1 birim uzunlugunda olursa altıgeni cevreleyen imajın genisiligi 2 olur
    }

    private float CalculateMaxWidthForHeight()
    {
        //oyunun oynanabilmesi icin en az iki stun gerekir, 1 stun olma durumunu hesaplamadım o yuzden


        float boardHeight = currentConfig.boardHeight + .5f; // .5 sonuncunun cıkıntısı
        
        boardOffet.y = boardHeight;
        
        float tileHeight = slotHolder.rect.height / boardHeight;
        float tileWidth = tileHeight * TileSizeRatio;
        

        return tileWidth;
    }


}
