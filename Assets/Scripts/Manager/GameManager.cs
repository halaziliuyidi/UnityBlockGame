using MoleMole;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return instance;
        }
    }

    [Header("测试模式")]
    public bool TestMode;
    [Header("关闭所有Log")]
    public bool CloseAllLog;
    [Header("是否设置指定帧率,不设置为-1")]
    public int targetFrameRate;
    [HideInInspector]
    public bool gameOver = false;
    [HideInInspector]
    public bool paused = false;
    private bool showedHighScore;
    [HideInInspector]
    public int nowGameScore = 0;
    [HideInInspector]
    public AudioController audioController;

    public FxController fxController;

    public int nowLevelIndex;

    public bool nowIsLevelMode;

    public Level nowLevel;

    public Vector3[] starPos;

    public SkeletonAnimatorController skeletonAnimatorController;

    private Coroutine gameFadeCor;

    private Coroutine gameReviveCor;



    private void Awake()
    {

#if !UNITY_EDITOR
                TestMode = false;
#endif

        instance = Instance;

        Application.targetFrameRate = targetFrameRate;

        GameInitialized();

    }

    public void GameInitialized()
    {
        nowIsLevelMode = false;
        nowLevel=new Level();
        GameConstManager.Instance.Initialized();
        GameDataManager.Instance.Initialized();
        nowLevelIndex=GameDataManager.Instance.GetNowLevelIndex();
        AudioManager.Instance.Initialized();
        InputManager.Instance.Initialized();
        DestroyManager.Instance.Initialized();
        ComboManager.Instance.Initialized();
        BoardManager.Instance.Initialized();
        audioController = GetComponent<AudioController>();
        if (audioController == null)
        {
            audioController = gameObject.AddComponent<AudioController>();
        }
        gameOver = false;
        paused = false;
        showedHighScore = false;
        EventManager.AddListener(GameConstManager.destroyLine, OnDestroyLine);
        UIRoot();

        if (GameDataManager.Instance.firstGame)
        {
            GameStart();
        }
    }

    private void UIRoot()
    {
        Singleton<UIManager>.Create();
        Singleton<ContextManager>.Create();
    }

    public void GameStart(bool isLevelMode=false)
    {
        nowIsLevelMode = isLevelMode;
        BoardManager.Instance.BoardBlockInit(isLevelMode,nowLevelIndex);
        BoardManager.Instance.StartGame();
        audioController.PlayStartGameAudio();
    }

    public void GameOver()
    {
        gameOver = true;
        StopFadeCor();
        StopReviveCor();
        gameFadeCor = StartCoroutine(WaitForFade());
        EventManager.TriggerEvent(GameConstManager.gameOver);
        audioController.PlayNoSapceAudio();
    }

    public void GamePaused()
    {
        paused = true;
    }

    public void GameContinue()
    {
        paused = false;
    }

    public void GameRestart()
    {
        gameOver = false;
        paused = false;
        showedHighScore = false;
        ResetGameObjects();
        BoardManager.Instance.BoardBlockInit(nowIsLevelMode, nowLevelIndex);
        BoardManager.Instance.StartGame();
        audioController.PlayStartGameAudio();
    }

    public void ToHome()
    {
        gameOver = false;
        paused = false;
        showedHighScore = false;
        ResetGameObjects();
        nowIsLevelMode = false;
        nowLevel = new Level();
    }

    public void NextLevel()
    {
        gameOver = false;
        paused = false;
        showedHighScore = false;
        ResetGameObjects();
        nowIsLevelMode = false;
        nowLevel = new Level();
        nowLevelIndex += 1;
        GameStart(true);
    }

    public void RetryLevel()
    {
        gameOver = false;
        paused = false;
        showedHighScore = false;
        ResetGameObjects();
        nowIsLevelMode = false;
        nowLevel = new Level();
        GameStart(true);
    }

    public void GameStop()
    {
        gameOver = false;
        paused = false;
    }

    public void GameRevive()
    {
        StopFadeCor();
        StopReviveCor();
        gameReviveCor = StartCoroutine(WaitForRevive());
        BoardManager.Instance.ReviveBoard();
        ComboManager.Instance.ResetCombo();
        gameOver = false;
        paused = false;
    }

    public void GameScoreWin()
    {
        EventManager.TriggerEvent(GameConstManager.gameScoreWin);
        UnLockLevel(nowLevelIndex);
        gameOver = true;
        paused = true;
    }

    public void GameStarWin()
    {
        EventManager.TriggerEvent(GameConstManager.gameStarWin);
        UnLockLevel(nowLevelIndex);
        gameOver = true;
        paused = true;
    }

    //用于游戏失败和游戏胜利或者游戏重新开始
    //棋盘重置，方块重置，当前游戏数据重置
    public void ResetGameObjects()
    {
        BoardManager.Instance.ResetBoard();
        ComboManager.Instance.ResetCombo();
        gameOver = false;
        paused = false;
        nowGameScore = 0;
    }

    private void StopReviveCor()
    {
        if (gameReviveCor != null)
        {
            StopCoroutine(gameReviveCor);
        }
        gameReviveCor = null;
    }

    public IEnumerator WaitForRevive()
    {
        for (int y = GameConstManager.Board_Size - 1; y >= 0; y--)
        {
            for (int x = 0; x < GameConstManager.Board_Size; x++)
            {
                BlockTile b = BoardManager.Instance.boardBlocks[x, y];
                if (b)
                    b.Fade(0.25f, GameConstManager.Instance.BlockColor);

                if (x % 2 == 0)
                    yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(0.25f);
        StopReviveCor();
        gameOver = false;
    }

    private void StopFadeCor()
    {
        if (gameFadeCor != null)
        {
            StopCoroutine(gameFadeCor);
        }
        gameFadeCor = null;
    }

    public IEnumerator WaitForFade()
    {
        gameOver = true;
        for (int y = GameConstManager.Board_Size - 1; y >= 0; y--)
        {
            for (int x = 0; x < GameConstManager.Board_Size; x++)
            {
                BlockTile b = BoardManager.Instance.boardBlocks[x, y];
                if (b)
                    b.Fade(0.25f, new Color(0.09f, 0.122f, 0.153f));

                if (x % 2 == 0)
                    yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(0.25f);
        StopFadeCor();
    }

    public async void AddScore(int destroyedLines, int comboNum, Vector3 position)
    {
        int addScore = GameConstManager.Instance.CheckAddScore(destroyedLines, comboNum);
        int oldScore = nowGameScore;
        nowGameScore += addScore;
        if (nowGameScore >= nowLevel.targetScore && nowIsLevelMode && !nowLevel.isStarLevel)
        {
            //分数模式下分数达标，游戏胜利
            DebugHelper.Log("分数达标，游戏胜利");
            GameScoreWin();
        }
        if (nowGameScore > 1000)
        {
            int level = nowGameScore / 1000;
            if (!nowIsLevelMode)
            { 
                //增加困难等级
                BoardManager.Instance.ChangedGameDifficulty(level);
                return;
            }
           
        }
        if (!nowIsLevelMode)
        {
            if (GameDataManager.Instance.CheckMaxScore(nowGameScore))
            {
                MaxScore(nowGameScore);
            }
        }
        if (comboNum > 0)
        {
            Combo(comboNum, position);
            await Task.Delay(600);
        }
        audioController.PlayDestroyLineLevelAudio(destroyedLines);
        EventManager.TriggerEvent(GameConstManager.addScoreEvent, nowGameScore, oldScore, destroyedLines, position);

    }

    public void AddBaseScore(int blockTitlesNum, Vector3 position)
    {
        int addScore = blockTitlesNum * GameConstManager.Instance.baseScore;
        int oldScore = nowGameScore;
        nowGameScore += addScore;
        EventManager.TriggerEvent(GameConstManager.addBaseScore, nowGameScore, oldScore, position);
        if (!nowIsLevelMode)
        {
            if (GameDataManager.Instance.CheckMaxScore(nowGameScore))
            {
                MaxScore(nowGameScore);
            }
        }
    }

    public void MaxScore(int maxScore)
    {
        EventManager.TriggerEvent(GameConstManager.maxScoreEvent, maxScore,showedHighScore);
    }

    public void ShowedHighScore()
    {
        showedHighScore = true;
    }

    public void Combo(int comboNum, Vector3 position)
    {
        if (GameDataManager.Instance.CheckMaxComobo(comboNum))
        {
            MaxCombo(comboNum);
        }
        if (comboNum > 2)
        {
            EventManager.TriggerEvent(GameConstManager.comboUp2);
        }
        fxController.SpawnComboMatchFx(position);
        EventManager.TriggerEvent(GameConstManager.comboEvent, comboNum, position);
        audioController.PlayComboAudio(comboNum);
    }

    public void MaxCombo(int maxComboNum)
    {
        EventManager.TriggerEvent(GameConstManager.maxComboEvent, maxComboNum);
    }

    public void OnDestroyLine(object[] args)
    {
        Vector3 vector3 = (Vector3)args[0];
        string str = args[1].ToString();
        switch (str)
        {
            case "v":
                fxController.SpawnDestroyLineFX(vector3, 1f, true);
                break;
            case "h":
                fxController.SpawnDestroyLineFX(vector3, 1f, false);
                break;
            default:
                break;
        }
    }

    public void UnLockLevel(int levelIndex)
    {
        GameDataManager.Instance.UnLockLevelData(levelIndex);
        DebugHelper.LogFormat("解锁了关卡:{0}",levelIndex);
    }

    public void SubtractStarNum(int index,int num)
    {
        EventManager.TriggerEvent(GameConstManager.SubtractStar, index, num);
    }

    public void SetStarPos(Vector3[] pos)
    {
        starPos = pos;
    }

    public Vector3 GetStarPos(int index)
    {
        return starPos[index];
    }

    public void OnApplicationQuit()
    {
        GameDataManager.Instance.SaveAllGameData();
    }

    public void OnDestroy()
    {
        EventManager.RemoveAllEvents();
    }
}
