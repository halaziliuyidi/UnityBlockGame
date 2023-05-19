using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static LevelEditor;

[InitializeOnLoad]
public class LevelEditor : EditorWindow
{
    [Serializable]
    public class LevelConfig
    {
        public LevelSetting LevelSetting = new LevelSetting();
        public LevelItem[] LevelItems = new LevelItem[64];

        public LevelConfig(LevelSetting levelSetting, LevelItem[] levelItems)
        {
            LevelSetting = levelSetting;
            LevelItems = levelItems;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static LevelConfig FromJson(string json)
        {
            return JsonUtility.FromJson<LevelConfig>(json);
        }
    }

    [Serializable]
    public class LevelSetting
    {
        //判断是不是有星星还是纯色得分的关卡
        public bool isContainStar = false;

        public string LevelName = "level_";

        public string targetScore = "0";

        public int[] startTargetNum = new int[5] { 0, 0, 0, 0, 0 };

        public int start1Num = 0;
        public int start2Num = 0;
        public int start3Num = 0;
        public int start4Num = 0;
        public int start5Num = 0;
    }

    [Serializable]
    public class LevelItem
    {
        //格子是否占用
        public bool isOccupy = false;
        //是否包含星星
        public bool isContainStar = false;
        //星星类型，共五种
        public int starIndex = 0;
    }

    [Serializable]
    public class LevelPrefab
    {
        public string path;
        public string name;
    }


    private const int BUTTON_SIZE = 100;
    private const int GRID_SIZE = 8;
    [SerializeField]
    public static LevelItem[,] levelItems = new LevelItem[,] { };
    [SerializeField]
    public static LevelSetting levelSetting = new LevelSetting();

    public static LevelPrefab[] levelPrefabs = new LevelPrefab[] { };

    public static LevelPrefab nowLevelPrefab = new LevelPrefab();

    private static GUIStyle itemBtnStyle;

    private static GUIStyle labelStyle;

    private static GUIStyle itemBoxStyle;

    private static GUIStyle itemTitleStyle;

    private static GUIStyle titleLableStyle;

    private static Texture2D backgroundTexture = null;

    private static Texture2D titleBoxTexture = null;

    private static Texture2D toggleBoxTexture = null;

    private static string dimandNum;

    private static string BlockTilePrefabPath = "Assets/Prefab/Block tile.prefab";

    private static string blockImagePath = "Assets/Sprites/BlocksImage/blockbg3.png";

    private static string clearBlockImagePath = "Assets/Sprites/ClearBlocksImage/blockbg3.png";

    private static string starImagePath = "Assets/Sprites/Star/";

    private static string rootPath = "Assets/Prefab/LevelPrefab";

    private static Sprite blockSprite;

    private static Sprite clearblockSprite;

    private static Sprite[] startSprites;

    private static GameObject BlockTilePrefab;

    private static GameObject NowLeveObject;

    private static int levelIndex;

    string[] startNums = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                        "20", "21", "22", "23", "24","25", "26", "27", "28", "29", "30" };


    [MenuItem("关卡编辑器/关卡编辑器")]
    public static void ShowWindow()
    {
        InitLevelItem();
        EditorWindow.GetWindow<LevelEditor>("关卡编辑器");

    }

    private void OnEnable()
    {
        InitLevelItem();
    }


