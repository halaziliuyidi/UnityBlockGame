using System;
using UnityEngine;
using AnnulusGames.LucidTools.RandomKit;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Reflection;

public class BoardManager : MonoBehaviour
{
    private static BoardManager instance;

    public static BoardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(BoardManager)) as BoardManager;
            }
            return instance;
        }
    }

    [HideInInspector]
    public Vector3 boardTileScale;
    [HideInInspector]
    public Vector3 scaledBlockTileScale;

    private Transform boardTrn;

    private Transform gameTrn;

    private Transform cameraTrn;

    private Transform backgroundTrn;

    public GameObject boardTilePrefab;

    public GameObject blockTilePrefab;

    public WeightedList<GameObject> blockPrefabs = new WeightedList<GameObject>();

    public GameObject[] levelPrefabs = new GameObject[] { };

    public List<GameObject> firstGameBlocksPrefab = new List<GameObject>();

    public List<GameObject> firstGameObject = new List<GameObject>();

    public List<Vector3> firstGameObjecPos = new List<Vector3>();

    public List<Transform> fistGameDragRanges = new List<Transform>();

    public List<GameObject> teachObjects = new List<GameObject>();

    //[HideInInspector]
    public BoardTile[,] boardTiles = new BoardTile[GameConstManager.Board_Size, GameConstManager.Board_Size];   //棋盘物体
    //[HideInInspector]
    public Block[] blocks = new Block[GameConstManager.Board_Amount];                                           //当前生成的三个可以拖拽的方块
    //[HideInInspector]
    public BlockTile[,] boardBlocks = new BlockTile[GameConstManager.Board_Size, GameConstManager.Board_Size];  //实际block titles

    [SerializeField]
    private List<Block> dragedBlocks = new List<Block>();                                                       //已经放置好的方块，最大长度为3
    [SerializeField]
    private List<Block> residueBlocks = new List<Block>();
    [SerializeField]
    private float[] weights;

    private int dragedBlocksNum = 0;
    [HideInInspector]
    public int firstGameIndex = 0;

    public Color boardColor;

    public Color highlightColor;

    public int testIndex;

    public Sprite DarkTitleBg;

    public Sprite BrightTitleBg;

    public void StartGame()
    {
        gameTrn.gameObject.SetActive(true);
        backgroundTrn.gameObject.SetActive(true);
        for (int i = 0; i < blocks.Length; i++)
        {
            GameManager.Instance.fxController.SpawnCreateBlockFX(blocks[i].transform.position);
        }
    }

    public void Initialized()
    {
        gameTrn = GameObject.Find("Game").transform;
        boardTrn = gameTrn.Find("Board");
        cameraTrn = Camera.main.transform;
        backgroundTrn = GameObject.Find("Background").transform;
        boardTileScale = GameScaler.GetBoardTileScale();
        scaledBlockTileScale = GameScaler.GetScaledBlockTileScale();
        backgroundTrn.gameObject.SetActive(false);
        gameTrn.gameObject.SetActive(false);
        weights = new float[blockPrefabs.Count];
        for (int i = 0; i < blockPrefabs.Count; i++)
        {
            weights[i] = blockPrefabs[i].weight;
        }
        CreatBoard();
        SetCameraAndBgPos();
        SpawnBlockTitles();

    }

    public void BoardBlockInit(bool isLevelMode = false, int levelIndex = 0)
    {
        if (isLevelMode)
        {
            SpawnLevelPrefab(levelIndex);
            SpawnBlocks();
        }
        else
        {
            if (GameDataManager.Instance.firstGame)
            {
                for (int i = 0; i < 3; i++)
                {
                    Debug.Log(string.Format("Range{0}Point", i + 1));
                    fistGameDragRanges.Add(GameObject.Find(string.Format("Range{0}Point", i + 1)).transform);
                    teachObjects.Add(GameObject.Find("Teach").transform.GetChild(i).gameObject);
                }
                SpawnFirstGameBlocks();
            }
            else
            {
                SpawnBlocks();
            }
        }
    }

    private void SpawnLevelPrefab(int levelIndex = 0)
    {
        Block levelBlock = Instantiate(levelPrefabs[levelIndex], gameTrn).GetComponent<Block>();
        GameManager.Instance.nowLevel = levelBlock.GetComponent<Level>();
        GameManager.Instance.nowLevel.sceneStarsNum = new int[5];
        levelBlock.Init();
        levelBlock.transform.localScale = Vector3.one;
        levelBlock.transform.position = new Vector3(3.5f, 3.5f, 0);
        Vector2 origin = levelBlock.transform.GetChild(0).position;

        Vector2Int start = RoundVector2(origin);

        for (int i = 0; i < levelBlock.structure.Length; i++)
        {
            Vector2Int coords = levelBlock.structure[i];

            if (levelBlock.transform.GetChild(i).name == "Block tile")
            {
                BlockTile bt = levelBlock.transform.GetChild(i).GetComponent<BlockTile>();
                boardBlocks[start.x + coords.x, start.y + coords.y] = bt;
            }
        }
        levelBlock.GetComponent<BoxCollider>().enabled = false;
        levelBlock.enabled = false;
    }

    private void SpawnFirstGameBlocks()
    {
        SpawnFirstGameBlock(firstGameIndex);
        SpawnFirstGamePlacedBlock(firstGameIndex);
    }

    private void SpawnFirstGamePlacedBlock(int index)
    {
        Block b = Instantiate(firstGameObject[index], gameTrn).GetComponent<Block>();
        b.Init(true);
        b.transform.localScale = Vector3.one;
        b.transform.position = firstGameObjecPos[index];

        Vector2 origin = b.transform.GetChild(0).position;

        Vector2Int start = RoundVector2(origin);

        for (int i = 0; i < b.structure.Length; i++)
        {
            Vector2Int coords = b.structure[i];

            if (b.transform.GetChild(i).name == "Block tile")
            {
                BlockTile bt = b.transform.GetChild(i).GetComponent<BlockTile>();
                boardBlocks[start.x + coords.x, start.y + coords.y] = bt;
            }
        }
        b.GetComponent<BoxCollider>().enabled = false;
        b.enabled = false;
    }

    private Vector2Int RoundVector2(Vector2 v)
    {
        return new Vector2Int((int)(v.x + 0.5f), (int)(v.y + 0.5f));
    }

    public void ResetBoard()
    {
        ResetWeights();
        dragedBlocks.Clear();

        foreach (Transform item in gameTrn)
        {
            if (item.name != "Board")
            {
                Destroy(item.gameObject);
            }
        }
        dragedBlocksNum = 0;
        gameTrn.gameObject.SetActive(false);
        backgroundTrn.gameObject.SetActive(true);
    }

    public void ReviveBoard()
    {
        dragedBlocksNum = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!dragedBlocks.Contains(blocks[i]))
            {
                blocks[i].gameObject.SetActive(false);
                Destroy(blocks[i].gameObject, 0.1f);
            }
        }
        dragedBlocks.Clear();
        SpawnBlocks();
        CheckSpace();
    }

    private void CreatBoard()
    {
        Vector3 scale = GameScaler.GetBoardTileScale();
        for (int i = 0; i < GameConstManager.Board_Size; i++)
        {
            for (int j = 0; j < GameConstManager.Board_Size; j++)
            {
                SpawnBoardTiles(i, j, scale);
            }
        }
    }

    private void SpawnBoardTiles(int x, int y, Vector3 scale)
    {
        Transform Tileitem = Instantiate(boardTilePrefab, boardTrn).transform;
        Tileitem.position = new Vector3(x, y, 0);
        boardTiles[x, y] = Tileitem.GetComponent<BoardTile>();
        boardTiles[x, y].gameObject.name = x + "," + y;
        boardTiles[x, y].Init();
        if (x % 2 == 0)
        {
            if (y % 2 == 0)
            {
                boardTiles[x, y].SetDefaultSprite(DarkTitleBg);
            }
            else
            {
                boardTiles[x, y].SetDefaultSprite(BrightTitleBg);
            }
        }
        else
        {
            if (y % 2 == 1)
            {
                boardTiles[x, y].SetDefaultSprite(DarkTitleBg);
            }
            else
            {
                boardTiles[x, y].SetDefaultSprite(BrightTitleBg);
            }
        }
    }

    private void SpawnBlocks()
    {
        blocks = new Block[GameConstManager.Board_Amount];
        List<int> spriteIndex = GameConstManager.Instance.GetBlockSpritesIndex();
        if (GameManager.Instance.TestMode)
        {
            for (int i = 0; i < GameConstManager.Board_Amount; i++)
            {
                blocks[i] = SpawnBlock(i, blockPrefabs[testIndex].value, spriteIndex[i]);
            }
            return;
        }

        List<GameObject> list = blockPrefabs.RandomDisticncElements(GameConstManager.Board_Amount);

        for (int i = 0; i < GameConstManager.Board_Amount; i++)
        {
            blocks[i] = SpawnBlock(i, list[i], spriteIndex[i]);
        }
    }

    public void SpawnFirstGameBlock(int index)
    {
        blocks = new Block[1];
        Block b = Instantiate(firstGameBlocksPrefab[index], gameTrn).GetComponent<Block>();
        b.Init();
        b.SetBasePosition(1);
        GameManager.Instance.fxController.SpawnCreateBlockFX(this.transform.position);
        blocks[0] = b;
        if (index > 0)
        {
            teachObjects[index - 1].SetActive(false);
        }
        teachObjects[index].SetActive(true);
    }

    private Block SpawnBlock(int i, GameObject obj, int spriteIndex)
    {
        Block b = Instantiate(obj, gameTrn).GetComponent<Block>();
        b.Init(false, spriteIndex);
        b.SetBasePosition(i);
        GameManager.Instance.fxController.SpawnCreateBlockFX(b.transform.position);
        return b;
    }


    public BlockTile SpawnBlockTile(int x, int y)
    {
        if (boardBlocks[x, y] != null)
        {
            Destroy(boardBlocks[x, y].gameObject);
        }
        boardBlocks[x, y] = Instantiate(blockTilePrefab, boardTrn).GetComponent<BlockTile>();
        boardBlocks[x, y].GetSpriteRender();
        Vector3 pos = new Vector3(x, y, -1);
        boardBlocks[x, y].transform.position = pos;
        boardBlocks[x, y].transform.localScale = boardTileScale;
        return boardBlocks[x, y];
    }

    private void SpawnBlockTitles()
    {
        for (int y = 0; y < GameConstManager.Board_Size; y++)
        {
            for (int x = 0; x < GameConstManager.Board_Size; x++)
            {
                Color color = GetColor(GetBlockKey(x, y));

                if (color != Color.black)
                {
                    SpawnBlockTile(x, y);
                }
            }
        }
    }

    //修改游戏难度
    public void ChangedGameDifficulty(int level)
    {
        for (int i = 0; i < blockPrefabs.Count; i++)
        {
            Block block = blockPrefabs[i].value.GetComponent<Block>();
            if (block.structure.Length >= 5)
            {
                blockPrefabs[i].weight += level * 0.2f;
            }
        }
    }

    private void SetCameraAndBgPos()
    {
        float XY = GameConstManager.TileScale * (GameConstManager.Board_Size / 2 - 1) + GameConstManager.TileScale / 2f;
        Vector3 cameraPos = new Vector3(XY, XY, -10f);
        Vector3 bgPos = new Vector3(XY, XY, 1);
        cameraTrn.position = cameraPos;
        backgroundTrn.position = bgPos;
    }

    public bool IsInFirstGameRange(Vector2 o)
    {
        teachObjects[firstGameIndex].SetActive(false);
        return o.x >= fistGameDragRanges[firstGameIndex].position.x - 0.5f && o.x <= fistGameDragRanges[firstGameIndex].position.x + 0.5f &&
               o.y >= fistGameDragRanges[firstGameIndex].position.y - 0.5f && o.y <= fistGameDragRanges[firstGameIndex].position.y + 0.5f;
    }

    public bool IsInRange(Vector2 o, Vector2 e)
    {
        return o.x >= -0.5f && e.x <= GameConstManager.Board_Size - 0.5f &&
               o.y >= -0.5f && e.y <= GameConstManager.Board_Size - 0.5f;
    }

    public bool IsEmpty(Block b, Vector2 o)
    {
        for (int i = 0; i < b.transform.childCount; i++)
        {
            Transform child = b.transform.GetChild(i);
            if (child != null)
            {
                if (child.name == "Block tile")
                {
                    Vector2Int coords = b.structure[i];

                    if (boardBlocks[(int)o.x + coords.x, (int)o.y + coords.y])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void HighlightBlocks()
    {
        Block db = InputManager.Instance.draggedBlock;
        Vector2Int c = db.GetFirstCoords();

        for (int x = c.x; x < c.x + db.size.x; x++)
            CheckVLine(x, true);
        for (int y = c.y; y < c.y + db.size.y; y++)
            CheckHLine(y, true);
    }

    private void CheckVLine(int x, bool h = false)
    {
        if (h)
        {
            BlockTile[,] b = new BlockTile[GameConstManager.Board_Size, GameConstManager.Board_Size];
            Array.Copy(boardBlocks, b, boardBlocks.Length);

            Block db = InputManager.Instance.draggedBlock;
            Vector2Int c = db.GetFirstCoords();
            for (int i = 0; i < db.structure.Length; i++)
            {
                if (db.transform.GetChild(i).name == "Block tile")
                {
                    BlockTile bt = db.transform.GetChild(i).GetComponent<BlockTile>();
                    b[c.x + db.structure[i].x, c.y + db.structure[i].y] = bt;
                }
            }

            for (int y = 0; y < GameConstManager.Board_Size; y++)
                if (!b[x, y])
                    return;

            for (int y = 0; y < GameConstManager.Board_Size; y++)
            {
                if (boardBlocks[x, y])
                {
                    boardBlocks[x, y].Fade(0.2f, Color.white);
                    boardBlocks[x, y].FadeSprite(db.defaultSprite);
                }
            }
        }
        else
        {
            for (int y = 0; y < GameConstManager.Board_Size; y++)
                if (!boardBlocks[x, y])
                    return;

            DestroyManager.Instance.PrepareToDestroy(x, true);
        }
    }

    private void CheckHLine(int y, bool h = false)
    {
        if (h)
        {
            BlockTile[,] b = new BlockTile[GameConstManager.Board_Size, GameConstManager.Board_Size];
            Array.Copy(boardBlocks, b, boardBlocks.Length);

            Block db = InputManager.Instance.draggedBlock;
            Vector2Int c = db.GetFirstCoords();
            for (int i = 0; i < db.structure.Length; i++)
            {
                if (db.transform.GetChild(i).name == "Block tile")
                {
                    BlockTile bt = db.transform.GetChild(i).GetComponent<BlockTile>();
                    b[c.x + db.structure[i].x, c.y + db.structure[i].y] = bt;
                }
            }

            for (int x = 0; x < GameConstManager.Board_Size; x++)
                if (!b[x, y])
                    return;

            for (int x = 0; x < GameConstManager.Board_Size; x++)
            {
                if (boardBlocks[x, y])
                {
                    //更改颜色
                    boardBlocks[x, y].Fade(0.2f, Color.white);
                    boardBlocks[x, y].FadeSprite(db.defaultSprite);
                }
            }
        }
        else
        {
            for (int x = 0; x < GameConstManager.Board_Size; x++)
                if (!boardBlocks[x, y])
                    return;
            DestroyManager.Instance.PrepareToDestroy(y, false);
        }
    }

    //方块放置之后
    public void MoveBlocks(Block block)
    {
        block.Layer(false);
        GameManager.Instance.audioController.PlayPlaceAudio();
        GameManager.Instance.AddBaseScore(block.GetBaseScore(), block.transform.GetChild(0).position);
        if (GameDataManager.Instance.firstGame)
        {
            return;
        }
        dragedBlocks.Add(block);
        dragedBlocksNum += 1;
        if (dragedBlocksNum >= GameConstManager.Board_Amount)
        {
            dragedBlocks.Clear();
            DebugHelper.Log("生成新的方块组");
            SpawnBlocks();
            dragedBlocksNum = 0;
        }
    }

    public void CheckBoard(Transform dragPos = null)
    {
        DestroyManager.Instance.SetDestroy();

        for (int x = 0; x < GameConstManager.Board_Size; x++)
        {
            CheckVLine(x);
        }
        for (int y = 0; y < GameConstManager.Board_Size; y++)
        {
            CheckHLine(y);
        }

        if (DestroyManager.Instance.destroyedLines > 0)
        {

            DebugHelper.LogFormat("删除{0}行", DestroyManager.Instance.destroyedLines);
            StartCoroutine(DestroyManager.Instance.DestroyAllBlocks());
            int comboNum = ComboManager.Instance.CheckCombo();
            GameManager.Instance.AddScore(DestroyManager.Instance.destroyedLines, comboNum, dragPos.GetChild(0).position);
            if (GameDataManager.Instance.firstGame)
            {
                firstGameIndex += 1;
                if (firstGameIndex < 3)
                {
                    SpawnFirstGameBlocks();
                }
                else
                {
                    //新手教程结束
                    DebugHelper.Log("新手教程结束");
                    teachObjects[2].SetActive(false);
                    GameDataManager.Instance.NoFirstGame();
                    EventManager.TriggerEvent(GameConstManager.teachEnd);
                    GameDataManager.Instance.firstGame = false;
                    SpawnBlocks();
                }
            }
        }

        CheckSpace();
    }

    public void CheckSpace()
    {
        if (!GameDataManager.Instance.firstGame)
        {
            //判断是否还有可用空间
            //判断游戏结束,调用ChekGameEnd()
            for (int i = 0; i < GameConstManager.Board_Amount; i++)
            {
                if (CheckBlock(i))
                {
                    blocks[i].movable = true;
                    Sprite c = blocks[i].defaultSprite;
                    blocks[i].ChangeSprite(c);

                }
                else
                {
                    blocks[i].movable = false;
                }
            }
            if (ChekGameEnd())
            {
                DebugHelper.Log("没有空位置,游戏结束");
                GameManager.Instance.GameOver();
            }
        }

    }

    public bool ChekGameEnd()
    {
        residueBlocks.Clear();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!dragedBlocks.Contains(blocks[i]))
            {
                residueBlocks.Add(blocks[i]);
            }
        }
        for (int i = 0; i < residueBlocks.Count; i++)
        {
            if (residueBlocks[i].movable)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckBlock(int i)
    {
        for (int y = 0; y < GameConstManager.Board_Size; y++)
        {
            for (int x = 0; x < GameConstManager.Board_Size; x++)
            {
                Vector2 size = new Vector2(blocks[i].size.x - 1, blocks[i].size.y - 1);
                Vector2 origin = new Vector2(x, y);
                Vector2 end = origin + size;
                if (IsInRange(origin, end) && IsEmpty(blocks[i], origin))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int GetEmptyFieldsAmount()
    {
        int x = 0;
        foreach (BlockTile b in boardBlocks)
        {
            if (!b)
            {
                x++;
            }
        }
        return x;
    }

    public void ResetWeights()
    {
        for (int i = 0; i < blockPrefabs.Count; i++)
        {
            blockPrefabs[i].weight = weights[i];
        }
    }

    private static string GetBlockKey(int x, int y)
    {
        return "block[" + x + ", " + y;
    }

    private static Color GetColor(string k)
    {
        float[] c = GetFloatArray(k, 4);

        if (c == null)
            return Color.black;

        return new Color(c[0], c[1], c[2], c[3]);
    }

    private static float[] GetFloatArray(string k, int s)
    {
        float[] arr = new float[s];

        if (!PlayerPrefs.HasKey(k))
        {
            //DebugHelper.LogError("The float array does not exist!");
            return null;
        }

        for (int i = 0; i < s; i++)
            arr[i] = PlayerPrefs.GetFloat(i + k);

        return arr;
    }

    public static int Rand(int min, int max)
    {
        return (int)UnityEngine.Random.Range(min, max - 0.000001f);
    }

}
