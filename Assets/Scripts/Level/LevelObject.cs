using MoleMole;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelObject : MonoBehaviour
{
    public TextMeshProUGUI LevelNameText;

    public Image LevelBgImage;

    public Button LevelButton;

    public int levelIndex;

    public GameDataManager.LevelData levelData;

    public LevelView levelView;

    public void Init()
    {
        levelView = GameObject.Find("LevelView").GetComponent<LevelView>();
        LevelButton.onClick.AddListener(OnLevelButtonClick);
    }

    public void UpdateLevelInfo(int _levelNum)
    {
        levelIndex = _levelNum - 1;
        LevelNameText.text = (_levelNum).ToString();
        levelData = GameDataManager.Instance.GetLevelData(levelIndex);
        LevelStateUpdate();
    }

    private void LevelStateUpdate()
    {
        LevelButton.gameObject.SetActive(true);
        LevelNameText.gameObject.SetActive(true);
        if (levelData.unlock)
        {
            //�ؿ�����
            LevelBgImage.sprite = GameConstManager.Instance.LevelUnLock;
        }
        else
        {
            if (levelData.levelIndex.Equals(GameDataManager.Instance.GetNowLevelIndex()))
            {
                //��ǰӦ����ɵĹؿ�
                LevelBgImage.sprite = GameConstManager.Instance.LevelSelected;
               
            }
            else
            {
                //�ؿ�δ����
                LevelBgImage.sprite = GameConstManager.Instance.LevelLock;
                LevelButton.gameObject.SetActive(false);
                LevelNameText.gameObject.SetActive(false);
            }
        }
        LevelBgImage.SetNativeSize();
    }

    private void OnLevelButtonClick()
    {
        Debug.Log("׼�����عؿ�:" + levelIndex);
        GameManager.Instance.nowLevelIndex = levelIndex;
        GameManager.Instance.GameStart(true);
        Singleton<ContextManager>.Instance.Push(new GameViewContext());
    }

    private void OnDestroy()
    {
        LevelButton.onClick.RemoveAllListeners();
    }
}
