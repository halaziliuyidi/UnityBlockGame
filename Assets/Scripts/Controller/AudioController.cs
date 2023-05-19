using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private StringBuilder audioName = new StringBuilder();

    public void PlayButtonClickAudio()
    {
        AudioManager.PlayAudio("ButtonClick", false);
    }

    public void PlayStartGameAudio()
    {
        AudioManager.PlayAudio("StartGame", false);
    }

    public void PlayDragAudio()
    {
        AudioManager.PlayAudio("Drag", false);
    }

    public void PlayPlaceAudio()
    {
        AudioManager.PlayAudio("Place", false);
    }

    public void PlayDestroyLineAudio()
    {
        AudioManager.PlayAudio("DestroyLine", true);
    }

    public void PlayComboAudio(int comboNum)
    {
        audioName.Clear();
        if (comboNum > 5)
        {
            comboNum = 5;
        }
        audioName.AppendFormat("Combo{0}", comboNum);

        AudioManager.PlayAudio(audioName.ToString(), true);
        audioName.Length = 0;
    }

    public void PlayDestroyLineLevelAudio(int destoryLine)
    {
        audioName.Clear();
        switch (destoryLine)
        {
            case 1:
                PlayDestroyLineAudio();
                break;
            case 2:
                audioName.Append("Good");
                break;
            case 3:
                audioName.Append("Nice");
                break;
            case 4:
                audioName.Append("Great");
                break;
            default:
                if (destoryLine >= 5)
                {
                    audioName.Append("Amazing");
                }
                break;
        }
        if (audioName.Length > 0)
        {
            AudioManager.PlayAudio(audioName.ToString(), true);
        }
        audioName.Length = 0;
    }

    public void PlayNoSapceAudio()
    {
        AudioManager.PlayAudio("NoSapce", false);
    }

    public void PlayFailedAudio()
    {
        AudioManager.PlayAudio("Failed", false);
    }

    public void PlayCountAudio()
    {
        AudioManager.PlayAudio("count", false);
    }

    public void PlayEatAudio()
    {
        AudioManager.PlayAudio("eat", true);
    }


}
