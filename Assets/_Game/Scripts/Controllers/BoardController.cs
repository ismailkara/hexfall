using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance;

    private const float TileSizeRatio = 1.1547005383792515290182975610039149112952035025402537520372046529f; // duzgun altıgenin boy/en oranı, 2/√3
    
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private RectTransform boardRect;
    
    
    
    private Tile[,] board;
    private ObjectPool pool;
    private Vector2 tileSize;
    

    private GameConfig currentConfig;
    private void Awake()
    {
        Instance = this;
        init();
    }

    void init()
    {
        pool = new ObjectPool(tilePrefab);
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
    }

    #endregion

    void buildBoard()
    {
        board = new Tile[currentConfig.boardWidth, currentConfig.boardHeight];

        for (int i = 0; i < currentConfig.boardWidth; i++)
        {
            for (int j = 0; j < currentConfig.boardHeight; j++)
            {
                spawnTile(i, j);
            }
        }
    }

    void spawnTile(int i, int j)
    {
        Tile temp = pool.get<Tile>();
        temp.transform.SetParent(boardRect);
        
        temp.rect.sizeDelta = tileSize;
        temp.rect.anchoredPosition = new Vector2(i * .75f, j) * tileSize;
        
        temp.rect.anchoredPosition += (i % 2) * .5f * tileSize.y * Vector2.down; // tek indexteki stunlarn pozisyonlarını asagıya itiyor
        
        
        // buradan sonrakiler board da ortalamak icin, anchor larını  merkezde tutmak istedim o yuzden elle ayarladım 
        temp.rect.anchoredPosition += boardRect.rect.width * .5f * Vector2.left; // yatay ortalamak
        temp.rect.anchoredPosition += tileSize.x * .5f * Vector2.right;


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
        boardWidth += (currentConfig.boardWidth % 2) * .75f; // tek sayıda stun varsa en sona eklenecek uzunluk;

        return 2 * boardRect.rect.width / boardWidth; // x2 cunku bir kenar 1 birim uzunlugunda olursa altıgeni cevreleyen imajın genisiligi 2 olur
    }

    private float CalculateMaxWidthForHeight()
    {
        //oyunun oynanabilmesi icin en az iki stun gerekir, 1 stun olma durumunu hesaplamadım o yuzden


        float boardHeight = currentConfig.boardHeight + .5f; // sonuncunun cıkıntısı
        float tileHeight = boardRect.rect.height / boardHeight;
        float tileWidth = tileHeight * TileSizeRatio;

        return tileWidth;
    }


}
