using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager instance;

    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GameDataManager)) as GameDataManager;
            }
            return instance;
        }
    }

    private const string GAMEDATAJSON = "GAMEDATAJSON";

    private const string GAMESETTINGJSON = "GAMESETTINGJSON";

    private const string FIRSTGAME = "FIRSTGAME";

    private const string LEVELDATA = "LEVELDATA";

    [SerializeField, Serializable]
    public class GameData
    {
        public int maxScore;
        public int maxComobo;
    }

    [SerializeField, Serializable]
    public class GameSetting
    {
        public bool soundState;
        public bool bgmState;
    }

    [SerializeField, Serializable]
    public class LevelData
    {
        public int levelIndex;
        public bool unlock;
    }

    [SerializeField, Serializable]
    public class LevelDatas
    {
        public int nowPlayLevel;
        public List<LevelData> levelDataList;
    }

    [HideInInspector]
    public GameData nowGameData;
    [HideInInspector]
    public GameSetting nowGameSetting;
    [HideInInspector]
    public bool firstGame;

    public LevelDatas levelDatas;

    public void Initialized()
    {
        nowGameData = GetGameData();
        nowGameSetting = GetGameSetting();
        firstGame = CheckFirstGame();
        levelDatas = GetLevelDatas();
    }

    public LevelDatas GetLevelDatas()
    {
        LevelDatas levelDatas = new LevelDatas();
        levelDatas.levelDataList = new List<LevelData>();
        string levelDatasStr = PlayerPrefs.GetString(LEVELDATA);
        if (levelDatasStr.Equals("")||levelDatasStr==null)
        {
            levelDatas.nowPlayLevel = 0;
            int levelNums = GameConstManager.LevelCount * 4;
            for (int i = 0; i < levelNums; i++)
            {
                LevelData levelData = new LevelData();
                levelData.levelIndex = i;
                levelData.unlock = false;
                levelDatas.levelDataList.Add(levelData);
            }
        }
        else
        {
            levelDatas = JsonUtility.FromJson<LevelDatas>(levelDatasStr);
        }
        DebugHelper.Log(levelDatasStr);
        return levelDatas;
    }

    public LevelData GetLevelData(int index)
    {
        if (levelDatas != null && index < levelDatas.levelDataList.Count)
        {
            return levelDatas.levelDataList[index];
        }
        return null;
    }

    public void UnLockLevelData(int levelIndex)
    {
        if (levelDatas != null)
        {
            levelDatas.levelDataList[levelIndex].unlock = true;
        }
    }

    public int GetNowLevelIndex()
    {
        for (int i = 0; i < levelDatas.levelDataList.Count; i++)
        {
            if (!levelDatas.levelDataList[i].unlock)
            {
                return i;
            }
        }
        return 0;
    }


    public bool CheckFirstGame()
    {
        string str = PlayerPrefs.GetString(FIRSTGAME);
        if (str.Equals("") || str == null)
        {
            return true;
        }
        return false;
    }

    public void NoFirstGame()
    {
        PlayerPrefs.SetString(FIRSTGAME, "false");
    }

    public void SaveGameDate(GameData data)
    {
        string datastr = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GAMEDATAJSON, datastr);
    }

    public GameData GetGameData()
    {
        GameData data = new GameData();

        if (CheckGameDataIsEmpty())
        {
            data.maxScore = 0;
            data.maxComobo = 0;
        }
        else
        {
            string str = PlayerPrefs.GetString(GAMEDATAJSON);
            data = JsonUtility.FromJson<GameData>(str);
        }
        return data;
    }

    public bool CheckGameDataIsEmpty()
    {
        string datastr = PlayerPrefs.GetString(GAMEDATAJSON);
        if (datastr.Equals("") || datastr == null)
        {
            return true;
        }
        return false;
    }

    public int GetMaxScore()
    {
        return nowGameData.maxScore;
    }

    public void UpdateMaxScore(int score)
    {
        nowGameData.maxScore = score;
    }

    public bool CheckMaxScore(int score)
    {
        if (score > nowGameData.maxScore)
        {
            UpdateMaxScore(score);
            return true;
        }
        return false;
    }

    public int GetMaxComobo()
    {
        return nowGameData.maxComobo;
    }

    public void UpdateMaxComobo(int comobo)
    {
        nowGameData.maxComobo = comobo;
    }

    public bool CheckMaxComobo(int comobo)
    {
        if (comobo > nowGameData.maxScore)
        {
            UpdateMaxComobo(comobo);
            return true;
        }
        return false;
    }

    public GameSetting GetGameSetting()
    {
        GameSetting setting = new GameSetting();
        string settingStr = PlayerPrefs.GetString(GAMESETTINGJSON);
        if (settingStr.Equals("") || settingStr == null)
        {
            setting.soundState = true;
            setting.bgmState = true;
        }
        else
        {
            setting = JsonUtility.FromJson<GameSetting>(settingStr);
        }
        return setting;
    }

    private void SaveGameSetting()
    {
        string str = JsonUtility.ToJson(nowGameSetting);
        PlayerPrefs.SetString(GAMESETTINGJSON, str);
    }

    private void SaveLevelData()
    {
        string str = JsonUtility.ToJson(levelDatas);
        PlayerPrefs.SetString(LEVELDATA, str);
    }

    public bool GetSoundState()
    {
        return nowGameSetting.soundState;
    }

    public bool GetBgmState()
    {
        return nowGameSetting.bgmState;
    }

    public bool ChangeSoundState()
    {
        bool state = nowGameSetting.soundState;
        nowGameSetting.soundState = !state;
        return nowGameSetting.soundState;
    }

    public bool ChangeBgmState()
    {
        bool state = nowGameSetting.bgmState;
        nowGameSetting.bgmState = !state;
        return nowGameSetting.bgmState;
    }

    public void ClearAllGameData()
    {
        PlayerPrefs.SetString(GAMEDATAJSON, "");
        PlayerPrefs.SetString(GAMESETTINGJSON, "");
        PlayerPrefs.SetString(FIRSTGAME, "");
    }

    public void ClearLevelData()
    {
        PlayerPrefs.SetString(LEVELDATA, "");
    }

    public void SaveAllGameData()
    {
        DebugHelper.Log("保存游戏数据");
        SaveGameDate(nowGameData);
        SaveGameSetting();
        SaveLevelData();
    }

    private void OnApplicationPause()
    {
        SaveAllGameData();
    }

    private void OnDestroy()
    {
        SaveAllGameData();
    }
}