    private static void InitLevelItem()
    {
        if (NowLeveObject != null)
        {
            DestroyImmediate(NowLeveObject);
        }
        levelSetting = new LevelSetting();
        levelItems = new LevelItem[GRID_SIZE, GRID_SIZE];
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                levelItems[i, j] = new LevelItem();
            }
        }

        levelPrefabs = new LevelPrefab[] { };
        levelPrefabs = GetLevelPrefabs();
        backgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Resources/btn.png");
        titleBoxTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Resources/bg.png");
        toggleBoxTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Resources/ToggleBg.png");
        BlockTilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BlockTilePrefabPath);
        blockSprite = AssetDatabase.LoadAssetAtPath<Sprite>(blockImagePath);
        clearblockSprite = AssetDatabase.LoadAssetAtPath<Sprite>(clearBlockImagePath);
        startSprites = new Sprite[5];
        for (int i = 0; i < startSprites.Length; i++)
        {
            startSprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(starImagePath + "star" + (i + 1) + ".png");
        }
    }

    private void OnGUI()
    {

        itemTitleStyle = new GUIStyle(GUI.skin.box);

        //itemTitleStyle.normal.background = titleBoxTexture;

        itemBtnStyle = new GUIStyle(GUI.skin.button);


        labelStyle = new GUIStyle(GUI.skin.label);

        itemBoxStyle = new GUIStyle(GUI.skin.box);
        itemBoxStyle.normal.background = backgroundTexture;

        titleLableStyle = new GUIStyle(GUI.skin.label);
        titleLableStyle.normal.textColor = Color.white;

        GUILayout.MaxWidth(900f);
        GUILayout.MaxHeight(900f);
        EditorGUILayout.BeginVertical(itemTitleStyle);
        GUILayout.BeginHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUIStyle toggleBoxStyle = new GUIStyle(GUI.skin.box);
        toggleBoxStyle.normal.background = toggleBoxTexture;
        EditorGUILayout.BeginVertical(toggleBoxStyle);
        GUILayout.BeginHorizontal();
        bool isCheck = EditorGUILayout.Toggle("本关卡是否是带钻关卡", levelSetting.isContainStar, new GUILayoutOption[] { GUILayout.MaxWidth(30) });
        levelSetting.isContainStar = isCheck;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        EditorGUILayout.LabelField("请在输入框中输入关卡名称", titleLableStyle, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
        string inputText = GUILayout.TextField(levelSetting.LevelName, GUILayout.MaxWidth(100));
        levelSetting.LevelName = inputText;
        EditorGUILayout.LabelField("当前关卡名称为:" + levelSetting.LevelName, titleLableStyle, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
        EditorGUILayout.BeginVertical(toggleBoxStyle);
        if (levelSetting.isContainStar)
        {
            levelSetting.targetScore = "0";
            //计算钻石数
            GUILayout.Label("当前关卡钻石数分别为:\n" + dimandNum);
            for (int i = 0; i < 5; i++)
            {
                GUILayout.Label(string.Format("钻石{0}目标数量{1}:\n", i + 1, levelSetting.startTargetNum[i]));
                int num = EditorGUILayout.Popup(levelSetting.startTargetNum[i], startNums);
                levelSetting.startTargetNum[i] = int.Parse(startNums[num]);
            }
        }
        else
        {
            EditorGUILayout.LabelField("请在输入框中输入目标分数", new GUILayoutOption[] { GUILayout.MaxWidth(200) });
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.normal.textColor = Color.red;
            textFieldStyle.fontSize = 30;
            textFieldStyle.normal.background = titleBoxTexture;
            string score = GUILayout.TextField(levelSetting.targetScore, textFieldStyle, GUILayout.MaxWidth(200), GUILayout.MaxHeight(50));
            levelSetting.targetScore = score;
            GUILayout.Space(10);
            GUILayout.Label("目标分数为:\n" + levelSetting.targetScore);
        }
        GUILayout.Box(titleBoxTexture, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(10), GUILayout.MaxWidth(200), GUILayout.MaxHeight(10) });
        if (GUILayout.Button("保存", GUILayout.Width(200), GUILayout.Height(50)))
        {
            ClearNowLevelPrefab();
            //保存关卡配置文件
            SaveLevelConfig();
        }
        if (GUILayout.Button("重置", GUILayout.Width(200), GUILayout.Height(50)))
        {
            string levelName = levelSetting.LevelName;
            levelSetting = new LevelSetting();
            levelSetting.LevelName = levelName;
            levelItems = new LevelItem[GRID_SIZE, GRID_SIZE];
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    levelItems[i, j] = new LevelItem();
                }
            }
        }
        if (GUILayout.Button("删除", GUILayout.Width(200), GUILayout.Height(50)))
        {
            //删除所有配置
            ClearNowLevelPrefab();
            DeleteLevelPrefabAndConfig();
            //清空关卡配置文件
            InitLevelItem();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(toggleBoxStyle);
        GUILayout.Label("当前关卡总数为:" + levelPrefabs.Length);
        GUILayout.Label("关卡列表");
        string[] names = new string[levelPrefabs.Length];
        for (int i = 0; i < levelPrefabs.Length; i++)
        {
            names[i] = levelPrefabs[i].name;
        }
        int inde = EditorGUILayout.Popup(levelIndex, names);
        levelIndex = inde;
        if (GUILayout.Button("加载", GUILayout.Width(200), GUILayout.Height(50)))
        {
            if (names.Length > 0)
            {
                InitLevelItem();
                LevelConfig level = LoadConfig(names[levelIndex].Replace(".prefab", ""));
                levelItems = SetItemsToArray2(level.LevelItems);

                levelSetting = level.LevelSetting;

                //levelItems = level.LevelItemsArray2;
            }
        }
        if (GUILayout.Button("修复所有关卡", GUILayout.Width(200), GUILayout.Height(50)))
        {
            for (int i = 0; i < names.Length; i++)
            {
                InitLevelItem();
                LevelConfig level = LoadConfig(names[i].Replace(".prefab", ""));
                levelItems = SetItemsToArray2(level.LevelItems);

                levelSetting = level.LevelSetting;

                ClearNowLevelPrefab();
                //保存关卡配置文件
                SaveLevelConfig();
            }
        }
        EditorGUILayout.EndVertical();


        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        // 设置每行的最大宽度和高度
        GUILayoutOption[] buttonLayoutOptions = { GUILayout.MaxWidth(BUTTON_SIZE), GUILayout.MaxHeight(BUTTON_SIZE) };
        EditorGUILayout.BeginVertical(itemTitleStyle);
        for (int y = GRID_SIZE - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int x = 0; x < GRID_SIZE; x++)
            {
                Color boxColor = levelItems[x, y].isOccupy ? Color.red : Color.white; // 根据isOccupy的值确定按钮的颜色
                GUI.backgroundColor = boxColor; // 设置按钮背景色

                string isOcccupyStr = levelItems[x, y].isOccupy ? "点击取消" : "点击添加";
                itemBtnStyle.normal.textColor = levelItems[x, y].isOccupy ? Color.yellow : Color.white;

                GUILayout.BeginVertical(itemBoxStyle, GUILayout.MaxWidth(BUTTON_SIZE), GUILayout.MaxHeight(BUTTON_SIZE));
                // 创建可调整大小的按钮
                if (GUILayout.Button(x + "-" + y + "\n" + isOcccupyStr, itemBtnStyle, buttonLayoutOptions))
                {
                    //Debug.Log($"Button ({x}, {y}) clicked!");
                    levelItems[x, y].isOccupy = !levelItems[x, y].isOccupy;
                    if (!levelItems[x, y].isOccupy)
                    {
                        levelItems[x, y].isContainStar = true;
                        levelItems[x, y].starIndex = 0;
                        dimandNum = GetNum();
                    }
                }
                GUI.backgroundColor = Color.black; // 恢复默认背景色
                if (levelSetting.isContainStar)
                {
                    string[] options = new string[] { "无钻石", "钻石1", "钻石2", "钻石3", "钻石4", "钻石5" };
                    int selectedIndex = EditorGUILayout.Popup(levelItems[x, y].starIndex, options);
                    string textStr = "无钻石";
                    if (selectedIndex != 0)
                    {
                        levelItems[x, y].isContainStar = true;
                        levelItems[x, y].starIndex = selectedIndex;
                        textStr = "类型为:\n" + options[selectedIndex];
                        labelStyle.normal.textColor = Color.yellow;
                        levelItems[x, y].isOccupy = true;
                        dimandNum = GetNum();
                    }
                    else
                    {
                        levelItems[x, y].isContainStar = false;
                        levelItems[x, y].starIndex = selectedIndex;
                        labelStyle.normal.textColor = Color.black;
                        textStr = "无钻石\n";
                        dimandNum = GetNum();
                    }
                    GUILayout.Label(textStr, labelStyle);
                }
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    public void SaveLevelConfig()
    {
        BuildLevel();
        LevelConfig levelConfig = new LevelConfig(levelSetting, SetItemsToArray(levelItems));
        string jsonStr = JsonUtility.ToJson(levelConfig);
        SaveConfig(jsonStr);
        levelPrefabs = GetLevelPrefabs();
    }

    public void SaveConfig(string saveMessage)
    {
        string filePath = Application.dataPath + "/Editor/LevelConfig/" + levelSetting.LevelName + ".txt";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        // 创建文件
        FileStream fs = File.Create(filePath);
        fs.Close(); // 关闭文件流
        StreamWriter writer = new StreamWriter(filePath, false); // 打开文件流
        writer.WriteLine(saveMessage); // 写入内容
        writer.Close(); // 关闭文件流
        Debug.Log("内容已写入文件 " + filePath);
        AssetDatabase.Refresh();
    }

    public LevelConfig LoadConfig(string name)
    {
        LevelConfig levelConfig = new LevelConfig(new LevelSetting(), new LevelItem[] { });
        string filePath = Application.dataPath + "/Editor/LevelConfig/" + name + ".txt";
        StreamReader reader = new StreamReader(filePath);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            //Debug.Log(line); // 输出读取到的内容
            levelConfig = JsonUtility.FromJson<LevelConfig>(line);
        }

        reader.Close();
        return levelConfig;
    }

    public void DeleteConfig(string name)
    {
        string filePath = "Assets/Editor/LevelConfig/" + name + ".txt";
        if (File.Exists(filePath))
        {
            AssetDatabase.DeleteAsset(filePath);
        }
        AssetDatabase.Refresh();
    }

    public void BuildLevel()
    {
        GameObject block = new GameObject(levelSetting.LevelName);
        block.transform.position = new Vector3(3.5f, 3.5f, 0);
        block.AddComponent<BoxCollider>().size = new Vector3(5.55f, 5.55f, 1f);
        Block blockCompent = block.AddComponent<Block>();
        blockCompent.size = Vector2.one * 8;
        Level level = block.AddComponent<Level>();
        level.isStarLevel = levelSetting.isContainStar;
        level.startTargetNum = levelSetting.startTargetNum;
        level.start1Num = levelSetting.start1Num;
        level.start2Num = levelSetting.start2Num;
        level.start3Num = levelSetting.start3Num;
        level.start4Num = levelSetting.start4Num;
        level.start5Num = levelSetting.start5Num;
        level.targetScore = int.Parse(levelSetting.targetScore);
        if (!levelItems[0, 0].isOccupy)
        {
            GameObject Fake_block_tile = new GameObject(levelSetting.LevelName);
            Fake_block_tile.transform.position = Vector3.zero;
            Fake_block_tile.transform.parent = block.transform;
            Fake_block_tile.name = "Fake block tile";
        }
        blockCompent.structure = GetBlockStructure();

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (levelItems[x, y].isOccupy)
                {
                    GameObject blockTitleObj = Instantiate(BlockTilePrefab, new Vector3(x, y, 0), new Quaternion());
                    blockTitleObj.transform.name = "Block tile";
                    blockTitleObj.transform.localScale = Vector3.one;

                    blockTitleObj.transform.parent = block.transform;
                    SpriteRenderer sp = blockTitleObj.GetComponent<SpriteRenderer>();
                    sp.sortingOrder = 5;
                    if (levelSetting.isContainStar)
                    {
                        sp.sprite = clearblockSprite;
                        if (levelItems[x, y].starIndex != 0)
                        {
                            Transform star = blockTitleObj.transform.Find("star");
                            star.GetComponent<SpriteRenderer>().sprite = startSprites[levelItems[x, y].starIndex - 1];
                            blockTitleObj.GetComponent<BlockTile>().isStar = true;
                            blockTitleObj.GetComponent<BlockTile>().starIndex = levelItems[x, y].starIndex - 1;
                            star.GetComponent<SpriteRenderer>().sortingOrder = 6;
                            star.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        sp.sprite = blockSprite;
                    }
                }
            }
        }
        block.tag = "Block";
        NowLeveObject = block;

        SaveLevelPrefab(NowLeveObject);
    }


    public string GetNum()
    {
        string numState = "";
        int noStar = 0;
        int star1 = 0, star2 = 0, star3 = 0, star4 = 0, star5 = 0;
        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (levelItems[y, x].isContainStar && levelItems[y, x].isOccupy)
                {
                    switch (levelItems[y, x].starIndex)
                    {
                        case 1:
                            star1 += 1;
                            levelSetting.start1Num = star1;
                            break;
                        case 2:
                            star2 += 1;

                            break;
                        case 3:
                            star3 += 1;

                            break;
                        case 4:
                            star4 += 1;

                            break;
                        case 5:
                            star5 += 1;

                            break;
                        default:
                            noStar += 1;
                            break;
                    }
                }
            }
        }
        if (noStar == levelItems.Length)
        {
            return "当前关卡暂无钻石";
        }
        numState = string.Format("start1：{0}个\nstart2：{1}个\nstart3：{2}个\nstart4：{3}个\nstart5：{4}个", star1, star2, star3, star4, star5);
        levelSetting.start1Num = star1;
        levelSetting.start2Num = star2;
        levelSetting.start3Num = star3;
        levelSetting.start4Num = star4;
        levelSetting.start5Num = star5;
        return numState;
    }

    public int GetBlockStructureLength()
    {
        int num = 0;
        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (levelItems[x, y].isOccupy)
                {
                    num++;
                }
            }
        }
        return num;
    }

    public Vector2Int[] GetBlockStructure()
    {
        int structureLength = levelItems[0, 0].isOccupy ? GetBlockStructureLength() : GetBlockStructureLength() + 1;
        Vector2Int[] st = new Vector2Int[structureLength];
        int num = 0;
        if (!levelItems[0, 0].isOccupy)
        {
            num += 1;
        }
        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (levelItems[x, y].isOccupy)
                {
                    st[num] = new Vector2Int(x, y);
                    num++;
                }
            }
        }
        return st;
    }

    public void SaveLevelPrefab(GameObject newprefab)
    {
        string savePath = rootPath + "/" + levelSetting.LevelName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(newprefab, savePath);
    }

    public void ClearNowLevelPrefab()
    {
        if (NowLeveObject != null)
        {
            DestroyImmediate(NowLeveObject);
        }
        if (GameObject.Find(levelSetting.LevelName))
        {
            DestroyImmediate(GameObject.Find(levelSetting.LevelName));
        }

    }

    public void DeleteLevelPrefabAndConfig()
    {
        string savePath = rootPath + "/" + levelSetting.LevelName + ".prefab";
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(savePath);
        if (obj != null)
        {
            if (AssetDatabase.Contains(obj))
                AssetDatabase.DeleteAsset(savePath);
        }
        DeleteConfig(levelSetting.LevelName);


        AssetDatabase.Refresh();
        levelPrefabs = GetLevelPrefabs();
    }

    public static LevelPrefab[] GetLevelPrefabs()
    {
        string[] resFiles = AssetDatabase.FindAssets("t:Prefab"/*"t:" + extension*/, new string[] { rootPath });
        LevelPrefab[] levels = new LevelPrefab[resFiles.Length];
        for (int i = 0; i < resFiles.Length; i++)
        {
            levels[i] = new LevelPrefab();
            levels[i].path = resFiles[i];
            levels[i].name = AssetDatabase.GUIDToAssetPath(resFiles[i]).Replace(rootPath + "/", "");
        }
        return levels;
    }

    public LevelItem[] SetItemsToArray(LevelItem[,] array2)
    {
        LevelItem[] levelItems = new LevelItem[array2.GetLength(0) * array2.GetLength(1)];
        for (int i = 0; i < array2.GetLength(0); i++)
        {
            for (int j = 0; j < array2.GetLength(1); j++)
            {
                levelItems[i * array2.GetLength(0) + j] = array2[i, j];
            }
        }
        return levelItems;
    }

    public LevelItem[,] SetItemsToArray2(LevelItem[] array)
    {
        LevelItem[,] levelItems = new LevelItem[GRID_SIZE, GRID_SIZE];
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                levelItems[i, j] = array[i * GRID_SIZE + j];
            }
        }
        return levelItems;
    }
}
