using AnnulusGames.LucidTools.RandomKit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameConstManager : MonoBehaviour
{
    private static GameConstManager instance;

    public static GameConstManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GameConstManager)) as GameConstManager;
            }
            return instance;
        }
    }

    public const int Board_Size = 8;

    public const int Board_Amount = 3;

    public const int Board_Prefabs_Amount = 5;

    public const float TileScale = 1f;

    public const string addBaseScore = "ADDBASESCORE";

    public const string addScoreEvent = "ADDSCORE";

    public const string maxScoreEvent = "MAXSCORE";

    public const string comboEvent = "COMBO";

    public const string maxComboEvent = "MAXCOMBO";

    public const string gameOver = "GAMEOVER";

    public const string destroyLine = "DESTROYLINE";

    public const string teachEnd = "TEACHEND";

    public const string comboUp2 = "COMBOUP";

    public const string gameScoreWin = "GAMESCOREWIN";

    public const string gameStarWin = "GAMESTARWIN";

    public const string gameScoreFailed = "GAMESCOREFAILED";

    public const string gameStarFailed = "GAMESTARFAILED";

    public const string SubtractStar = "SUBTRACTSTAR";

    public const string SubtractStarUpdateUI = "SUBTRACTSTARUPDATEUI";

    [Header("拖拽方块的颜色")]
    public Color BlockColor;

    [Header("拖拽方块的贴图")]
    public Sprite[] BlockSprites;

    [Header("拖拽方块的空白贴图，配合星星 一块使用")]
    public Sprite[] ClearSprites;

    [Header("方块贴图对应的食物贴图")]
    public Sprite[] FoodSprites;

    [Header("方块贴图对应的星星贴图")]
    public Sprite[] StarSprites;

    [Header("拖拽到棋盘上之后棋盘对应的的颜色的alph值，值越小颜色越深")]
    [Range(0f, 1f)]
    public float boardDragColor_A = 0.5f;

    [Header("控制生成的三个可拖拽方块的在屏幕上的高度")]
    [Range(0f, 5f)]
    public float BlockOffsetY = 1f;

    #region 基础加分
    [Header("小方块基础分")]
    public int baseScore = 1;

    [Header("每次消除一行")]
    public int destroyOneLineScore = 10;

    [Header("每次同时消除两行")]
    public int destroyTwoLineScore = 30;

    [Header("每次同时消除三行")]
    public int destroyThreeLineScore = 60;

    [Header("每次同时消除四行")]
    public int destroyFourLineScore = 120;

    [Header("每次同时消除≥五行")]
    public int destroyFiveLineScore = 240;

    [Header("首次连击")]
    public int firstComboAddScore = 10;
    #endregion


    #region 连击加分
    [Header("二连")]
    public int twoComboAddScore = 20;

    [Header("三连")]
    public int threeComboAddScore = 30;

    [Header("四连")]
    public int fourComboAddScore = 40;

    [Header("五连")]
    public int fiveComboAddScore = 50;

    [Header("六连")]
    public int sixComboAddScore = 100;

    [Header("七连")]
    public int sevenComboAddScore = 150;

    [Header("八连")]
    public int eightComboAddScore = 200;

    [Header("九连")]
    public int nineComboAddScore = 400;

    [Header("≥十连")]
    public int tenComboAddScore = 800;
    #endregion

    #region UI相关
    [Header("游戏结束之后的倒计时时间")]
    public float GameOverCountDown=5f;
    #endregion

    [Header("Combo间隔时间，依次为0，1，2，3，4")]
    public float[] comboTimeLimits = { 30f, 30f, 20f, 15f, 10f, 5f };

    public const int LevelCount=15;

    public const int SinglePageLevelItem = 4;

    public Sprite LevelUnLock;

    public Sprite LevelSelected;

    public Sprite LevelLock;

    public Dictionary<int, int> destroyLineScores;

    public Dictionary<int, int> comboScores;

    public void Initialized()
    {

        destroyLineScores = new Dictionary<int, int>()
        {
            { 1, destroyOneLineScore },
            { 2, destroyTwoLineScore },
            { 3, destroyThreeLineScore },
            { 4, destroyFourLineScore },
            { 5, destroyFiveLineScore },
        };

        comboScores = new Dictionary<int, int>()
        {
            { 1, firstComboAddScore },
            { 2, twoComboAddScore },
            { 3, threeComboAddScore },
            { 4, fourComboAddScore },
            { 5, fiveComboAddScore },
            { 6, sixComboAddScore },
            { 7, sevenComboAddScore },
            { 8, eightComboAddScore },
            { 9, nineComboAddScore },
            { 10, tenComboAddScore }
        };
    }

    public List<int> GetBlockSpritesIndex()
    {
        int[] indexs = Enumerable.Range(0, BlockSprites.Length).ToArray();
       
        List<int> rangeIndex= indexs.RandomDisticncElements(3);

        return rangeIndex;
    }

    public Sprite[] GetBlockSprites(int index)
    {
        return new Sprite[2] { BlockSprites[index], FoodSprites[index] };
    }

    public Sprite[] GetBlockClearSprites(int index)
    {
        return new Sprite[2] { ClearSprites[index], ClearSprites[index] };
    }

    public Sprite[] GetFirstGameSprite(int index)
    {
        if (index >= BlockSprites.Length)
        {
             index = Random.Range(0, BlockSprites.Length);
        }
        return new Sprite[] { BlockSprites[index], FoodSprites[index] };
    }

    public Sprite GetStarSprite(int index)
    {
        if (index < 0)
            index = 0;
        return StarSprites[index];
    }

    public int CheckAddScore(int removeBlockLine, int comboNum)
    {
        int addScore = 0;
        if (removeBlockLine > 0 && removeBlockLine < 5)
        {
            addScore += destroyLineScores[removeBlockLine];
        }
        else if (removeBlockLine >= 5)
        {
            addScore += destroyFiveLineScore;
        }

        if (comboNum > 0 && comboNum < 10)
        {
            addScore += comboScores[comboNum];
        }
        else if (comboNum >= 10)
        {
            addScore += tenComboAddScore;
        }

        return addScore;
    }
}
