using MoleMole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingView : MonoBehaviour
{

    public Animator settingAnimator;

    private void OnEnable()
    {
        OnEnter();
    }

    private void OnDisable()
    {
        OnExit();
    }

    public void OnEnter()
    {
        GameManager.Instance.GamePaused();
        ViewInit();
    }

    public void OnExit()
    {
        GameManager.Instance.GameContinue();
    }

    public void ViewInit()
    {
        if (GameDataManager.Instance.GetSoundState())
        {
            settingAnimator.Play("SoundIsOpen", 1);
        }
        else
        {
            settingAnimator.Play("SoundIsClose", 1);
        }

        if (GameDataManager.Instance.GetBgmState())
        {
            settingAnimator.Play("BgmIsOpen", 2);
        }
        else
        {
            settingAnimator.Play("BgmIsClose", 2);
        }
    }

    public void CloseBtnClick()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance.audioController.PlayButtonClickAudio();
    }

    public void HomeBtnClick()
    {
        GameManager.Instance.ToHome();
        Singleton<ContextManager>.Instance.Push(new StartViewContext());
        this.gameObject.SetActive(false);
        GameManager.Instance.audioController.PlayButtonClickAudio();
    }

    public void ResetBtnClick()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance.GameRestart();
        GameManager.Instance.audioController.PlayButtonClickAudio();
        Singleton<ContextManager>.Instance.Push(new GameViewContext());
    }

    public void SoundBtnClick()
    {
        if (GameDataManager.Instance.ChangeSoundState())
        {
            //������
            settingAnimator.Play("OpenSound",1);
        }
        else
        {
            //�ر�����
            settingAnimator.Play("CloseSound",1);
        }
        AudioManager.Instance.SetSoundState(GameDataManager.Instance.GetSoundState());
    }

    public void BgmBtnClik()
    {
        if (GameDataManager.Instance.ChangeBgmState())
        {
            //��BGM
            settingAnimator.Play("OpenBgm",2);
        }
        else
        {
            //�ر�BGM
            settingAnimator.Play("CloseBgm",2);
        }
        AudioManager.Instance.SetBgmState(GameDataManager.Instance.GetBgmState());
    }
}
